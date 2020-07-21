namespace Testflow.Utility.Controls
{
    partial class ErrorInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorInfoForm));
            this.label_message = new System.Windows.Forms.Label();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.textBox_source = new System.Windows.Forms.TextBox();
            this.label_source = new System.Windows.Forms.Label();
            this.textBox_errorCode = new System.Windows.Forms.TextBox();
            this.label_errorCode = new System.Windows.Forms.Label();
            this.textBox_stackTrace = new System.Windows.Forms.TextBox();
            this.label_detail = new System.Windows.Forms.Label();
            this.pictureBox_icon = new System.Windows.Forms.PictureBox();
            this.label_errorHead = new System.Windows.Forms.Label();
            this.textBox_errorIntro = new System.Windows.Forms.TextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.button_save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // label_message
            // 
            this.label_message.AutoSize = true;
            this.label_message.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_message.Location = new System.Drawing.Point(12, 115);
            this.label_message.Name = "label_message";
            this.label_message.Size = new System.Drawing.Size(68, 14);
            this.label_message.TabIndex = 0;
            this.label_message.Text = "Message:";
            // 
            // textBox_message
            // 
            this.textBox_message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_message.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_message.Location = new System.Drawing.Point(97, 111);
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.ReadOnly = true;
            this.textBox_message.Size = new System.Drawing.Size(538, 22);
            this.textBox_message.TabIndex = 1;
            // 
            // textBox_source
            // 
            this.textBox_source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_source.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_source.Location = new System.Drawing.Point(97, 165);
            this.textBox_source.Name = "textBox_source";
            this.textBox_source.ReadOnly = true;
            this.textBox_source.Size = new System.Drawing.Size(538, 22);
            this.textBox_source.TabIndex = 3;
            // 
            // label_source
            // 
            this.label_source.AutoSize = true;
            this.label_source.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_source.Location = new System.Drawing.Point(12, 169);
            this.label_source.Name = "label_source";
            this.label_source.Size = new System.Drawing.Size(55, 14);
            this.label_source.TabIndex = 2;
            this.label_source.Text = "Source:";
            // 
            // textBox_errorCode
            // 
            this.textBox_errorCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_errorCode.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_errorCode.Location = new System.Drawing.Point(97, 138);
            this.textBox_errorCode.Name = "textBox_errorCode";
            this.textBox_errorCode.ReadOnly = true;
            this.textBox_errorCode.Size = new System.Drawing.Size(538, 22);
            this.textBox_errorCode.TabIndex = 5;
            // 
            // label_errorCode
            // 
            this.label_errorCode.AutoSize = true;
            this.label_errorCode.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_errorCode.Location = new System.Drawing.Point(12, 142);
            this.label_errorCode.Name = "label_errorCode";
            this.label_errorCode.Size = new System.Drawing.Size(80, 14);
            this.label_errorCode.TabIndex = 4;
            this.label_errorCode.Text = "Error Code:";
            // 
            // textBox_stackTrace
            // 
            this.textBox_stackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_stackTrace.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_stackTrace.Location = new System.Drawing.Point(97, 192);
            this.textBox_stackTrace.Multiline = true;
            this.textBox_stackTrace.Name = "textBox_stackTrace";
            this.textBox_stackTrace.ReadOnly = true;
            this.textBox_stackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_stackTrace.Size = new System.Drawing.Size(538, 182);
            this.textBox_stackTrace.TabIndex = 7;
            // 
            // label_detail
            // 
            this.label_detail.AutoSize = true;
            this.label_detail.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_detail.Location = new System.Drawing.Point(12, 194);
            this.label_detail.Name = "label_detail";
            this.label_detail.Size = new System.Drawing.Size(84, 14);
            this.label_detail.TabIndex = 6;
            this.label_detail.Text = "Stack Trace:";
            // 
            // pictureBox_icon
            // 
            this.pictureBox_icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_icon.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_icon.Image")));
            this.pictureBox_icon.Location = new System.Drawing.Point(-9, 5);
            this.pictureBox_icon.Name = "pictureBox_icon";
            this.pictureBox_icon.Size = new System.Drawing.Size(133, 100);
            this.pictureBox_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_icon.TabIndex = 8;
            this.pictureBox_icon.TabStop = false;
            // 
            // label_errorHead
            // 
            this.label_errorHead.Font = new System.Drawing.Font("Verdana", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_errorHead.Location = new System.Drawing.Point(126, 11);
            this.label_errorHead.Name = "label_errorHead";
            this.label_errorHead.Size = new System.Drawing.Size(498, 35);
            this.label_errorHead.TabIndex = 9;
            this.label_errorHead.Text = "ErrorHead";
            // 
            // textBox_errorIntro
            // 
            this.textBox_errorIntro.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_errorIntro.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_errorIntro.Location = new System.Drawing.Point(130, 55);
            this.textBox_errorIntro.Multiline = true;
            this.textBox_errorIntro.Name = "textBox_errorIntro";
            this.textBox_errorIntro.ReadOnly = true;
            this.textBox_errorIntro.Size = new System.Drawing.Size(505, 48);
            this.textBox_errorIntro.TabIndex = 10;
            this.textBox_errorIntro.Text = "ErrorIntroduction";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "ErrorInfo";
            this.saveFileDialog.Filter = "Text files|*.txt";
            // 
            // button_save
            // 
            this.button_save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_save.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_save.Location = new System.Drawing.Point(532, 377);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(103, 23);
            this.button_save.TabIndex = 11;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // ErrorInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 403);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.textBox_errorIntro);
            this.Controls.Add(this.label_errorHead);
            this.Controls.Add(this.pictureBox_icon);
            this.Controls.Add(this.textBox_stackTrace);
            this.Controls.Add(this.label_detail);
            this.Controls.Add(this.textBox_errorCode);
            this.Controls.Add(this.label_errorCode);
            this.Controls.Add(this.textBox_source);
            this.Controls.Add(this.label_source);
            this.Controls.Add(this.textBox_message);
            this.Controls.Add(this.label_message);
            this.MinimumSize = new System.Drawing.Size(663, 441);
            this.Name = "ErrorInfoForm";
            this.Text = "Error";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_message;
        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.TextBox textBox_source;
        private System.Windows.Forms.Label label_source;
        private System.Windows.Forms.TextBox textBox_errorCode;
        private System.Windows.Forms.Label label_errorCode;
        private System.Windows.Forms.TextBox textBox_stackTrace;
        private System.Windows.Forms.Label label_detail;
        private System.Windows.Forms.PictureBox pictureBox_icon;
        private System.Windows.Forms.Label label_errorHead;
        private System.Windows.Forms.TextBox textBox_errorIntro;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button button_save;
    }
}