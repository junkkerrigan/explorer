namespace Explorer.Views
{
    partial class Explorer
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainWrapper = new System.Windows.Forms.Panel();
            this.FileView = new System.Windows.Forms.TreeView();
            this.DirectoryViewWrapper = new System.Windows.Forms.Panel();
            this.MainWrapper.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainWrapper
            // 
            this.MainWrapper.Controls.Add(this.DirectoryViewWrapper);
            this.MainWrapper.Controls.Add(this.FileView);
            this.MainWrapper.Location = new System.Drawing.Point(0, 30);
            this.MainWrapper.Name = "MainWrapper";
            this.MainWrapper.Size = new System.Drawing.Size(1297, 678);
            this.MainWrapper.TabIndex = 0;
            // 
            // FileView
            // 
            this.FileView.Location = new System.Drawing.Point(654, 15);
            this.FileView.Name = "FileView";
            this.FileView.Size = new System.Drawing.Size(643, 543);
            this.FileView.TabIndex = 1;
            // 
            // DirectoryViewWrapper
            // 
            this.DirectoryViewWrapper.Location = new System.Drawing.Point(0, 15);
            this.DirectoryViewWrapper.Name = "DirectoryViewWrapper";
            this.DirectoryViewWrapper.Size = new System.Drawing.Size(643, 543);
            this.DirectoryViewWrapper.TabIndex = 2;
            // 
            // Explorer
            // 
            this.ClientSize = new System.Drawing.Size(1297, 708);
            this.Controls.Add(this.MainWrapper);
            this.Name = "Explorer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "";
            this.Text = "Explorer";
            this.MainWrapper.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainWrapper;
        private System.Windows.Forms.TreeView FileView;
        private System.Windows.Forms.Panel DirectoryViewWrapper;
    }
}