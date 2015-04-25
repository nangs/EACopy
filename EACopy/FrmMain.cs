using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using EACopy.TransferObject;

namespace EACopy
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            this.btnCopy.Enabled = false;
            string fileName = string.Empty;
            CopyResponse response = new CopyResponse();

            if (tbFilename.Text.Trim() != string.Empty)
                fileName = tbFilename.Text;

            List<string> destFolders = new List<string>();

            TargetFiles target = new TargetFiles();
            target.FileName = fileName;
            target.Load();
            destFolders = target.TargetList;
            response = new EACopyFactory(destFolders).Start();

            if (response.Success)
            {
                lblStatus.Text = "Done !";
                btnLoad.Enabled = true;
                btnCopy.Enabled = false;
                this.tbFilename.Text = string.Empty;
            }
            else
            {
                lblStatus.Text = response.Message;
                btnLoad.Enabled = true;
                btnCopy.Enabled = false;
                this.tbFilename.Text = string.Empty;
            }
        }

        private void ShowLoadFileDialog()
        {
            openFileDialog.InitialDirectory = Constants.Dropbox;
            openFileDialog.Title = "Browse Files";
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Text Files|*.txt";
            openFileDialog.FileName = string.Empty;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //if (Path.GetExtension(openFileDialog.FileName).Equals("txt",
                //                         StringComparison.InvariantCultureIgnoreCase))
                this.tbFilename.Text = openFileDialog.FileName; ;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            ShowLoadFileDialog();
            btnCopy.Enabled = true;
            btnLoad.Enabled = false;
        }
    }
}
