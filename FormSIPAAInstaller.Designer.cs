namespace SipAAInstaller
{
    partial class FmSipAAInstaller
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonInstall = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.ContinueStopService = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.ContinueCopy = new System.Windows.Forms.Button();
            this.FirstStart = new System.Windows.Forms.Button();
            this.Unamebox = new System.Windows.Forms.TextBox();
            this.PWDBox = new System.Windows.Forms.TextBox();
            this.UserName = new System.Windows.Forms.Label();
            this.PassWord = new System.Windows.Forms.Label();
            this.UserConfirm = new System.Windows.Forms.Label();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.Cancel1 = new System.Windows.Forms.Button();
            this.CPassWord = new System.Windows.Forms.Label();
            this.CPWD = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(340, 182);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(75, 23);
            this.buttonInstall.TabIndex = 3;
            this.buttonInstall.Text = "下一步";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("宋体", 11F);
            this.labelInfo.Location = new System.Drawing.Point(56, 53);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(467, 71);
            this.labelInfo.TabIndex = 5;
            this.labelInfo.Text = "label5";
            // 
            // ContinueStopService
            // 
            this.ContinueStopService.Location = new System.Drawing.Point(340, 182);
            this.ContinueStopService.Name = "ContinueStopService";
            this.ContinueStopService.Size = new System.Drawing.Size(75, 23);
            this.ContinueStopService.TabIndex = 7;
            this.ContinueStopService.Text = "下一步";
            this.ContinueStopService.UseVisualStyleBackColor = true;
            this.ContinueStopService.Click += new System.EventHandler(this.ContinueStopService_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(441, 182);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 8;
            this.Cancel.Text = "取消";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.CancelWithSvcRun_Click);
            // 
            // ContinueCopy
            // 
            this.ContinueCopy.Location = new System.Drawing.Point(340, 182);
            this.ContinueCopy.Name = "ContinueCopy";
            this.ContinueCopy.Size = new System.Drawing.Size(75, 23);
            this.ContinueCopy.TabIndex = 9;
            this.ContinueCopy.Text = "下一步";
            this.ContinueCopy.UseVisualStyleBackColor = true;
            this.ContinueCopy.Click += new System.EventHandler(this.ContinueCopy_Click);
            // 
            // FirstStart
            // 
            this.FirstStart.Location = new System.Drawing.Point(340, 182);
            this.FirstStart.Name = "FirstStart";
            this.FirstStart.Size = new System.Drawing.Size(75, 23);
            this.FirstStart.TabIndex = 10;
            this.FirstStart.Text = "下一步";
            this.FirstStart.UseVisualStyleBackColor = true;
            this.FirstStart.Click += new System.EventHandler(this.FirstStart_Click);
            // 
            // Unamebox
            // 
            this.Unamebox.Location = new System.Drawing.Point(238, 79);
            this.Unamebox.Name = "Unamebox";
            this.Unamebox.Size = new System.Drawing.Size(150, 21);
            this.Unamebox.TabIndex = 11;
            // 
            // PWDBox
            // 
            this.PWDBox.Location = new System.Drawing.Point(238, 115);
            this.PWDBox.Name = "PWDBox";
            this.PWDBox.PasswordChar = '*';
            this.PWDBox.Size = new System.Drawing.Size(150, 21);
            this.PWDBox.TabIndex = 12;
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Location = new System.Drawing.Point(174, 82);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(53, 12);
            this.UserName.TabIndex = 13;
            this.UserName.Text = "用户名：";
            // 
            // PassWord
            // 
            this.PassWord.AutoSize = true;
            this.PassWord.Location = new System.Drawing.Point(176, 118);
            this.PassWord.Name = "PassWord";
            this.PassWord.Size = new System.Drawing.Size(53, 12);
            this.PassWord.TabIndex = 14;
            this.PassWord.Text = "密  码：";
            // 
            // UserConfirm
            // 
            this.UserConfirm.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UserConfirm.Location = new System.Drawing.Point(54, 18);
            this.UserConfirm.Name = "UserConfirm";
            this.UserConfirm.Size = new System.Drawing.Size(467, 44);
            this.UserConfirm.TabIndex = 15;
            this.UserConfirm.Text = "label1";
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Location = new System.Drawing.Point(178, 188);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(75, 23);
            this.ConfirmButton.TabIndex = 17;
            this.ConfirmButton.Text = "确认";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // Cancel1
            // 
            this.Cancel1.Location = new System.Drawing.Point(311, 188);
            this.Cancel1.Name = "Cancel1";
            this.Cancel1.Size = new System.Drawing.Size(75, 23);
            this.Cancel1.TabIndex = 19;
            this.Cancel1.Text = "退出";
            this.Cancel1.UseVisualStyleBackColor = true;
            this.Cancel1.Click += new System.EventHandler(this.Cancel1_Click);
            // 
            // CPassWord
            // 
            this.CPassWord.AutoSize = true;
            this.CPassWord.Location = new System.Drawing.Point(138, 153);
            this.CPassWord.Name = "CPassWord";
            this.CPassWord.Size = new System.Drawing.Size(89, 12);
            this.CPassWord.TabIndex = 18;
            this.CPassWord.Text = "重新填写密码：";
            // 
            // CPWD
            // 
            this.CPWD.Location = new System.Drawing.Point(238, 150);
            this.CPWD.Name = "CPWD";
            this.CPWD.PasswordChar = '*';
            this.CPWD.Size = new System.Drawing.Size(150, 21);
            this.CPWD.TabIndex = 16;
            // 
            // FmSipAAInstaller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 255);
            this.Controls.Add(this.CPWD);
            this.Controls.Add(this.Unamebox);
            this.Controls.Add(this.PWDBox);
            this.Controls.Add(this.CPassWord);
            this.Controls.Add(this.Cancel1);
            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.UserConfirm);
            this.Controls.Add(this.PassWord);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.FirstStart);
            this.Controls.Add(this.ContinueCopy);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.ContinueStopService);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonInstall);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FmSipAAInstaller";
            this.Text = "SipAA Installer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FmSipAAInstaller_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button ContinueStopService;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button ContinueCopy;
        private System.Windows.Forms.Button FirstStart;
        private System.Windows.Forms.TextBox Unamebox;
        private System.Windows.Forms.TextBox PWDBox;
        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label PassWord;
        private System.Windows.Forms.Label UserConfirm;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.Button Cancel1;
        private System.Windows.Forms.Label CPassWord;
        private System.Windows.Forms.TextBox CPWD;
    }
}

