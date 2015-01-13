using MusicOrganizer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer
{
    public class FolderScanner
    {
        private ObservableCollection<FolderItem> rootFolders;
        private string rootFolderName;

        public FolderScanner(string rootFolder)
        {
            this.rootFolders = new ObservableCollection<FolderItem>();
            this.rootFolderName = rootFolder;

            rootFolders.Add(new FolderItem(this.rootFolderName, false, null));
        }

        public ICollection<string> GetSelectedItems()
        {
            //TODO: thread
            HashSet<string> selectedItems = new HashSet<string>();
            FolderItem root = this.rootFolders[0];
            if (root.IsChecked.HasValue && root.IsChecked.Value == true)
            {
                foreach (string fileName in SafeWalk.EnumerateFiles(this.rootFolderName, "*.*", SearchOption.AllDirectories).Where(FolderItem.FilesWithWantedExtensionPredicate).ToList())
                {
                    selectedItems.Add(fileName);
                }
            }

            selectedItems.UnionWith(FolderItem.GetCheckedItems(rootFolders[0].ChildFolderItem));
            return selectedItems;
        }

        public ObservableCollection<FolderItem> Items { get { return rootFolders; } }
    }
}
