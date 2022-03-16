using Context.DAL;
using Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repository
{
    public interface IUserRepository :  IMongoRepository<User> 
    {

        Task<User> Login(String username, String password);

    }
}
