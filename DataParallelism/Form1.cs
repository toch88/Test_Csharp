using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace DataParallelism
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btnProcessImages_Click(object sender, EventArgs e)
        {
            // Start a new "task" to process the files.
            Task.Factory.StartNew(() =>
            {
                ProcessFiles();
            });
        }
        private void ProcessFiles()
        {
            // Load up all *.jpg files, and make a new folder for the modified data.
            string[] files = Directory.GetFiles(@"C:\Users\Public\Pictures\Sample Pictures", "*.jpg", SearchOption.AllDirectories);
            string newDir = @"C:\ModifiedPictures";
            Directory.CreateDirectory(newDir);
            // Process the image data in a blocking manner.
            foreach (string currentFile in files)
            {
                string filename = Path.GetFileName(currentFile);
                using (Bitmap bitmap = new Bitmap(currentFile))
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    bitmap.Save(Path.Combine(newDir, filename));
                    // Print out the ID of the thread processing the current image.
                    // in a thread-safe manner.
                    this.Invoke((Action)delegate
                    {
                        this.TextBox.Text += string.Format("Processing {0} on thread {1} \r\n", filename, Thread.CurrentThread.ManagedThreadId);
                    }
                    );
                }
            }
        }
       
    }
}
