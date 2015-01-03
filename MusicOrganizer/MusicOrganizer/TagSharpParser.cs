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
            get { return this.tagFile.Tag.Title; }
        }

        public override string Track
        {
            //format: 00
            get { return this.tagFile.Tag.Track.ToString("00"); }
        }

        public override string Artist
        {
            get { return this.tagFile.Tag.FirstPerformer; }
        }

        public override string Album
        {
            get { return this.tagFile.Tag.Album; }
        }
        public override string Year
        {
            // format: yyyy
            get { return this.tagFile.Tag.Year.ToString(); }
        }

        public override string Genre
        {
            get { return this.tagFile.Tag.FirstGenre; }
        }

        public override string DiscNumber
        {
            get { return this.tagFile.Tag.Disc.ToString(); }
        }
        #endregion

        public override string Parse(string tagFolderFormat, string tagFileFormat, string destFolder)
        {
            string extension = System.IO.Path.GetExtension(this.filename);
            destFolder = String.IsNullOrWhiteSpace(destFolder) ? "" : destFolder + @"\";
            tagFolderFormat = String.IsNullOrWhiteSpace(tagFolderFormat) ? "" : tagFolderFormat + @"\";

            string parsedFilename = destFolder + tagFolderFormat + tagFileFormat + extension;
            Console.WriteLine("parsedFilename in: " + parsedFilename);

            foreach (KeyValuePair<string, Func<string>> entry in dictTagNameToTagValue)
            {
                parsedFilename = parsedFilename.Replace(entry.Key, entry.Value());
            }

            Console.WriteLine("parsedFilename out: " + parsedFilename);
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
    }
}
