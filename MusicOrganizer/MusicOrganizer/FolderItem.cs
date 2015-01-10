using MusicOrganizer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer
{
    public class FolderItem : INotifyPropertyChanged
    {
        // dummy créé pour pouvoir déplier les dossiers. On les peuple seulement quand ils sont expanded pour la première fois
        private static FolderItem dummy = new FolderItem("*?-Dummy*?-", false, null);
        public static Func<string, bool> FilesWithWantedExtensionPredicate = (file) => file.ToLower().EndsWith("mp3"); //|| file.ToLower().EndsWith("cfg");

        private bool? _isChecked = false;
        private bool _isFolder = true;
        private FolderItem _parent = null;
        private string _folderLabel = "[BAD_FOLDER_NAME]";
        private string _path = "[BAD_PATH]";

        private bool mustBeExpanded = true;
        private bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (mustBeExpanded)
                {
                    // remove dummy
                    childFolderItem.RemoveAt(0);

                    foreach (string subFolderName in SafeWalk.GetDirectories(_path))
                    {
                        FolderItem subFolderItem = new FolderItem(subFolderName, false, this);
                        this.ChildFolderItem.Add(subFolderItem);
                    }

                    foreach (string fileName in SafeWalk.EnumerateFiles(_path, "*.*", SearchOption.TopDirectoryOnly).Where(FilesWithWantedExtensionPredicate).ToList())
                    {
                        bool isFolder = false;
                        FolderItem subFolderItem = new FolderItem(fileName, false, this, isFolder);
                        this.ChildFolderItem.Add(subFolderItem);
                    }

                    mustBeExpanded = false;
                }
                _isExpanded = value;
            }
        }

        public string FolderLabel
        {
            get { return _folderLabel; }
            set { _folderLabel = value; }
        }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }
        public string Path { get { return _path; } set { _path = value; } }

        private ObservableCollection<FolderItem> childFolderItem;
        public ObservableCollection<FolderItem> ChildFolderItem
        {
            get { return childFolderItem; }
            set { childFolderItem = value; }
        }

        public FolderItem(string path, bool isChecked, FolderItem parent, bool isFolder = true)
        {
            this._path = path;
            this._isChecked = isChecked;
            this._parent = parent;
            this._isFolder = isFolder;

            this._folderLabel = path.Substring(path.LastIndexOf(@"\") + 1);
            this.childFolderItem = new ObservableCollection<FolderItem>();

            if (_isFolder)
            {
                this.ChildFolderItem.Add(FolderItem.dummy);
            }
            else
            {
                this.mustBeExpanded = false;
            }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                foreach (FolderItem child in this.childFolderItem)
                {

                    if (child != null)
                    {
                        child.SetIsChecked(_isChecked, true, false);
                    }
                }
            }

            if (updateParent && _parent != null)
            {
                FolderItem p = _parent as FolderItem;
                p.VerifyCheckedState();
            }

            NotifyPropertyChanged("IsChecked");
        }

        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < childFolderItem.Count; ++i)
            {
                FolderItem f = childFolderItem[i] as FolderItem;
                bool? current = f.IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ICollection<string> GetCheckedItems(ObservableCollection<FolderItem> observableCollection)
        {
            // ici on prend un set afin d'éviter les doublons
            HashSet<String> selectedItem = new HashSet<String>();

            if (observableCollection != null)
            {
                foreach (FolderItem subItem in observableCollection)
                {
                    if (subItem == null || subItem._path.Equals(dummy._path)) continue;

                    // on ajoute les fichiers racines cochés
                    if (!subItem._isFolder && subItem._isChecked.HasValue && subItem._isChecked.Value == true)
                    {
                        selectedItem.Add(subItem._path);
                    }

                    // si c'est un dossier qui est coché, on l'ajoute en totalité
                    if (subItem._isFolder && subItem._isChecked.HasValue && subItem._isChecked.Value == true)
                    {

                        foreach (string fileName in SafeWalk.EnumerateFiles(subItem._path, "*.*", SearchOption.AllDirectories).Where(FilesWithWantedExtensionPredicate).ToList())
                        {
                            selectedItem.Add(fileName);
                        }
                    }

                    selectedItem.UnionWith(GetCheckedItems(subItem.ChildFolderItem));
                }
            }
            return selectedItem;
        }
    }
}
