using KelimeOyunu.Business.Abstract;
using KelimeOyunu.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KelimeOyunu.Business.Concrete
{
    public class ManagerBs<T> : IServiceBs<T> where T : class
    {
        IGenericRepository<T> genericRepository;
        public ManagerBs(IGenericRepository<T> _genericRepository)
        {
            genericRepository = _genericRepository;
        }
        public List<T> FiltreliGetir(Expression<Func<T, bool>> filter)
        {
            return genericRepository.FiltreliGet(filter);
        }

        public string Guncelle(T entity)
        {
            return genericRepository.Guncelleme(entity);
        }

        public string Kaydet(T entity)
        {
            return genericRepository.Kayit(entity);
        }

        public string Sil(T entity)
        {
            return genericRepository.Sil(entity);
        }

        public T Tekgetir(int id)
        {
            return genericRepository.TekGet(id);
        }

        public T Tekgetir(Expression<Func<T, bool>> filter)
        {
            return genericRepository.TekGet(filter);
        }

        public List<T> TumGetir()
        {
            return genericRepository.TumGet();
        }
    }
}
