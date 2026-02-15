using System.Linq.Expressions;
using SpotifyRoast.Dtos;

namespace SpotifyRoast.Services
{
    public interface IGeneric<T> where T : class
    {
        ResponseDto<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        ResponseDto<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        ResponseDto<T> GetById(int id);
        ResponseDto<T> Insert(T entity);
        ResponseDto<T> Update(T entity);
        ResponseDto<T> Delete(T entity);
        ResponseDto<T> DeleteRange(IEnumerable<T> entities);
    }
}
