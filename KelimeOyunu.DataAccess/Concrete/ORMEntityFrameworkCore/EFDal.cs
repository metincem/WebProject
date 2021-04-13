using KelimeOyunu.DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KelimeOyunu.DataAccess.Concrete.ORMEntityFrameworkCore
{
    public class EFDal<T, TContext> : IGenericRepository<T>
        where T : class
        where TContext : DbContext, new()
    {
        public List<T> FiltreliGet(Expression<Func<T, bool>> filter)
        {
            using(var context = new TContext())
            {
                return context.Set<T>().Where(filter).ToList(); ;
            }
        }

        public string Guncelleme(T entity)
        {
            using(var context = new TContext())
            {
                try
                {
                    context.Entry(entity).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                    return "Hata:" + e.Message;
                }
                return null;
            }
        }

        public string Kayit(T entity)
        {
            using(var context = new TContext())
            {
                try
                {
                    context.Set<T>().Add(entity);
                    context.SaveChanges();
                }
                catch(Exception e)
                {
                    return "Hata:" + e.Message;
                }
                return null;
            }
        }

        public string Sil(T entity)
        {
            using(var context = new TContext())
            {
                try
                {
                    context.Set<T>().Remove(entity);
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                    return "Hata:" + e.Message;
                }
                return null;
            }
        }

        public T TekGet(Expression<Func<T, bool>> filter)
        {
            using(var context = new TContext())
            {
                return context.Set<T>().Where(filter).FirstOrDefault();
            }
        }

        public T TekGet(int id)
        {
            using(var context = new TContext())
            {
                return context.Set<T>().Find(id);
            }
        }

        public List<T> TumGet()
        {
            using(var context = new TContext())
            {
                return context.Set<T>().ToList();
            }
        }
    }
}
