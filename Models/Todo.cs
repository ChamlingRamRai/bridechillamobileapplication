using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    public class Todo
    {
        public int ID { get; set; }
        public int WeddingID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public bool TaskState { get; set; }

    }
}
