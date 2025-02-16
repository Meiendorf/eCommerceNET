using System.Linq.Expressions;
using  eCommerce.SharedLibrary.Response;

namespace eCommerce.SharedLibrary.Interface;

public interface IGenericInterface<T> where T : class
{
    Task<GenericResponse> CreateAsync(T entity);
    
    Task<GenericResponse> UpdateAsync(T entity);
    
    Task<GenericResponse> DeleteAsync(T entity);
    
    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> FindByIdAsync(int id);
    
    Task<T?> GetByAsync(Expression<Func<T, bool>> predicate);
}