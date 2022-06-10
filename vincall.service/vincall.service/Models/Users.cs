using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class Users
    {
        public static List<User> All => new List<User>
        {
            new User
            {
                Account = "admin21",
                UserName = "Admin21",
                CreateDate = DateTime.UtcNow,
                IsAdmin = true,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "admin20",
                UserName = "Admin20",
                CreateDate = DateTime.UtcNow,
                IsAdmin = true,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "Peter",
                UserName = "Peter",
                CreateDate = DateTime.UtcNow,
                IsAdmin = false,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "Tom",
                UserName = "Tom",
                CreateDate = DateTime.UtcNow,
                IsAdmin = false,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "Grubby",
                UserName = "Grubby",
                CreateDate = DateTime.UtcNow,
                IsAdmin = false,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "admin",
                UserName = "Admin",
                CreateDate = DateTime.UtcNow,
                IsAdmin = true,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "Jerry",
                UserName = "Jerry",
                CreateDate = DateTime.UtcNow,
                IsAdmin = false,
                Password = Md5Helper.Md5("Aa000000")
            },
            new User
            {
                Account = "mary",
                UserName = "Mary",
                CreateDate = DateTime.UtcNow,
                IsAdmin = false,
                Password = Md5Helper.Md5("Aa000000")
            }

        };
    }
}
