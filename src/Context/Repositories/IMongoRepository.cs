using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories
{
    public interface IMongoRepository<TEntity> where TEntity : MongoDocument
    {
        IEnumerable<TEntity> FilterBy(
          Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> FindByIdAsync(string id);

        Task<TEntity> InsertOneAsync(TEntity document);

        Task<TEntity> UpdateOneAsync(TEntity document);

        Task<TEntity> InsertOrUpdateOneAsync(TEntity document);

        Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        Task DeleteByIdAsync(string id);

        Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);
    }
}
