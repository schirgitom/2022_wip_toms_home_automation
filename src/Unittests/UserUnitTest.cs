using Context;
using Context.DAL;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class UserUnitTest : BaseUnitTests
    {
        [Test]
        public async Task CreateUser()
        {
            User user = new User();
            user.UserName = "schirgitom";
            user.Role = Role.Admin;
            user.ValidTill = DateTime.MaxValue;
            user.Password = "12345";
            user.Firstname = "Thomas";
            user.Lastname = "Schirgi";

            User returnval = await MongoUoW.Users.InsertOneAsync(user);

            Assert.NotNull(returnval);
        }


        [Test]
        public async Task CreateUserAndChange()
        {
            User user = new User();
            user.UserName = "schirgitom";
            user.Role = Role.Admin;
            user.ValidTill = DateTime.MaxValue;
            user.Password = "12345";
            user.Firstname = "Thomas";
            user.Lastname = "Schirgi";

            await MongoUoW.Users.InsertOneAsync(user);

            user.Firstname = "Thomas2";

            await MongoUoW.Users.UpdateOneAsync(user);
        }

        [Test]
        public async Task LoginTest()
        {
            String user = "schirgitom";
            String password = "12345";

            User usr = await MongoUoW.Users.Login(user, password);

            Assert.NotNull(usr);
        }


       
        [Test]
        public async Task LoginFailedTest()
        {
            String user = "schirgitom";
            String password = "123457";

            User usr = await MongoUoW.Users.Login(user, password);

            Assert.IsNull(usr);
        }
    }
}
