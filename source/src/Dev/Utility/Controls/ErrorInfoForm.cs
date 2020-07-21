using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Testflow.Utility.Controls
{
    /// <summary>
    /// 错误窗体
    /// </summary>
    public partial class ErrorInfoForm : Form
    {
        /// <summary>
        /// 显示错误信息窗体
        /// </summary>
        public static void ShowErrorInfoForm(string errorHead, string errorIntroduction, Exception ex)
        {
            Application.EnableVisualStyles();
            Application.Run(new ErrorInfoForm(errorHead, errorIntroduction, ex));
        }

        /// <summary>
        /// 显示错误信息对话框
        /// </summary>
        public static void ShowErrorInfoDialog(string errorHead, string errorIntroduction, Exception ex)
        {
            new ErrorInfoForm(errorHead, errorIntroduction, ex).ShowDialog();
        }

        /// <summary>
        /// 创建错误窗体实例
        /// </summary>
        internal ErrorInfoForm(string errorHead, string errorIntroduction, Exception ex)
        {
            InitializeComponent();

            this.Text = errorHead;
            label_errorHead.Text = errorHead;
            textBox_errorIntro.Text = errorIntroduction;
            textBox_message.Text = ex.Message;
            textBox_errorCode.Text = ex.HResult.ToString();
            textBox_source.Text = ex.Source;
            textBox_stackTrace.Text = GetExceptionInfo(ex);
        }

        private static string GetExceptionInfo(Exception ex)
        {
            if (null == ex.InnerException)
            {
                return ex.StackTrace;
            }
            return ex.StackTrace + Environment.NewLine + GetInnerExceptionInfo(ex.InnerException);
        }

        private static string GetInnerExceptionInfo(Exception ex)
        {
            StringBuilder errorInfo = new StringBuilder(2000);
            errorInfo.Append(ex.Message)
                .Append(Environment.NewLine)
                .Append("ErrorCode:")
                .Append(ex.HResult)
                .Append(Environment.NewLine)
                .Append(ex.StackTrace);
            if (null != ex.InnerException)
            {
                errorInfo.Append(Environment.NewLine).Append(GetInnerExceptionInfo(ex.InnerException));
            }
            return errorInfo.ToString();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }
            StringBuilder info = new StringBuilder(1000);
            try
            {
                info.Append("Head:").Append(label_errorHead.Text).Append(Environment.NewLine);
                info.Append("HeadIntrodcution:").Append(textBox_errorIntro.Text).Append(Environment.NewLine);
                info.Append("Message:").Append(textBox_message.Text).Append(Environment.NewLine);
                info.Append("ErrorCode:").Append(textBox_errorCode.Text).Append(Environment.NewLine);
                info.Append("Source:").Append(textBox_source.Text).Append(Environment.NewLine);
                info.Append("StackTrace:").Append(textBox_stackTrace.Text).Append(Environment.NewLine);
                File.WriteAllText(saveFileDialog.FileName, info.ToString());
                info.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
