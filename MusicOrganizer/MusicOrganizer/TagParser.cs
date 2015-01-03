using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicOrganizer.Tag
{
    abstract class TagParser
    {
        protected string filename;

        public TagParser(String path)
        {
            this.filename = path;
        }

        public abstract string Album { get; }

        public abstract string Title { get; }

        public abstract string Track { get; }

        public abstract string Artist { get; }

        public abstract string Year { get; }

        public abstract string Genre { get; }

        public abstract string DiscNumber { get; }

        public string Path { get { return this.filename; } }

        public abstract string Parse(string tagFolderFormat, string tagFileFormat, string destFolder);
    }
}
