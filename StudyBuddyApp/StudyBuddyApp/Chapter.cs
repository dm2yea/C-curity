using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyBuddyApp
{
    public class Chapter
    {
        private List<section> Sections;
        String name;
        String itemID;

        public Chapter(String name, String itemID)
        {
            this.name = name;
            this.itemID = itemID;
            Sections = new List<section>();
        }

        public List<section> GetSectionList()
        {
            return Sections;
        }
        public void setName(String name)
        {
            this.name = name;
        }
        public String getName()
        {
            return name;
        }
        public String getItemID()
        {
            return itemID;
        }

    }
}
