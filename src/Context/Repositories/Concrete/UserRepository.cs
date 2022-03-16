using Context.DAL;
using Context.Repositories;
using Context.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Password;

namespace Context.Repository
{
    public class UserRepository : MongoRepository<User>,  IUserRepository
    {
        PasswordHasher hasher = new PasswordHasher();
        public UserRepository(MongoDBContext Context) : base(Context)
        {
        }

       
        public async override Task<User> InsertOneAsync(User document)
        {
            SetUser(document);
            document = await base.InsertOneAsync(document);
            return document;
        }

        public async Task<User> Login(string username, string password)
        {

            User fromdb = await base.FindOneAsync(x => x.UserName == username);

            if (fromdb != null)
            {
                    Boolean verified = hasher.Check(fromdb.HashedPassword, password);

                    if(verified)
                    {
                        
                            return fromdb;
                        
                    }
                    else
                    {
                        log.Warning("Cannot login " + username + " Wrong password");
                    }
               
            }
            else
            {
                log.Warning("User " + username + " not available");
            }

            return null;

        }

        public async override Task<User> UpdateOneAsync(User document)
        {
            SetUser(document);
            document = await base.UpdateOneAsync(document);
            return document;
        }

    
        private void SetUser(User document)
        {
            document.HashedPassword = hasher.Hash(document.Password);

        }
    }
}
