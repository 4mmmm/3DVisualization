using System;
using System.Windows.Forms;

namespace _3DVisualization
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text==""&&textBox2.Text=="")
            {
                DialogResult = DialogResult.OK;
                Dispose();
                Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误，请重新输入");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "皮肤文件(*.ssk)|*.ssk";
            ofd.ShowDialog();
            skinEngine1.SkinFile = ofd.FileName;
        }
    }
}
