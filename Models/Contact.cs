using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int WeddingId { get; set; }
        public int TypeId { get; set; }


    }
}
