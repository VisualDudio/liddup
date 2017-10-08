﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liddup.Models
{
    class Song
    {
        public Song() : this("", "") { }

        public Song(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"title", Title },
                {"description", Description }
            };

            return dictionary;
        }
    }
}
