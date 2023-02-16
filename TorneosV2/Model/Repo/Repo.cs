using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TorneosV2.Model.Interface;

namespace TorneosV2.Model.Repo
{
    public class Repo<TEntity, TDataContext> : ApiFiltroGet<TEntity>
        where TEntity : class
        where TDataContext : DbContext
    {
        protected readonly TDataContext context;
        internal DbSet<TEntity> dbset;

        public Repo(TDataContext dataContext)
        {
            context = dataContext;
            dbset = context.Set<TEntity>();
        }

        public virtual async Task<bool> DeleteEntity(TEntity entityToDel)
        {
            if (context.Entry(entityToDel).State == EntityState.Detached)
            {
                dbset.Attach(entityToDel);
            }
            dbset.Remove(entityToDel);
            return await context.SaveChangesAsync() >= 1;
        }

        public virtual async Task<bool> DeleteEntity(object id)
        {
            TEntity entityToDel = await dbset.FindAsync(id);
            return await DeleteEntity(entityToDel);
        }

        public virtual async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filtro = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string propiedades = "")
        {
            try
            {
                IQueryable<TEntity> querry = dbset;
                if (filtro != null)
                {
                    querry = querry.Where(filtro);
                }
                foreach (var propiedad in propiedades.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    querry = querry.Include(propiedad);
                }
                if (orderby != null)
                {
                    return orderby(querry).ToList();
                }
                else
                {
                    return await querry.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var msn = ex.Message;
                return null;

            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            await Task.Delay(1);
            return dbset;
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            return await dbset.FindAsync(id);
        }

        public virtual async Task<TEntity> Insert(TEntity entity)
        {
            await dbset.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> InsertPlus(IEnumerable<TEntity> entities)
        {
            await dbset.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return entities;
        }

        public virtual async Task<TEntity> Update(TEntity entityToUpdate)
        {
            var dbSet = context.Set<TEntity>();
            dbset.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entityToUpdate;
        }
    }
}
