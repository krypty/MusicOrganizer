using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


        public static string cleanFilename(string raw)
        {
            string cleaned = raw;
            foreach (var item in INVALID_FILENAME_CHAR)
            {
                cleaned = cleaned.Replace(item.Key, item.Value);
            }
            return cleaned;
        }

        protected static Dictionary<String, String> INVALID_FILENAME_CHAR = new Dictionary<string, string>()
        {
            {"<",""},
            {">",""},
            {":",""},
            {"\"","''"}, 
            {"/","_"},
            //{"\\","_"}, // celui-ci on l'enlève si l'utilisateur veut pouvoir faire une arborescence.
            {"?",""},
            {"*",""}
            
        };
    }
}
