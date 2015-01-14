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
        /// <summary>
        /// Méthode qui se fait passer pour une Factory. Comme ça l'air de rien.
        /// Permet de créer un parser sans se soucier du fournisseur de tags (TagSharpParser ou un autre)
        /// </summary>
        /// <param name="path">Nom du fichier à parser</param>
        /// <returns></returns>
        public static TagParser Create(String path)
        {
            return new TagSharpParser(path);
        }

        /// <summary>
        /// Renvoie la liste des tags gérés par la librairie utilisée
        /// </summary>
        /// <returns></returns>
        public static List<TagItem> GetAvailableTags()
        {
            return TagSharpParser.AVAILABLE_TAGS;
        }

        // ici on pourrait imaginer une autre méthode Create avec un autre fournisseur également
        //public static TagParser CreateUsingXXXTagProvider(String path)
        //{
        //    return new XXXTagParser(path);
        //}


        /// <summary>
        /// Méthode qui permet d'enlever les caractères invalides qui pourrait provenir d'un tag
        /// Exemple: "AC/DC" est un nom invalide à cause du caractère '/'. Cette fonction va le remplacer par un équivalent valide
        /// </summary>
        /// <param name="raw">nom du fichier avec éventuellement des caractères invalides</param>
        /// <returns></returns>
        public static string cleanFilename(string raw)
        {
            string cleaned = raw;
            foreach (var item in INVALID_FILENAME_CHAR)
            {
                cleaned = cleaned.Replace(item.Key, item.Value);
            }
            return cleaned;
        }

        // liste des caractères invalides: http://msdn.microsoft.com/en-us/library/windows/desktop/aa365247%28v=vs.85%29.aspx#paths
        protected static Dictionary<String, String> INVALID_FILENAME_CHAR = new Dictionary<string, string>()
        {
            {"<",""},
            {">",""},
            {":",""},
            {"\"","''"}, 
            {"/","_"},
            //{"\\","_"}, // celui-ci on l'enlève si l'utilisateur veut pouvoir faire une arborescence. Exemple: <artist>\<album>
            {"?",""},
            {"*",""}
            
        };
    }
}
