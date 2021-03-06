﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyBuddyApp
{
    public class Chapter
    {
        private List<section> Sections;
        private List<Quizzes> Quizzes;
        String name;
        String itemID;

        public Chapter(String name, String itemID)
        {
            this.name = name;
            this.itemID = itemID;
            Sections = new List<section>();
            Quizzes = new List<Quizzes>();
        }

        public List<section> GetSectionList()
        {
            return Sections;
        }
        public List<Quizzes> GetQuizList()
        {
            return Quizzes;
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
