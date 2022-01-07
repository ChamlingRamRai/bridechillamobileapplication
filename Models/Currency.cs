using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    public class Currency
    {
        public int ID { get; set; }
        public string CurrencyType { get; set; }

        public override string ToString()
        {
            return this.CurrencyType;
        }
    }
}
