using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer.Tools
{
    //source: http://stackoverflow.com/a/5957525
    // je l'ai comprise et modifiée pour qu'elle s'adapte à mes besoins.

    /// <summary>
    /// Classe qui permet de traverser un dossier et des fichiers en ignorant ceux auxquels l'utilisateur n'a pas accès et particulièrement
    /// ce que couvre UnauthorizedAccessException.
    /// </summary>
    public static class SafeWalk
    {
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
            catch (PathTooLongException)
            {
                return Enumerable.Empty<string>();
            }
        }


        //source: http://stackoverflow.com/a/1604187
        private static void AddFiles(string path, IList<string> files)
        {
            try
            {
                Directory.GetDirectories(path).ToList().ForEach(
                    s =>
                    {
                        // lance une exception si on a pas accès. Comportement voulu et connu. C'est un piège pour déclancher le catch et donc ne pas faire de Add
                        Directory.Exists(s);
                        files.Add(s);
                    }
                    );

            }
            catch (UnauthorizedAccessException)
            {
                //rien
            }
        }

        public static IEnumerable<string> GetDirectories(string path)
        {
            List<string> directories = new List<string>();
            AddFiles(path, directories);
            return directories;
        }
    }
}
