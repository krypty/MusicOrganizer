using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicOrganizer
{
    /// <summary>
    /// Classe utilisée pour afficher les tags disponibles dans la ListView/ListBox
    /// </summary>
    class TagItem
    {
        private string tagValue;
        private string tagLabel;

        /// <summary
        /// </summary>
        /// <param name="tagValue">Utilisé en interne %title% par exemple</param>
        /// <param name="tagLabel">Utilisé pour l'affichage Titre par exemple</param>
        public TagItem(string tagValue, string tagLabel)
        {
            this.tagValue = tagValue;
            this.tagLabel = tagLabel;
        }


        public string TagValue
        {
            get { return tagValue; }
            set { tagValue = value; }
        }


        public string TagLabel
        {
            get { return tagLabel; }
            set { tagLabel = value; }
        }


        public override string ToString()
        {
            return this.tagLabel;
        }
    }
}
