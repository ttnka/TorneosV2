namespace TorneosV2.Model.Interface
{
    public interface IRepo<TEntity> where TEntity : class
    {
        Task<bool> DeleteEntity(TEntity entityToDel);
        Task<bool> DeleteEntity(object id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(object id);
        Task<TEntity> Insert(TEntity entity);
        Task<IEnumerable<TEntity>> InsertPlus(IEnumerable<TEntity> entities);
        Task<TEntity> Update(TEntity entityToUpdate);

    }
}
