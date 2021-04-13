using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KelimeOyunu.Business.Abstract
{
    public interface IServiceBs<T> where T : class
    {
        List<T> TumGetir();
        T Tekgetir(int id);
        T Tekgetir(Expression<Func<T, bool>> filter);
        List<T> FiltreliGetir(Expression<Func<T, bool>> filter);
        string Kaydet(T entity);
        string Sil(T entity);
        string Guncelle(T entity);
    }
}
