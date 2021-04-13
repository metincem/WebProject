using KelimeOyunu.Business.Abstract;
using KelimeOyunu.Entity;
using KelimeOyunu.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace KelimeOyunu.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceBs<Oturum> oturumBs;
        private readonly IServiceBs<Surec> surecBs;
        private readonly IServiceBs<Kelime> kelimeBs;
        private IWebHostEnvironment environment;
        public HomeController(IServiceBs<Oturum> _oturumBs, IServiceBs<Surec> _surecBs, IServiceBs<Kelime> _kelimeBs, IWebHostEnvironment _environment)
        {
            oturumBs = _oturumBs;
            surecBs = _surecBs;
            kelimeBs = _kelimeBs;
            environment = _environment;
        }

        [Route("/Eniyiler")]
        public IActionResult Eniyiler()
        {
            List<SkorlarModel> skorlar = new List<SkorlarModel>();

            string docPath = environment.WebRootPath;
            string jsonGelen;
            if (System.IO.File.Exists(Path.Combine(docPath, "Scores.mcb")))
            {
                using (var sr = new StreamReader(Path.Combine(docPath, "Scores.mcb")))
                {
                    jsonGelen = sr.ReadToEnd();
                }
                skorlar = JsonConvert.DeserializeObject<List<SkorlarModel>>(jsonGelen);
            }

            return View(skorlar.OrderByDescending(x => x.Puan).ThenByDescending(x => x.ArtanSure).Take(10).ToList());
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/Basla")]
        public IActionResult Begin()
        {
            return View();
        }

        [HttpPost]
        [Route("/Basla")]
        public IActionResult Begin(string wordGss)
        {
            DateTime nwtm = DateTime.Now;
            Oturum oturum = new Oturum() { AdSoyad = wordGss, OturumSaati = nwtm, Puan = 0, Sonlandi = false };

            int kelimeUzunluk = 4;
            for (int i = 1; i <= 14; i++)
            {
                List<Kelime> kelimeler = kelimeBs.FiltreliGetir(x => x.KelimeUzunluk == kelimeUzunluk).OrderBy(x => x.SonKullanma).Take(6).ToList();
                Random rd = new Random();
                Kelime kelime = kelimeler[rd.Next(5)];
                QuickOpModel.SetPropValue(oturum, "Soru" + i, kelime.ID);
                kelime.SonKullanma = DateTime.Now;
                kelimeBs.Guncelle(kelime);
                if (i % 2 == 0)
                    kelimeUzunluk++;
            }

            oturumBs.Kaydet(oturum);

            int id = oturumBs.Tekgetir(x => x.OturumSaati.CompareTo(nwtm) == 0).ID;
            surecBs.Kaydet(new Surec
            {
                OturumID = id,
                KalanSure = 240,
                SureDurduruldu = false
            });
            HttpContext.Session.SetInt32("oturumID", id);
            return RedirectToAction("KelimeOyunu");
        }

        [Route("/KelimeOyunu")]
        public IActionResult KelimeOyunu()
        {
            int oid = HttpContext.Session.GetInt32("oturumID") == null ? 0 : (int)HttpContext.Session.GetInt32("oturumID");
            Oturum oturum = oturumBs.Tekgetir(oid);

            if (oturum == null)
                return NotFound();
            if (oturum.Sonlandi)
                return NotFound();

            oturum.Sonlandi = true;
            Surec sur = surecBs.Tekgetir(x => x.OturumID == oid);
            sur.BaslanacakSure = DateTime.Now;
            surecBs.Guncelle(sur);
            oturumBs.Guncelle(oturum);
            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32("sonucID", oid);
            return View("KelimeOyunu", oturum.ID);
        }

        public IActionResult KelimeKaydet()
        {
            List<Kelime> model = kelimeBs.TumGetir();
            return View(model);
        }

        [HttpPost]
        public JsonResult KelimeKaydet(string kelime, string soru)
        {
            DateTime simdi = DateTime.Now;
            kelimeBs.Kaydet(new Kelime
            {
                KelimeUzunluk = kelime.Length,
                SoruKelime = kelime,
                Aciklama = soru,
                SonKullanma = simdi
            });
            Kelime model = kelimeBs.Tekgetir(x => x.SonKullanma == simdi);
            return Json(model);
        }

        [HttpPost]
        public JsonResult KelimeSil(int soruId)
        {
            Kelime kelime = kelimeBs.Tekgetir(soruId);
            kelimeBs.Sil(kelime);
            return Json(new { sonuc = 1 });
        }

        [Route("/Sonuc")]
        public IActionResult Sonuc()
        {
            int oid = HttpContext.Session.GetInt32("sonucID") == null ? 0 : (int)HttpContext.Session.GetInt32("sonucID");
            if (oid == 0)
            {
                return NotFound();
            }
            Oturum oturum = oturumBs.Tekgetir(oid);
            Surec surec = surecBs.Tekgetir(x => x.OturumID == oid);
            oturum.ArtanSure = surec.KalanSure;
            SonucModel model = new SonucModel { OturumModel = oturum };
            int dogruSayisi = 0;
            for (int i = 1; i <= 14; i++)
            {
                if ((int)QuickOpModel.GetPropValue(surec, "Soru" + i + "Durum") == 3)
                    dogruSayisi++;

                if ((int)QuickOpModel.GetPropValue(surec, "Soru" + i + "Durum") == 0)
                {
                    model.OturumModel.ArtanSure = 0;
                    oturumBs.Guncelle(model.OturumModel);
                    break;
                }
            }
            model.DogruSayisi = dogruSayisi;
            oturumBs.Guncelle(oturum);
            HttpContext.Session.Clear();
            List<SkorlarModel> skorlar = new List<SkorlarModel>();
            SkorlarModel skorModel = new SkorlarModel { AdSoyad = oturum.AdSoyad, ArtanSure = oturum.ArtanSure, Puan = oturum.Puan, Zaman = oturum.OturumSaati };

            string docPath = environment.WebRootPath;
            string jsonGelen;
            if (System.IO.File.Exists(Path.Combine(docPath, "Scores.mcb")))
            {
                using (var sr = new StreamReader(Path.Combine(docPath, "Scores.mcb")))
                {
                    jsonGelen = sr.ReadToEnd();
                }
                skorlar = JsonConvert.DeserializeObject<List<SkorlarModel>>(jsonGelen);
            }
            if (skorlar.Count != 0)
            {
                SkorlarModel skmdl = skorlar.Where(x => x.AdSoyad.ToLower().Equals(skorModel.AdSoyad.ToLower())).FirstOrDefault();
                if (skmdl != null)
                {
                    if (skmdl.Puan < skorModel.Puan)
                    {
                        skorlar.Remove(skmdl);
                        skorlar.Add(skorModel);
                    }
                }
                else
                    skorlar.Add(skorModel);
            }
            else
            {
                skorlar.Add(skorModel);
            }


            string json = JsonConvert.SerializeObject(skorlar);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Scores.mcb"), false))
            {
                outputFile.WriteLine(json);
            }

            model.Skorlar = skorlar.OrderByDescending(x => x.Puan).Take(10).ToList();
            return View(model);
        }
    }
}
