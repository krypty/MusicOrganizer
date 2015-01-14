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
    /// <summary>
    /// Modèle arborescent utilisé dans un treeview pour représenter les fichiers musicaux d'un répertoire
    /// Ce modèle a une propriété IsChecked qui permet de savoir si un fichier musical ou un dossier a été sélectionné
    /// Il faut donc utiliser un TreeView (ou similaire) avec une case à cocher par item
    /// </summary>
    public class FolderScanner
    {
        private ObservableCollection<FolderItem> rootFolders;
        private string rootFolderName;

        /// <summary>
        /// Crée un modèle arborescent et retient les fichiers musicaux (uniquement .mp3 pour l'instant)
        /// </summary>
        /// <param name="rootFolder">Dossier racine de l'arborescence à manipuler (dossier musical de préférence)</param>
        public FolderScanner(string rootFolder)
        {
            this.rootFolders = new ObservableCollection<FolderItem>();
            this.rootFolderName = rootFolder;

            rootFolders.Add(new FolderItem(this.rootFolderName, false, null));
        }


        /// <summary>
        /// Parcourt l'arbre à la recherche des fichiers musicaux cochés par l'utilisateur
        /// </summary>
        /// <returns>Collection de chemin vers les fichiers musicaux sélectionnés</returns>
        public ICollection<string> GetSelectedItems()
        {
            // On utilise un hashset car on souhaite éviter les chemins des fichiers musicaux en double
            HashSet<string> selectedItems = new HashSet<string>();
            FolderItem root = this.rootFolders[0];
            if (root.IsChecked.HasValue && root.IsChecked.Value == true)
            {
                foreach (string fileName in SafeWalk.EnumerateFiles(this.rootFolderName, "*.*", SearchOption.AllDirectories).Where(FolderItem.FilesWithWantedExtensionPredicate).ToList())
                {
                    selectedItems.Add(fileName);
                }
            }

            // On fusionne les fichiers sélectionnés de la racine avec ceux des enfants. C'est ici que le hashset prend son sens
            selectedItems.UnionWith(FolderItem.GetCheckedItems(rootFolders[0].ChildFolderItem));
            return selectedItems;
        }

        public ObservableCollection<FolderItem> Items { get { return rootFolders; } }
    }
}
