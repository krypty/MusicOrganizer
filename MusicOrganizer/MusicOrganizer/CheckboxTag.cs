using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrganizer
{
    public class CheckboxTag
    {
        public CheckboxTag(String tag, bool isChecked)
        {
            this.tag = tag;
            this.isChecked = isChecked;
        }


        private String tag;

        public String Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }


        public override string ToString()
        {
            return tag;
        }


    }
}
