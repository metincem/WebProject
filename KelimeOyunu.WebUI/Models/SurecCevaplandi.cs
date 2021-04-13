using KelimeOyunu.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class SurecCevaplandi
    {
        public Surec SurecModel { get; set; }
        public int SoruIndex { get; set; }
    }
}
