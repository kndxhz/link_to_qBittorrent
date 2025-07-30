using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qBHelper
{
    public partial class MainForm : Form
    {

        private string _sid = null;
        private readonly HttpClient _http = new HttpClient(new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true
        });

        private static readonly string IniPath =
            Path.Combine(
                Path.GetDirectoryName(
                    new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                "config.ini");


        public MainForm(string[] args = null)
        {
            InitializeComponent();
            LoadConfig();
            CheckAssociationOnStart();
            button_login_Click(this, EventArgs.Empty);

            if (args != null && args.Length > 1)
                HandleCommandLine();
        }



        #region 配置读写
        private void LoadConfig()
        {

            if (!File.Exists(IniPath)) return;
            var lines = File.ReadAllLines(IniPath);
            foreach (var l in lines)
            {
                var kv = l.Split(new[] { '=' }, 2);
                if (kv.Length != 2) continue;
                switch (kv[0])
                {
                    case "url": input_server_url.Text = kv[1]; break;
                    case "user": input_user.Text = kv[1]; break;
                    case "pwd": input_password.Text = kv[1]; break;
                }
            }
        }

        private void SaveConfig()
        {
            File.WriteAllLines(IniPath, new[]
            {
                $"url={input_server_url.Text.Trim()}",
                $"user={input_user.Text.Trim()}",
                $"pwd={input_password.Text.Trim()}"
            });
        }
        #endregion

        #region 登录
        private async Task<bool> LoginAsync()
        {
            var url = input_server_url.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("服务器URL不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!url.EndsWith("/")) url += "/";
            var loginUrl = $"{url}api/v2/auth/login";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("username", input_user.Text.Trim()),
                new KeyValuePair<string,string>("password", input_password.Text.Trim())
            });

            var request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            request.Content = content;

            try
            {
                request.Headers.Referrer = new Uri(url);
                var resp = await _http.SendAsync(request);
                var respContent = await resp.Content.ReadAsStringAsync();

                if (resp.StatusCode == HttpStatusCode.OK && respContent == "Ok.")
                {
                    // 取出 SID
                    if (resp.Headers.TryGetValues("Set-Cookie", out var cookies))
                    {
                        var sid = cookies.FirstOrDefault(c => c.StartsWith("SID="));
                        
                        if (sid != null)
                        {
                            _sid = sid.Split(';')[0].Substring(4);
                            return true;
                        }
                    }
                    return false;

                }
                else if (resp.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("登录失败：IP被禁止（过多失败尝试）", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    MessageBox.Show($"登录失败: {resp.StatusCode}\n{respContent}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"登录时发生错误:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void button_login_Click(object sender, EventArgs e)
        {
            if (_sid != null)
            {
                label5.Text = "成功";
                label5.ForeColor = Color.Chartreuse;
                return;
            }
            bool ok = await LoginAsync();

            label5.Text = ok ? "成功" : "失败";
            label5.ForeColor = ok ? Color.Chartreuse : Color.OrangeRed;

                
            SaveConfig();
        }
        #endregion

        #region 协议关联
        private const string MagnetKey = @"magnet\shell\open\command";
        private const string TorrentKey = @"Software\Classes\.torrent\shell\open\command";

        private bool IsAssociated()
        {
            // 检查磁力链接关联（保持不变）
            using (var k = Registry.ClassesRoot.OpenSubKey(MagnetKey, false))
            {
                if (k == null) return false;
                var v = k.GetValue("") as string;
                if (v == null || !v.Contains(Application.ExecutablePath)) return false;

                using (var openKey = Registry.ClassesRoot.OpenSubKey(@"magnet\shell\open", false))
                {
                    if (openKey == null) return false;
                    var friendlyName = openKey.GetValue("FriendlyAppName") as string;
                    if (string.IsNullOrEmpty(friendlyName)) return false;
                }
            }

            // 检查.torrent文件关联（保持不变）
            using (var k = Registry.CurrentUser.OpenSubKey(TorrentKey, false))
            {
                if (k == null) return false;
                var v = k.GetValue("") as string;
                if (v == null || !v.Contains(Application.ExecutablePath)) return false;
            }

            return true;
        }

        private void SetAssociation(bool register)
        {
            bool success = false;
            string operation = register ? "注册" : "注销";

            try
            {
                if (register)
                {
                    // 注册代码（保持不变）
                    using (var magnetKey = Registry.ClassesRoot.CreateSubKey("magnet", true))
                    {
                        magnetKey.SetValue("", "URL:Magnet Protocol");
                        magnetKey.SetValue("URL Protocol", "");
                    }

                    using (var openKey = Registry.ClassesRoot.CreateSubKey(@"magnet\shell\open", true))
                    {
                        openKey.SetValue("FriendlyAppName", "qBHelper");
                    }

                    using (var cmdKey = Registry.ClassesRoot.CreateSubKey(MagnetKey, true))
                    {
                        var exePath = Application.ExecutablePath;
                        cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                    }

                    using (var key = Registry.CurrentUser.CreateSubKey(TorrentKey, true))
                    {
                        var exePath = Application.ExecutablePath;
                        key.SetValue("", $"\"{exePath}\" \"%1\"");
                    }
                }
                else
                {
                    // 精确删除注册时创建的键 ------------------ 修改点1
                    // 删除磁力链接关联（只删除我们创建的部分）
                    try
                    {
                        Registry.ClassesRoot.DeleteSubKey(MagnetKey, false);
                        Registry.ClassesRoot.DeleteSubKey(@"magnet\shell\open", false);
                        Registry.ClassesRoot.DeleteSubKey(@"magnet\shell", false);
                        Registry.ClassesRoot.DeleteSubKey("magnet", false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"删除磁力链接注册表键失败: {ex.Message}");
                    }

                    // 删除.torrent文件关联（只删除command键）
                    try
                    {
                        Registry.CurrentUser.DeleteSubKey(TorrentKey, false);
                        // 尝试清理空父键
                        var openParent = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.torrent\shell", true);
                        if (openParent?.SubKeyCount == 0)
                        {
                            Registry.CurrentUser.DeleteSubKey(@"Software\Classes\.torrent\shell", false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"删除.torrent注册表键失败: {ex.Message}");
                    }
                    MessageBox.Show("基本上注销成功\n关闭程序即可完全注销", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 刷新资源管理器
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
                //Thread.Sleep(3000); // 等待刷新
                success = register ? IsAssociated() : !IsAssociated();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{operation}关联时出错: {ex.Message}", "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // UI状态同步 ------------------ 修改点2
            label7.Text = success ? "成功" : "失败";
            label7.ForeColor = success ? Color.Chartreuse : Color.OrangeRed;


        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private void CheckAssociationOnStart()
        {
            bool ok = IsAssociated();
            label7.Text = ok ? "成功" : "失败";
            label7.ForeColor = ok ? Color.Chartreuse : Color.OrangeRed;

        }

        private void button_register_Click(object sender, EventArgs e) => SetAssociation(true);
        private void button_unregister_Click(object sender, EventArgs e) => SetAssociation(false);
        #endregion

        #region 接收 magnet / .torrent 并添加
        private void HandleCommandLine()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) return;

            string target = args[1];
            if (!target.StartsWith("magnet:") && !target.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
                return;

            if (MessageBox.Show($"是否添加到 qBittorrent?\r\n{target}", "添加种子",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AddTorrentAsync(target).ConfigureAwait(false);
            }
            else
            {
                Application.Exit();
            }
        }

        private async Task AddTorrentAsync(string uriOrFile)
        {
            if (!await EnsureLoggedIn())
            {
                MessageBox.Show("登录失败，无法添加种子", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var cats = await GetCategoriesAsync();
            if (cats == null || cats.Count == 0)
            {
                MessageBox.Show("无法获取分类列表", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var cat = PromptCategory(cats);
            if (string.IsNullOrEmpty(cat)) return;

            string url = input_server_url.Text.Trim();
            if (!url.EndsWith("/")) url += "/";
            string addUrl = $"{url}api/v2/torrents/add";

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    if (uriOrFile.StartsWith("magnet:") || uriOrFile.StartsWith("http"))
                    {
                        content.Add(new StringContent(uriOrFile), "urls");
                    }
                    else  // 本地 .torrent 文件
                    {
                        var fileBytes = File.ReadAllBytes(uriOrFile);
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-bittorrent");
                        content.Add(fileContent, "torrents", Path.GetFileName(uriOrFile));
                    }

                    content.Add(new StringContent(cat), "category");

                    var request = new HttpRequestMessage(HttpMethod.Post, addUrl) { Content = content };
                    request.Headers.Add("Cookie", $"SID={_sid}");

                    var resp = await _http.SendAsync(request);
                    if (resp.IsSuccessStatusCode)
                    {
                        MessageBox.Show("种子添加成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                        return;
                    }
                    else
                    {
                        var errorContent = await resp.Content.ReadAsStringAsync();
                        MessageBox.Show($"添加失败: {resp.StatusCode}\n{errorContent}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加种子时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> EnsureLoggedIn()
        {
            if (_sid != null) return true;
            return await LoginAsync();
        }

        private async Task<Dictionary<string, CategoryInfo>> GetCategoriesAsync()
        {
            string url = input_server_url.Text.Trim();
            if (!url.EndsWith("/")) url += "/";
            string catUrl = $"{url}api/v2/torrents/categories";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, catUrl);
                request.Headers.Add("Cookie", $"SID={_sid}");
                var resp = await _http.SendAsync(request);

                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, CategoryInfo>>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private string PromptCategory(Dictionary<string, CategoryInfo> cats)
        {
            var arr = cats.Keys.ToArray();
            if (arr.Length == 0) return string.Empty;

            using (var f = new Form())
            {
                f.Text = "选择分类";
                f.Size = new Size(300, 150);
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;

                var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top };
                cb.Items.AddRange(arr);
                cb.SelectedIndex = 0;

                var ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom };

                f.Controls.Add(cb);
                f.Controls.Add(ok);
                f.AcceptButton = ok;

                return f.ShowDialog() == DialogResult.OK ? cb.Text : string.Empty;
            }
        }

        public class CategoryInfo
        {
            public string name { get; set; }
            public string savePath { get; set; }
        }
        #endregion
    }
}