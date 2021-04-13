using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KelimeOyunu.DataAccess.Abstract
{
    public interface IGenericRepository<T> where T:class
    {
        List<T> TumGet();
        List<T> FiltreliGet(Expression<Func<T, bool>> filter);
        T TekGet(Expression<Func<T, bool>> filter);
        T TekGet(int id);
        string Kayit(T entity);
        string Guncelleme(T entity);
        string Sil(T entity);
    }
}
