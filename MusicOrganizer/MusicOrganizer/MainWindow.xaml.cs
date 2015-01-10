using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using MusicOrganizer.Tag;

namespace MusicOrganizer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection<TreeViewModel> tata = TreeViewModel.SetTree("Top Level");
        FolderScanner folderScanner;
        public MainWindow()
        {
            InitializeComponent();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            folderScanner = new FolderScanner(path);
            treeViewFolders.DataContext = folderScanner.Items;

            lblSourceFolder.Content = path;
        }

        private void btnSourceFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            String path = dialog.SelectedPath;

            if (path != "")
            {
                if (path == lblDestFolder.Content.ToString())
                {
                    MessageBox.Show("Veuillez sélectionner un dossier de destination différent que le dossier source.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    lblSourceFolder.Content = path;

                    folderScanner = new FolderScanner(path);
                    treeViewFolders.DataContext = folderScanner.Items;
                }
            }


        }

        private List<String> getMP3Files(string path)
        {
            List<String> mp3Files = new List<string>();

            foreach (String file in Directory.GetFiles(path, "*.mp3"))
            {
                Console.WriteLine("f: " + file);
                mp3Files.Add(file);
            }

            return mp3Files;
        }

        private void btnDestFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            String path = dialog.SelectedPath;

            if (path != "")
            {
                if (path == lblSourceFolder.Content.ToString())
                {
                    MessageBox.Show("Veuillez sélectionner un dossier de destination différent que le dossier source.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    lblDestFolder.Content = path;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadAvailableTags();
        }

        private void loadAvailableTags()
        {
            List<TagItem> availableTags = TagParserTools.GetAvailableTags();
            availableTags.ForEach(t => lbxTags.Items.Add(t));
        }

        private void lbxTags_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lbx = (ListBox)sender;

                if (lbx.SelectedItem == null)
                {
                    return;
                }
                TagItem tagItem = (TagItem)lbx.SelectedItem;

                DataObject dataObj = new DataObject();
                dataObj.SetData(typeof(TagItem), tagItem);
                DragDropEffects effects = DragDrop.DoDragDrop(this.lbxTags, dataObj, DragDropEffects.Copy);
            }
        }

        private void previewDragEnterTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
            }

            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void dropTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
                TagItem tagItem = (TagItem)e.Data.GetData(typeof(TagItem));

                ComboBox tbx = (ComboBox)sender;
                tbx.Text = tbx.Text + tagItem.TagValue;
            }

            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void updateFilePreview()
        {
            bool isDestFolderValid = Directory.Exists(lblDestFolder.Content.ToString());
            bool isOneFileSelected = this.treeViewFolders.SelectedItem != null;
            bool isTagFileFormatValid = !String.IsNullOrEmpty(this.tbxFileFormat.Text.ToString());

            if (!(isDestFolderValid && isOneFileSelected && isTagFileFormatValid))
            {
                lblPreviewFile.Content = "Indiquer un dossier de destination, un format de fichier valide et d'avoir sélectionner un fichier";
            }
            else
            {
                FolderItem selectedFolderItem = (FolderItem)this.treeViewFolders.SelectedItem;
                string originalFilename = selectedFolderItem.Path;

                string tagFileFormat = tbxFileFormat.Text.ToString();
                string tagFolderFormat = tbxFolderFormat.Text.ToString();
                string destFolder = lblDestFolder.Content.ToString();

                TagParser parser = TagParserTools.Create(originalFilename);
                string tagPreview = parser.Parse(tagFolderFormat, tagFileFormat, destFolder);

                lblPreviewFile.Content = tagPreview;
            }
        }

        // TODO: fusionner tbxFolderFormat_TextChanged et tbxFileFormat_TextChanged
        private void tbxFolderFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateFilePreview();
        }

        private void tbxFileFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateFilePreview();
        }

        private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            updateFilePreview();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in folderScanner.GetSelectedItems())
            {
                Console.WriteLine("[SELECTTED] " + item);
            }
        }

    }
}
