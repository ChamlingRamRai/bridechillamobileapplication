using System;
using System.Collections.Generic;
using System.Text;

namespace BrideChillaPOC.Models
{
    class BudgetModel
    {
        public int BudgetId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int Cost { get; set; }
        public int WeddingId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }

    }
}
