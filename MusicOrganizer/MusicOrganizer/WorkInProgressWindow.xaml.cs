using MusicOrganizer.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicOrganizer
{
    /// <summary>
    /// Interaction logic for WorkInProgressWindow.xaml
    /// </summary>
    public partial class WorkInProgressWindow : Window
    {
        private TagWorker worker;
        private FolderScanner folderScanner;
        private string tagFolderFormat;
        private string tagFileFormat;
        private string destFolder;

        public WorkInProgressWindow()
        {
            InitializeComponent();
        }

        public WorkInProgressWindow(FolderScanner folderScanner, string tagFolderFormat, string tagFileFormat, string destFolder)
            : this()
        {
            this.folderScanner = folderScanner;
            this.tagFolderFormat = tagFolderFormat;
            this.tagFileFormat = tagFileFormat;
            this.destFolder = destFolder;


            this.worker = new TagWorker(folderScanner, tagFolderFormat, tagFileFormat, destFolder);

            worker.ProgressChanged += worker_ProgressChanged;
            worker.OnCompleteEvent += worker_OnCompleteEvent;
        }

        private void worker_OnCompleteEvent()
        {
            btnCancel.IsEnabled = false;
            btnClose.IsEnabled = true;
        }

        private void worker_ProgressChanged(int percentage)
        {
            this.pbStatus.IsIndeterminate = worker.IsProgressIndeterminate;
            pbStatus.Value = percentage;
        }

        public void ShowAndRun()
        {
            worker.Run();
            this.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.worker.Cancel();
            btnCancel.IsEnabled = false;
        }
    }
}
