using System;
using System.Collections.Generic;
using System.Text;

namespace KelimeOyunu.Entity
{
    public class Kelime
    {
        public int ID { get; set; }
        public int KelimeUzunluk { get; set; }
        public string SoruKelime { get; set; }
        public string Aciklama { get; set; }
        public DateTime SonKullanma { get; set; }
    }
}
