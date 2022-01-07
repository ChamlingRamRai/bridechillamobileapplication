using BrideChillaPOC.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class ReligionViewModel 
    {
        public List<Religion> ReligionList { get; set; }

        public ReligionViewModel()
        {
            ReligionList = GetReligions().OrderBy(t => t.ReligionType).ToList();
        }
        public List<Religion> GetReligions()
        {
            List<Religion> religions = new List<Religion>()

            {
                new Religion(){ReligionId = 1, ReligionType="Christian"},
                new Religion(){ReligionId = 2, ReligionType="Muslim"},
                new Religion(){ReligionId = 3, ReligionType="Other"}
            };
            return religions;
        }
    }
}
