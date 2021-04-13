using KelimeOyunu.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class SonucModel
    {
        public Oturum OturumModel { get; set; }
        public int DogruSayisi { get; set; }
        public List<SkorlarModel> Skorlar { get; set; }
    }
}
