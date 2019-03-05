using System;
using System.Windows.Forms;
using System.Linq;

namespace MNK_Game
{
    public partial class Form1 : Form
    {
        private MNK mMnk;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            textBoxRow.Select();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxRow.Text, out int row) 
                && int.TryParse(textBoxColumn.Text, out int column)
                && int.TryParse(textBoxNumberOfMarks.Text, out int numMarks))
            {
                if(mMnk != null)
                {
                    foreach (Panel p in this.Controls.OfType<Panel>())
                        this.Controls.Remove(p);
                }

                mMnk = new MNK(this, row, column, numMarks);
                mMnk.MakeGrid();
            }
            else
                MessageBox.Show("All input must be integer!");
        }
    }
}