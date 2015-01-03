using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MusicOrganizer
{
    class FolderScanner
    {
        private DirectoryInfo directoryInfo;
        private TreeView treeView;

        //List<TreeViewModel> treeViewModel = new List<TreeViewModel>();
        AsyncObservableCollection<TreeViewModel> treeViewModel = new AsyncObservableCollection<TreeViewModel>();

        public FolderScanner(String folder, TreeView treeView)
        {
            this.directoryInfo = new DirectoryInfo(folder);
            //this.treeView = treeView;

            // on attache les données à la vue, pour l'instant les données sont vides (List vide)
            //treeView.ItemsSource = treeViewModel;
        }

        /// <summary>
        /// Est appelé pour remplir le modèle. C'est un thread qui va peupler le modèle en parcourant récursivement les dossiers
        /// </summary>
        public void doWork()
        {
            /*  
              Thread t = new Thread(new ThreadStart(() =>
              {*/
            if (directoryInfo.Exists)
            {
                TreeViewModel tv = new TreeViewModel(directoryInfo.Name);
                treeViewModel.Add(tv);

                tv.Children.Add(CreateDirectoryNode(directoryInfo));

                // c'est ici que ca plante.
                // comme on est dans un tread, on ne peut pas changer l'UI directement. On demande donc au thread gérant l'UI de rafraichir la treeview pour nous
                //treeView.Dispatcher.BeginInvoke((Action)(() => treeView.Items.Refresh

                Console.WriteLine("fini !");
            }
            else
            {
                throw new InvalidOperationException("Folder does not exist");
            }
            /*}));

            t.Start();*/
        }

        private TreeViewModel CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewModel(directoryInfo.Name);

            try
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directoryNode.Children.Add(CreateDirectoryNode(directory));
                    Console.WriteLine(directoryNode.Children[0].Name);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                //nothing
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                directoryNode.Children.Add(new TreeViewModel(file.Name));
            }

            //meme en essayant de rafraichir la treeview en plusieurs fois ca plante
            //treeView.Dispatcher.BeginInvoke((Action)(() => treeView.Items.Refresh()));


            return directoryNode;

        }

        // chargement si expand
        public void Populate(string header, string tag, TreeView _root, TreeViewItem _child, bool isfile)
        {
            TreeViewItem _driitem = new TreeViewItem();
            _driitem.Tag = new CheckboxTag(tag, false);
            _driitem.Header = header;

            _driitem.Expanded += new RoutedEventHandler(_driitem_Expanded);
            if (!isfile)
                _driitem.Items.Add(new TreeViewItem());

            if (_root != null)
            { _root.Items.Add(_driitem); }
            else { _child.Items.Add(_driitem); }
        }

        void _driitem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem _item = (TreeViewItem)sender;

            //TreeViewModel root = (TreeViewModel)_item.Items[0];
            //root.Children.Add(new TreeViewModel("tonton"));



            if (_item.Items.Count == 1 && ((TreeViewItem)_item.Items[0]).Header == null)
            {
                _item.Items.Clear();
                foreach (string dir in Directory.GetDirectories(_item.Tag.ToString()))
                {
                    DirectoryInfo _dirinfo = new DirectoryInfo(dir);
                    Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, false);
                }

                foreach (string dir in Directory.GetFiles(_item.Tag.ToString()))
                {
                    FileInfo _dirinfo = new FileInfo(dir);
                    Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, true);
                }

            }
        }
    }

}
