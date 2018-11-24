using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyBuddyApp
{
    class quiz
    {
        String name;
        String itemID;

        public quiz(String name, String itemID)
        {
            this.name = name;
            this.itemID = itemID;
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
