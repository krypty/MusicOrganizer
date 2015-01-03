using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer.Tag
{
    /// <summary>
    /// Permet d'abstraire le fournisseur des tags. On peut changer simplement de fournisseur de tag grâce à cette encapsulation
    /// </summary>
    class TagParserTools
    {
        public static TagParser Create(String path)
        {
            return new TagSharpParser(path);
        }

        public static List<TagItem> GetAvailableTags()
        {
            return TagSharpParser.AVAILABLE_TAGS;
        }

        // ici on pourrait imaginer une autre méthode Create avec un autre fournisseur également
        //public static TagParser CreateUsingXXXTagProvider(String path)
        //{
        //    return new XXXTagParser(path);
        //}
    }
}
