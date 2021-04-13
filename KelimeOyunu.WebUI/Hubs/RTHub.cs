using KelimeOyunu.Business.Abstract;
using KelimeOyunu.Entity;
using KelimeOyunu.WebUI.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Hubs
{
    public class RTHub:Hub
    {
        private readonly IServiceBs<Kelime> kelimeBs;
        private readonly IServiceBs<Oturum> oturumBs;
        private readonly IServiceBs<Surec> surecBs;
        public RTHub(IServiceBs<Kelime> _kelimeBs, IServiceBs<Oturum> _oturumBs, IServiceBs<Surec> _surecBs)
        {
            kelimeBs = _kelimeBs;
            oturumBs = _oturumBs;
            surecBs = _surecBs;
        }
        public async Task SurecBasladi(string idNo)
        {
            int id = Convert.ToInt32(idNo);
            int kelimeNo = oturumBs.Tekgetir(x => x.ID == id).Soru1;
            Kelime soru = kelimeBs.Tekgetir(x => x.ID == kelimeNo);
            Surec surec= surecBs.Tekgetir(x => x.OturumID == id);
            surec.SrConnectionID = Context.ConnectionId;
            surec.Soru1Durum = 1;
            surec.BaslanacakSure = DateTime.Now;
            surecBs.Guncelle(surec);
            await Clients.Caller.SendAsync("sorubildiri", soru.Aciklama, 1, soru.KelimeUzunluk, 240, 0);

            //await Clients.Caller.SendAsync("rtmessage", Context.ConnectionId);
            //await Clients.Client(Context.ConnectionId).SendAsync("rtmessage", "Oldu");
        }

        public async Task HarfAlindi()
        {
            Surec surec = surecBs.Tekgetir(x => x.SrConnectionID.Equals(Context.ConnectionId));
            Oturum oturum = oturumBs.Tekgetir(x => x.ID == surec.OturumID);
            SurecHarfModel model = QuickOpModel.RandomHarf(surec, oturum, kelimeBs);
            surec = model.SurecModel;
            surecBs.Guncelle(surec);
            await Clients.Caller.SendAsync("harfbildiri", model.Harf, model.HarfIndex, model.SoruPuan);
            if(model.HarflerBitti)
            {
                TahminSonuc sonuc = QuickOpModel.HarflerBitti(surec, oturum, oturumBs, surecBs, kelimeBs);
                await Clients.Caller.SendAsync("cevapsiz", sonuc.TotalPuan, sonuc.Kelime, sonuc.SoruNo);
            }
        }

        public async Task CevapVer()
        {
            Surec surec = surecBs.Tekgetir(x => x.SrConnectionID.Equals(Context.ConnectionId));
            TimeSpan fark = DateTime.Now.Subtract(surec.BaslanacakSure);
            surec.KalanSure -= fark.Seconds;
            SurecCevaplandi src = QuickOpModel.Cevaplandi(surec);
            surecBs.Guncelle(src.SurecModel);
            if(surec.KalanSure < 0)
                await Clients.Caller.SendAsync("sorubildiri", "<script>window.location.href = \"/Sonuc\";</script>", 1, 0, 240, 0);
        }

        public async Task TahminEt(string kelime)
        {
            if(!string.IsNullOrEmpty(kelime))
            {
                Surec surec = surecBs.Tekgetir(x => x.SrConnectionID.Equals(Context.ConnectionId));
                TimeSpan fark = DateTime.Now.Subtract(surec.BaslanacakSure);
                int saniyeFarki = fark.Seconds;
                if (saniyeFarki > 22)
                {
                    await Clients.Caller.SendAsync("sorubildiri", "<script>alert('Süre Doldu')</script>", 1, 0, 240, 0);
                }
                else
                {
                    Oturum oturum = oturumBs.Tekgetir(surec.OturumID);
                    TahminSonuc snc = QuickOpModel.TahminDogrumu(surec, oturum, kelimeBs, kelime, surecBs, oturumBs);
                    if (snc.Dogru)
                    {
                        await Clients.Caller.SendAsync("tahminsonuc", 1, snc.TotalPuan, snc.Kelime, snc.SoruNo);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("tahminsonuc", 0);
                    }
                }
            }
            
        }

        public async Task Cevaplanmadi()
        {
            Surec surec = surecBs.Tekgetir(x => x.SrConnectionID.Equals(Context.ConnectionId));
            Oturum oturum = oturumBs.Tekgetir(surec.OturumID);
            TahminSonuc sonuc = QuickOpModel.Cevaplanmadi(surec, oturum, oturumBs, surecBs, kelimeBs);

            await Clients.Caller.SendAsync("cevapsiz", sonuc.TotalPuan, sonuc.Kelime, sonuc.SoruNo);
        }

        public async Task SiradakiSoru()
        {
            Surec surec = surecBs.Tekgetir(x => x.SrConnectionID.Equals(Context.ConnectionId));
            Oturum oturum = oturumBs.Tekgetir(surec.OturumID);
            SiradakiModel model = QuickOpModel.SiradakiSoru(surec, oturum, kelimeBs, oturumBs, surecBs);
            await Clients.Caller.SendAsync("sorubildiri", model.Aciklama, model.SoruNo, model.KelimeUzunluk, model.KalanSure, model.TotalPuan);
        }
    }
}
