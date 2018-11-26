using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyBuddyApp
{
    public class Quizzes
    {
        String name;
        String itemID;
        Chapter parent;

        public Quizzes(String name, String itemID, Chapter parent)
        {
            this.name = name;
            this.itemID = itemID;
            this.parent = parent;
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
        public Chapter getParent()
        {
            return parent;
        }
    }
}
