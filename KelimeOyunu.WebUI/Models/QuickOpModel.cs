using KelimeOyunu.Business.Abstract;
using KelimeOyunu.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class QuickOpModel
    {
        public static int CevapBekleyen(Surec model)
        {
            for (int i = 14; i >= 1; i--)
            {
                if ((int)GetPropValue(model, "Soru" + i + "Durum") == 2)
                    return i;
            }
            return -1;
        }
        public static int CevapBekleyen(Surec model, int index)
        {
            for (int i = 14; i >= 1; i--)
            {
                if ((int)GetPropValue(model, "Soru" + i + "Durum") == index)
                    return i;
            }
            return -1;
        }

        public static TahminSonuc TahminDogrumu(Surec model, Oturum oturumModel, IServiceBs<Kelime> kelimeler, string tahminKelime, IServiceBs<Surec> surecBs, IServiceBs<Oturum> oturumBs)
        {
            TahminSonuc tahminSonuc = new TahminSonuc();
            int soruNo = CevapBekleyen(model);
            int kelimeID = (int)GetPropValue(oturumModel, "Soru" + soruNo);
            string kelime = kelimeler.Tekgetir(kelimeID).SoruKelime;
            if (tahminKelime.ToLower().Equals(kelime.ToLower()))
            {
                SetPropValue(model, "Soru" + soruNo + "Durum", 3);
                int uzunluk = GetPropValue(model, "Soru" + soruNo + "AHa") == null ? 0 : GetPropValue(model, "Soru" + soruNo + "AHa").ToString().Length;
                int alinanPuan = (kelime.Length - (uzunluk / 2)) * 100;
                oturumModel.Puan += alinanPuan;
                oturumBs.Guncelle(oturumModel);
                surecBs.Guncelle(model);
                tahminSonuc.Dogru = true;
                tahminSonuc.Kelime = tahminKelime;
                tahminSonuc.TotalPuan = oturumModel.Puan;
                tahminSonuc.SoruNo = soruNo;
                return tahminSonuc;
            }
            else
                return tahminSonuc;
        }

        public static TahminSonuc Cevaplanmadi(Surec model, Oturum oturumModel, IServiceBs<Oturum> oturumBs, IServiceBs<Surec> surecBs, IServiceBs<Kelime> kelimeBs)
        {
            TahminSonuc tahminSonuc = new TahminSonuc();
            int soruNo = CevapBekleyen(model);
            int kelimeID = (int)GetPropValue(oturumModel, "Soru" + soruNo);
            string kelime = kelimeBs.Tekgetir(kelimeID).SoruKelime;
            tahminSonuc.Kelime = kelime;
            int uzunluk = GetPropValue(model, "Soru" + soruNo + "AHa") == null ? 0 : GetPropValue(model, "Soru" + soruNo + "AHa").ToString().Length;
            int alinanPuan = (kelime.Length - (uzunluk / 2)) * 100;
            oturumModel.Puan -= alinanPuan;
            SetPropValue(model, "Soru" + soruNo + "Durum", 4);
            oturumBs.Guncelle(oturumModel);
            surecBs.Guncelle(model);
            tahminSonuc.TotalPuan = oturumModel.Puan;
            tahminSonuc.SoruNo = soruNo;
            return tahminSonuc;
        }

        public static object GetPropValue(Oturum src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);
        public static void SetPropValue(Oturum src, string propName, int deger) => src.GetType().GetProperty(propName).SetValue(src, deger);

        public static object GetPropValue(Surec src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);

        public static void SetPropValue(Surec src, string propName, int deger) => src.GetType().GetProperty(propName).SetValue(src, deger);

        public static object GetPropValue(SurecCevaplandi src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);

        public static SurecCevaplandi Cevaplandi(Surec model)
        {
            SurecCevaplandi srccvp = new SurecCevaplandi() { SoruIndex = -1, SurecModel = model };

            for (int i = 14; i >= 1; i--)
            {
                if ((int)GetPropValue(srccvp.SurecModel, "Soru" + i + "Durum") == 1)
                {
                    SetPropValue(srccvp.SurecModel, "Soru" + i + "Durum", 2);
                    srccvp.SoruIndex = i;
                    srccvp.SurecModel.BaslanacakSure = DateTime.Now;
                    break;
                }
            }
            return srccvp;
        }

        public static TahminSonuc HarflerBitti(Surec model, Oturum oturumModel, IServiceBs<Oturum> oturumBs, IServiceBs<Surec> surecBs, IServiceBs<Kelime> kelimeBs)
        {
            TahminSonuc tahminSonuc = new TahminSonuc();
            int soruNo = CevapBekleyen(model, 1);
            int kelimeID = (int)GetPropValue(oturumModel, "Soru" + soruNo);
            string kelime = kelimeBs.Tekgetir(kelimeID).SoruKelime;
            tahminSonuc.Kelime = kelime;
            tahminSonuc.TotalPuan = oturumModel.Puan;
            tahminSonuc.Kelime = kelime;
            tahminSonuc.Dogru = false;
            SetPropValue(model, "Soru" + soruNo + "Durum", 4);
            TimeSpan fark = DateTime.Now.Subtract(model.BaslanacakSure);
            model.KalanSure -= fark.Seconds;
            model.BaslanacakSure = DateTime.Now;
            surecBs.Guncelle(model);
            tahminSonuc.SoruNo = soruNo;
            return tahminSonuc;
        }

        public static SiradakiModel SiradakiSoru(Surec model, Oturum oturumModel, IServiceBs<Kelime> kelimeBs, IServiceBs<Oturum> oturumBs, IServiceBs<Surec> surecBs)
        {
            SiradakiModel siradakiModel = new SiradakiModel();
            int siradakiSoruNo = 0;
            for(int i = 1; i <= 14; i++)
            {
                if((int)GetPropValue(model, "Soru" + i + "Durum") == 0)
                {
                    siradakiSoruNo = i;
                    break;
                }
            }
            int kelimeID = (int)GetPropValue(oturumModel, "Soru" + siradakiSoruNo);
            Kelime kelime = kelimeBs.Tekgetir(kelimeID);
            siradakiModel.Aciklama = kelime.Aciklama;
            siradakiModel.KalanSure = model.KalanSure;
            siradakiModel.SoruNo = siradakiSoruNo;
            siradakiModel.TotalPuan = oturumModel.Puan;
            siradakiModel.KelimeUzunluk = kelime.KelimeUzunluk;
            SetPropValue(model, "Soru" + siradakiSoruNo + "Durum", 1);
            model.BaslanacakSure = DateTime.Now;
            surecBs.Guncelle(model);
            return siradakiModel;
        }

        public static SurecHarfModel RandomHarf(Surec model, Oturum oturumModel, IServiceBs<Kelime> kelimeBs)
        {
            SurecHarfModel modelHarf = new SurecHarfModel();
            modelHarf.HarflerBitti = false;
            if (model.Soru14Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru14).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru14AHa) ? 0 : model.Soru14AHa.Length;
                int[] sayilar = new int[10 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 10; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru14AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru14AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru14AHa += randomSayim;
                if (modelHarf.SurecModel.Soru14AHa.Length == 20)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 1000 - ((modelHarf.SurecModel.Soru14AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru13Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru13).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru13AHa) ? 0 : model.Soru13AHa.Length;
                int[] sayilar = new int[10 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 10; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru13AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru13AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru13AHa += randomSayim;
                if (modelHarf.SurecModel.Soru13AHa.Length == 20)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 1000 - ((modelHarf.SurecModel.Soru13AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru12Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru12).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru12AHa) ? 0 : model.Soru12AHa.Length;
                int[] sayilar = new int[9 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 9; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru12AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru12AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru12AHa += randomSayim;
                if (modelHarf.SurecModel.Soru12AHa.Length == 18)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 900 - ((modelHarf.SurecModel.Soru12AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru11Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru11).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru11AHa) ? 0 : model.Soru11AHa.Length;
                int[] sayilar = new int[9 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 9; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru11AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru11AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru11AHa += randomSayim;
                if (modelHarf.SurecModel.Soru11AHa.Length == 18)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 900 - ((modelHarf.SurecModel.Soru11AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru10Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru10).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru10AHa) ? 0 : model.Soru10AHa.Length;
                int[] sayilar = new int[8 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 8; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru10AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru10AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru10AHa += randomSayim;
                if (modelHarf.SurecModel.Soru10AHa.Length == 16)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 800 - ((modelHarf.SurecModel.Soru10AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru9Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru9).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru9AHa) ? 0 : model.Soru9AHa.Length;
                int[] sayilar = new int[8 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 8; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru9AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru9AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru9AHa += randomSayim;
                if (modelHarf.SurecModel.Soru9AHa.Length == 16)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 800 - ((modelHarf.SurecModel.Soru9AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru8Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru8).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru8AHa) ? 0 : model.Soru8AHa.Length;
                int[] sayilar = new int[7 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 7; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru8AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru8AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru8AHa += randomSayim;
                if (modelHarf.SurecModel.Soru8AHa.Length == 14)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 700 - ((modelHarf.SurecModel.Soru8AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru7Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru7).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru7AHa) ? 0 : model.Soru7AHa.Length;
                int[] sayilar = new int[7 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 7; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru7AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru7AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru7AHa += randomSayim;
                if (modelHarf.SurecModel.Soru7AHa.Length == 14)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 700 - ((modelHarf.SurecModel.Soru7AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru6Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru6).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru6AHa) ? 0 : model.Soru6AHa.Length;
                int[] sayilar = new int[6 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 6; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru6AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru6AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru6AHa += randomSayim;
                if (modelHarf.SurecModel.Soru6AHa.Length == 12)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 600 - ((modelHarf.SurecModel.Soru6AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru5Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru5).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru5AHa) ? 0 : model.Soru5AHa.Length;
                int[] sayilar = new int[6 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 6; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru5AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru5AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru5AHa += randomSayim;
                if (modelHarf.SurecModel.Soru5AHa.Length == 12)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 600 - ((modelHarf.SurecModel.Soru5AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru4Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru4).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru4AHa) ? 0 : model.Soru4AHa.Length;
                int[] sayilar = new int[5 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 5; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru4AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru4AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru4AHa += randomSayim;
                if (modelHarf.SurecModel.Soru4AHa.Length == 10)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 500 - ((modelHarf.SurecModel.Soru4AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru3Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru3).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru3AHa) ? 0 : model.Soru3AHa.Length;
                int[] sayilar = new int[5 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 5; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru3AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru3AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru3AHa += randomSayim;
                if (modelHarf.SurecModel.Soru3AHa.Length == 10)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 500 - ((modelHarf.SurecModel.Soru3AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru2Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru2).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru2AHa) ? 0 : model.Soru2AHa.Length;
                int[] sayilar = new int[4 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 4; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru2AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru2AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru2AHa += randomSayim;
                if (modelHarf.SurecModel.Soru2AHa.Length == 8)
                    modelHarf.HarflerBitti = true;

                modelHarf.SoruPuan = 400 - ((modelHarf.SurecModel.Soru2AHa.Length / 2) * 100);

                return modelHarf;
            }
            else if (model.Soru1Durum == 1)
            {
                string kelime = kelimeBs.Tekgetir(x => x.ID == oturumModel.Soru1).SoruKelime;
                Random rnd = new Random();
                int aharfSayisi = string.IsNullOrEmpty(model.Soru1AHa) ? 0 : model.Soru1AHa.Length;
                int[] sayilar = new int[4 - (aharfSayisi / 2)];
                int indexSayac = 0;
                for (int j = 1; j <= 4; j++)
                {
                    string sayiIndex = j.ToString();
                    if (sayiIndex.Length < 2)
                        sayiIndex = "0" + sayiIndex;

                    if (string.IsNullOrEmpty(model.Soru1AHa))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }
                    else if (!model.Soru1AHa.Contains(sayiIndex))
                    {
                        sayilar[indexSayac] = j;
                        indexSayac++;
                    }

                }

                int randomSayi = sayilar[rnd.Next(sayilar.Length - 1)];
                modelHarf.HarfIndex = randomSayi;
                modelHarf.Harf = kelime.Substring(randomSayi - 1, 1);
                string randomSayim = randomSayi.ToString();
                if (randomSayim.Length < 2)
                {
                    randomSayim = "0" + randomSayim;
                }
                modelHarf.SurecModel = model;
                modelHarf.SurecModel.Soru1AHa += randomSayim;
                if (modelHarf.SurecModel.Soru1AHa.Length == 8)
                    modelHarf.HarflerBitti = true;
                modelHarf.SoruPuan = 400 - ((modelHarf.SurecModel.Soru1AHa.Length / 2) * 100);

                return modelHarf;
            }
            return null;
        }

    }
}
