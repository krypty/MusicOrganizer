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
        private static FolderItem dummy = new FolderItem("*?-Dummy*?-", false, null); // le nom choisi est suffisament improbable (et surtout invalide --> *?) 
        public static Func<string, bool> FilesWithWantedExtensionPredicate = (file) => file.ToLower().EndsWith("mp3"); // j'utilise cette manière de faire car il sera facile d'ajouter d'autres extensions

        private bool? isChecked = false;
        private bool isFolder = true;

        private FolderItem _parent = null;
        private string folderLabel = "[BAD_FOLDER_NAME]";
        private string path = "[BAD_PATH]";

        // booléen qui vaut vrai si on a jamais déroulé (expand) un dossier. Permet de tester si on doit peupler ou pas ce dossier (treeviewitem) avec ses enfants
        private bool mustBeExpanded = true;
        private bool isExpanded = false;

        // Quand on déroule un item on va chercher les fichiers et dossiers qu'il contient et peupler le modèle (l'enfant déroulé)
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (mustBeExpanded)
                {
                    // on supprime le dossier Dummy
                    childFolderItem.RemoveAt(0);

                    foreach (string subFolderName in SafeWalk.GetDirectories(path))
                    {
                        FolderItem subFolderItem = new FolderItem(subFolderName, false, this);
                        this.ChildFolderItem.Add(subFolderItem);
                    }

                    foreach (string fileName in SafeWalk.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(FilesWithWantedExtensionPredicate).ToList())
                    {
                        bool isFolder = false;
                        FolderItem subFolderItem = new FolderItem(fileName, false, this, isFolder);
                        this.ChildFolderItem.Add(subFolderItem);
                    }

                    mustBeExpanded = false;
                }
                isExpanded = value;
            }
        }

        public string FolderLabel
        {
            get { return folderLabel; }
            set { folderLabel = value; }
        }

        public bool? IsChecked
        {
            get { return isChecked; }
            set { SetIsChecked(value, true, true); }
        }
        public string Path { get { return path; } set { path = value; } }

        private ObservableCollection<FolderItem> childFolderItem;
        public ObservableCollection<FolderItem> ChildFolderItem
        {
            get { return childFolderItem; }
            set { childFolderItem = value; }
        }

        public bool IsFolder
        {
            get { return isFolder; }
        }

        public FolderItem(string path, bool isChecked, FolderItem parent, bool isFolder = true)
        {
            this.path = path;
            this.isChecked = isChecked;
            this._parent = parent;
            this.isFolder = isFolder;

            this.folderLabel = path.Substring(path.LastIndexOf(@"\") + 1);
            this.childFolderItem = new ObservableCollection<FolderItem>();

            if (isFolder)
            {
                this.ChildFolderItem.Add(FolderItem.dummy);
            }
            else
            {
                this.mustBeExpanded = false;
            }
        }

        //source: http://www.thebestcsharpprogrammerintheworld.com/blogs/Treeview-with-checkbox-in-WPF-using-csharp.aspx
        // j'ai adapté l'exemple pour utiliser une arborescence de fichiers/dossiers.
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) return;

            isChecked = value;

            if (updateChildren && isChecked.HasValue)
            {
                foreach (FolderItem child in this.childFolderItem)
                {

                    if (child != null)
                    {
                        child.SetIsChecked(isChecked, true, false);
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
                    // si le Dummy est encore présent, on l'ignore
                    if (subItem == null || subItem.path.Equals(dummy.path)) continue;

                    // on ajoute les fichiers racines cochés
                    if (!subItem.isFolder && subItem.isChecked.HasValue && subItem.isChecked.Value == true)
                    {
                        selectedItem.Add(subItem.path);
                    }

                    // si c'est un dossier qui est coché, on l'ajoute en totalité
                    if (subItem.isFolder && subItem.isChecked.HasValue && subItem.isChecked.Value == true)
                    {

                        foreach (string fileName in SafeWalk.EnumerateFiles(subItem.path, "*.*", SearchOption.AllDirectories).Where(FilesWithWantedExtensionPredicate).ToList())
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
