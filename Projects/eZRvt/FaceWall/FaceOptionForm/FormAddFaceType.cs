using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eZRvt.FaceWall
{
    public partial class FormAddFaceType : Form
    {
        public string FaceType;

        public FormAddFaceType()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            FaceType = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void FormAddFaceType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
