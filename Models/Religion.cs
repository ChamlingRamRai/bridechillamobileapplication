using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    public class Religion
    {
        public int ReligionId { get; set; }
        public string ReligionType { get; set; }

        public override string ToString()
        {
            return  this.ReligionType;
        }
    }
}
