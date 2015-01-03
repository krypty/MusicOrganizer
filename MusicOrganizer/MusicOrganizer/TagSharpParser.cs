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


        public TagSharpParser(String path)
            : base(path)
        {
            this.tagFile = TagLib.File.Create(path);
        }


        public override string Title
        {
            get { return this.tagFile.Tag.Title; }
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

        private static void populateAvailableTags()
        {
            availableTags.Add(new TagItem("<title>", "Titre"));
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
