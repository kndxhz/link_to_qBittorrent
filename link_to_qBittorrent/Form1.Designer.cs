namespace qBHelper
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.input_server_url = new System.Windows.Forms.TextBox();
            this.input_user = new System.Windows.Forms.TextBox();
            this.input_password = new System.Windows.Forms.TextBox();
            this.button_register = new System.Windows.Forms.Button();
            this.button_unregister = new System.Windows.Forms.Button();
            this.button_test_login = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // input_server_url
            // 
            this.input_server_url.Location = new System.Drawing.Point(80, 12);
            this.input_server_url.Name = "input_server_url";
            this.input_server_url.Size = new System.Drawing.Size(300, 21);
            this.input_server_url.TabIndex = 0;
            // 
            // input_user
            // 
            this.input_user.Location = new System.Drawing.Point(81, 44);
            this.input_user.Name = "input_user";
            this.input_user.Size = new System.Drawing.Size(298, 21);
            this.input_user.TabIndex = 1;
            // 
            // input_password
            // 
            this.input_password.Location = new System.Drawing.Point(80, 75);
            this.input_password.Name = "input_password";
            this.input_password.Size = new System.Drawing.Size(299, 21);
            this.input_password.TabIndex = 2;
            // 
            // button_register
            // 
            this.button_register.Location = new System.Drawing.Point(141, 145);
            this.button_register.Name = "button_register";
            this.button_register.Size = new System.Drawing.Size(116, 54);
            this.button_register.TabIndex = 3;
            this.button_register.Text = "注册协议";
            this.button_register.UseVisualStyleBackColor = true;
            this.button_register.Click += new System.EventHandler(this.button_register_Click);
            // 
            // button_unregister
            // 
            this.button_unregister.Location = new System.Drawing.Point(263, 145);
            this.button_unregister.Name = "button_unregister";
            this.button_unregister.Size = new System.Drawing.Size(116, 54);
            this.button_unregister.TabIndex = 4;
            this.button_unregister.Text = "注销协议";
            this.button_unregister.UseVisualStyleBackColor = true;
            this.button_unregister.Click += new System.EventHandler(this.button_unregister_Click);
            // 
            // button_test_login
            // 
            this.button_test_login.Location = new System.Drawing.Point(19, 145);
            this.button_test_login.Name = "button_test_login";
            this.button_test_login.Size = new System.Drawing.Size(116, 54);
            this.button_test_login.TabIndex = 5;
            this.button_test_login.Text = "尝试登录";
            this.button_test_login.UseVisualStyleBackColor = true;
            this.button_test_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "服务器地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "用户名";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "密码";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "服务器状态：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.DarkOrange;
            this.label5.Location = new System.Drawing.Point(100, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "未知";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(227, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "协议状态：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.DarkOrange;
            this.label7.Location = new System.Drawing.Point(298, 120);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "未知";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 210);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_test_login);
            this.Controls.Add(this.button_unregister);
            this.Controls.Add(this.button_register);
            this.Controls.Add(this.input_password);
            this.Controls.Add(this.input_user);
            this.Controls.Add(this.input_server_url);
            this.Name = "MainForm";
            this.Text = "link_to_qBittorrent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input_server_url;
        private System.Windows.Forms.TextBox input_user;
        private System.Windows.Forms.TextBox input_password;
        private System.Windows.Forms.Button button_register;
        private System.Windows.Forms.Button button_unregister;
        private System.Windows.Forms.Button button_test_login;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}

