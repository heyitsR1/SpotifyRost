using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpotifyRoast.Data;
using SpotifyRoast.Dtos;
using SpotifyRoast.Services;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace SpotifyRoast.Repositories
{
    public class GenericRepository<T> : IGeneric<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        private readonly ResponseDto<T> _responseDto;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            _dbSet = context.Set<T>();
            _responseDto = new ResponseDto<T>();
        }

        public ResponseDto<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            try
            {
                IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

                query = query.Where(filter);

                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                var result = query.FirstOrDefault();
                if (result != null)
                {
                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Data = result;
                }
                else
                {
                    _responseDto.StatusCode = HttpStatusCode.NotFound;
                    _responseDto.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
            }

            return _responseDto;
        }

        public ResponseDto<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
        {
            try
            {
                IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                // Initial simplistic encoding approach - skipped for now to avoid complexity with System.Web dependency
                
                var list = query.ToList();
                if (list.Count > 0)
                {
                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Datas = list;
                }
                else
                {
                    _responseDto.StatusCode = HttpStatusCode.NotFound;
                    _responseDto.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = $"Failed due to: {ex.Message}";
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
            }

            return _responseDto;
        }

        public ResponseDto<T> GetById(int id)
        {
            try
            {
                var item = _dbSet.Find(id);
                if (item != null)
                {
                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Data = item;
                }
                else
                {
                    _responseDto.StatusCode = HttpStatusCode.NotFound;
                    _responseDto.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _responseDto;
        }

        public ResponseDto<T> Insert(T entity)
        {
            try
            {
                // Using transaction for single insert is overkill but matches reference pattern
                using var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    _dbSet.Add(entity);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Data = entity;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
                _responseDto.Data = entity;
            }
            return _responseDto;
        }

        public ResponseDto<T> Update(T entity)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    _dbSet.Update(entity);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Data = entity;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
                _responseDto.Data = entity;
            }
            return _responseDto;
        }

        public ResponseDto<T> Delete(T entity)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    _dbSet.Remove(entity);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Data = entity;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
                _responseDto.Data = entity;
            }
            return _responseDto;
        }

        public ResponseDto<T> DeleteRange(IEnumerable<T> entities)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    _dbSet.RemoveRange(entities);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    _responseDto.StatusCode = HttpStatusCode.OK;
                    _responseDto.Datas = entities.ToList();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = "Failed due to: " + ex.Message;
                _responseDto.StatusCode = HttpStatusCode.InternalServerError;
                _responseDto.Datas = entities.ToList();
            }
            return _responseDto;
        }
    }
}
