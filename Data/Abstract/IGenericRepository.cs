using System.Linq.Expressions;

namespace BlogProject.Data.Abstract
{
    public interface IGenericRepository<T> where T : class
    {  //Ienumerable olma nedeni herhangi bir tür olabilir ve koleksiyon üzerinde döngüde önemli yeri var.
    //T tipinde nesnelerden olusan koleksiyonu döndüren yapılar.
       
        // Temel CRUD işlemleri
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);//veritabanından filtreli verileri alır
        Task<IEnumerable<T>> FindWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);//filtreleme yaparkenilişkili veriler de getirilir.
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
} 