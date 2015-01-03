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
        public ObservableCollection<TreeViewModel> tata = TreeViewModel.SetTree("Top Level");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSourceFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            //TagItem tagItem = new TagItem("alal", 5);
            //lbxTags.Items.Add(tagItem);

            ////String path = @"C:\Users\Gary\Dropbox\HE_Arc\3emeAnnee\CSharp\projetSolo\";
            //String path = @"C:\Android\SDK";
            //FolderScanner fs = new FolderScanner(path, treeView1);

            ////treeView1.ItemsSource = tata;
            ////fs.doWork();

            //fs.Populate("Test dossier", path, treeView1, null, false);

            //this.lblInfo.Content = "Running...";

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            String path = dialog.SelectedPath;

            if (path != "")
            {
                List<String> mp3Files = getMP3Files(path);

                mp3Files.ForEach(f => lbxFiles.Items.Add(f));

                Console.WriteLine(mp3Files.Count);
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

        private void lbxTags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lbx = (ListBox)sender;
            TagItem tagItem = (TagItem)lbx.SelectedItem;

            Console.WriteLine(tagItem.TagValue);
            tbxFolderFormat.Text = tbxFolderFormat.Text + tagItem.TagValue;
        }

        private void lbxTags_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lbx = (ListBox)sender;
                TagItem tagItem = (TagItem)lbx.SelectedItem;

                DataObject dataObj = new DataObject();
                dataObj.SetData(typeof(TagItem), tagItem);
                DragDropEffects effects = DragDrop.DoDragDrop(this.lbxTags, dataObj, DragDropEffects.Copy);

                Console.WriteLine(tagItem.TagValue);
            }
        }

        private void previewDragEnterTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
                Console.WriteLine("dragOver copy");
            }

            else
            {
                e.Effects = DragDropEffects.None;
                Console.WriteLine("dragOver none");
            }
            e.Handled = true;
        }

        private void dropTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
                TagItem tagItem = (TagItem)e.Data.GetData(typeof(TagItem));

                TextBox tbx = (TextBox)sender;
                tbx.Text = tbx.Text + tagItem.TagValue;
                Console.WriteLine("dragDrop copy");
            }

            else
            {
                e.Effects = DragDropEffects.None;
                Console.WriteLine("dragDrop none");
            }
        }
    }
}
