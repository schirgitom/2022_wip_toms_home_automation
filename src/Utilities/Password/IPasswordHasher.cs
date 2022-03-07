﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Password
{
    public interface IPasswordHasher
    {
            string Hash(string password);

            bool Check(string hash, string password);
    }
}
