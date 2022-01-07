using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int WeddingId { get; set; }
        public int RoleId { get; set; }

    }
}
