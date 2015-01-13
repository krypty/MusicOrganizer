using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicOrganizer.Tag;
using System.Windows;

namespace MusicOrganizer
{
    class TagWorker
    {
        private const bool MUST_OVERWRITE = false;

        private int progressCopy = 0;

        private int ProgressInPercent { get { return this.progressCopy; } }

        public bool IsProgressIndeterminate { get; set; }

        private FolderScanner folderScanner;
        private string tagFolderFormat;
        private string tagFileFormat;
        private string destFolder;

        private BackgroundWorker worker;


        // Events du backgroundworker qu'on rend accessible à l'exterieur mais en encapsulant l'objet worker
        public delegate void PercentageChangedEventHandler(int percentage);
        public event PercentageChangedEventHandler ProgressChanged;


        public delegate void OnCompleteEventHandler();
        public event OnCompleteEventHandler OnCompleteEvent;

        public TagWorker(FolderScanner folderScanner, string tagFolderFormat, string tagFileFormat, string destFolder)
        {
            this.folderScanner = folderScanner;
            this.tagFolderFormat = tagFolderFormat;
            this.tagFileFormat = tagFileFormat;
            this.destFolder = destFolder;

            this.worker = new BackgroundWorker();
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += worker_DoWork;
            this.worker.ProgressChanged += worker_ProgressChanged;
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Erreur: " + e.Error.ToString(), "Une erreur s'est produite", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.OnCompleteEvent();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressChanged(e.ProgressPercentage);
        }

        public void Run()
        {
            this.worker.RunWorkerAsync();
        }

        public void Cancel()
        {
            this.worker.CancelAsync();
        }

        private Dictionary<string, string> CreateDictFromTags()
        {
            this.IsProgressIndeterminate = true;
            this.worker.ReportProgress(0);

            Dictionary<String, String> dicFileToProcess = new Dictionary<string, string>();
            foreach (var originalFilename in folderScanner.GetSelectedItems())
            {
                TagParser parser = TagParserTools.Create(originalFilename);
                String parsedFilename = parser.Parse(tagFolderFormat, tagFileFormat, destFolder);

                dicFileToProcess.Add(originalFilename, parsedFilename);
            }
            return dicFileToProcess;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, string> dicFileToProcess = CreateDictFromTags();

            this.IsProgressIndeterminate = false;
            this.worker.ReportProgress(0);

            int itemProcessed = 0;
            this.progressCopy = 0;

            foreach (KeyValuePair<string, string> entry in dicFileToProcess)
            {
                if(worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                String srcFilename = entry.Key;
                String destFilename = entry.Value;

                String destFolder = Path.GetDirectoryName(destFilename);

                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }

                if (!System.IO.File.Exists(destFilename) || MUST_OVERWRITE)
                {
                    try
                    {
                        System.IO.File.Copy(srcFilename, destFilename, true);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erreur lors de la copie !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                itemProcessed++;
                this.progressCopy = (int)(100 * (itemProcessed / (float)dicFileToProcess.Count));
                this.worker.ReportProgress(ProgressInPercent);

            }
        }
    }
}
