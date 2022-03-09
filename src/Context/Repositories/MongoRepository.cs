using Context.Settings;
using MongoDB.Driver;
using MongoDB.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : MongoDocument
    {
        ILogger log = Utilities.Logger.ContextLog<MongoDocument>();

        private readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(MongoDBContext Context)
        {
            _collection = Context.DataBase.GetCollection<TEntity>(typeof(TEntity).Name.ToString());
        }


        public Task DeleteByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> InsertOneAsync(TEntity document)
        {
            try
            {
                await document.SaveAsync();
                return document;
            }
            catch (Exception ex)
            {
                log.Error("Error during saving....");
            }

            return default;
        }

        public Task<TEntity> InsertOrUpdateOneAsync(TEntity document)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateOneAsync(TEntity document)
        {
            throw new NotImplementedException();
        }
    }
}
