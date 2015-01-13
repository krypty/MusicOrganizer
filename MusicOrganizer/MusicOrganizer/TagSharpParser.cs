using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer.Tag
{
    class TagSharpParser : TagParser
    {
        private TagLib.File tagFile;
        private Dictionary<String, Func<String>> dictTagNameToTagValue;


        public TagSharpParser(String filename)
            : base(filename)
        {
            this.tagFile = TagLib.File.Create(filename);

            this.dictTagNameToTagValue = new Dictionary<string, Func<String>>();
            dictTagNameToTagValue.Add("<title>", () => Title);
            dictTagNameToTagValue.Add("<track>", () => Track);
            dictTagNameToTagValue.Add("<artist>", () => Artist);
            dictTagNameToTagValue.Add("<album>", () => Album);
            dictTagNameToTagValue.Add("<year>", () => Year);
            dictTagNameToTagValue.Add("<genre>", () => Genre);
            dictTagNameToTagValue.Add("<disc_number>", () => DiscNumber);
        }

        #region Properties
        public override string Title
        {
            get { return getValueOrFallback(this.tagFile.Tag.Title, "title"); }
        }

        public override string Track
        {
            //format: 00
            get { return getValueOrFallback(this.tagFile.Tag.Track.ToString("00"), "track"); }
        }

        public override string Artist
        {
            //TODO: si first performer est vide regarder si on peut trouver une alternative
            get { return getValueOrFallback(this.tagFile.Tag.FirstPerformer, "artist"); }
        }

        public override string Album
        {
            get { return getValueOrFallback(this.tagFile.Tag.Album, "album"); }
        }
        public override string Year
        {
            // format: yyyy
            get { return getValueOrFallback((int)this.tagFile.Tag.Year, "year"); }
        }

        public override string Genre
        {
            get { return getValueOrFallback(this.tagFile.Tag.FirstGenre, "genre"); }
        }

        public override string DiscNumber
        {
            get { return getValueOrFallback(this.tagFile.Tag.Disc.ToString(), "disc number"); }
        }
        #endregion

        public override string Parse(string tagFolderFormat, string tagFileFormat, string destFolder)
        {
            //TODO: replace invalid char in filename and foldername
            //TODO: renvoie "Unknown XXX ou XXX vaut Artist, Album... 
            string extension = System.IO.Path.GetExtension(this.filename);
            destFolder = String.IsNullOrWhiteSpace(destFolder) ? "" : destFolder + @"\";
            tagFolderFormat = String.IsNullOrWhiteSpace(tagFolderFormat) ? "" : tagFolderFormat + @"\";

            string parsedFilename = tagFolderFormat + tagFileFormat + extension;
            //Console.WriteLine("parsedFilename in: " + parsedFilename);

            foreach (KeyValuePair<string, Func<string>> entry in dictTagNameToTagValue)
            {
                parsedFilename = parsedFilename.Replace(entry.Key, entry.Value());
            }

            parsedFilename = destFolder + TagParserTools.cleanFilename(parsedFilename);

            //Console.WriteLine("parsedFilename out: " + parsedFilename);
            return parsedFilename;
        }

        private static void populateAvailableTags()
        {
            availableTags.Add(new TagItem("<title>", "Titre"));
            availableTags.Add(new TagItem("<track>", "Piste"));
            availableTags.Add(new TagItem("<artist>", "Artiste"));
            availableTags.Add(new TagItem("<album>", "Album"));
            availableTags.Add(new TagItem("<year>", "Année de sortie"));
            availableTags.Add(new TagItem("<genre>", "Genre"));
            availableTags.Add(new TagItem("<disc_number>", "Numéro du disque"));
        }

        private static List<TagItem> availableTags;
        public static List<TagItem> AVAILABLE_TAGS
        {
            get
            {
                if (availableTags == null)
                {
                    availableTags = new List<TagItem>();
                    populateAvailableTags();
                }

                return availableTags;
            }
        }

        private string getValueOrFallback(string value, string label)
        {
            return String.IsNullOrWhiteSpace(value) ? "[UNKNOWN " + label.ToUpper() + "]" : value;
        }

        private string getValueOrFallback(int value, string label)
        {
            return value == 0 || value == -1 ? "[UNKNOWN " + label.ToUpper() + "]" : value.ToString();
        }
    }
}
