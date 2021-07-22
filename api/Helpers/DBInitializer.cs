using Microsoft.EntityFrameworkCore;
using Supermarket.Constants;
using Supermarket.Data;
using Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Helpers
{
    public static class DBInitializer
    {
        //private readonly DataContext _context;
        //public DBInitializer(DataContext context)
        //{
        //    _context = context;
        //}
        public static void Seed(DataContext _context)
        {
            #region AccountTypes

            if (!(_context.AccountTypes.Any()))
            {
                _context.AccountTypes.AddRange(new List<AccountType> {
                    new AccountType(SystemRole.Admin, "Admin"),
                    new AccountType(SystemRole.Consumer, "Consumer"),
                });
                _context.SaveChanges();
            }
            #endregion
            #region Account

            if (!(_context.Accounts.Any()))
            {
                _context.Accounts.AddRange(new List<Account> {
                    new Account {
                        Username = "Admin" ,
                        Password = "1".ToEncrypt() ,
                        AccountTypeId = _context.AccountTypes.FirstOrDefault(x => x.Code == SystemRole.Admin).Id
                    },
                    new Account {
                        Username = "Consumer" ,
                        Password = "1".ToEncrypt() ,
                        AccountTypeId = _context.AccountTypes.FirstOrDefault(x => x.Code == SystemRole.Consumer).Id
                    },

                });
                _context.SaveChanges();
            }
            #endregion

           

            #region Store
            if (!(_context.Stores.Any()))
            {
                _context.Stores.AddRange(new List<Store> {
                    new Store {Name = "AEON"},
                    new Store {Name = "Big C"}
                });
                _context.SaveChanges();
            }

            #endregion

            #region Kind
            if (!(_context.Kinds.Any()))
            {
                _context.Kinds.AddRange(new List<Kind> {
                    new Kind {VietnameseName = "Sữa" , ChineseName = "奶製品", EnglishName = "Milk"},
                    new Kind {VietnameseName = "Đồ uống" , ChineseName = "飲料", EnglishName = "Drinks"},
                    new Kind {VietnameseName = "Khác" , ChineseName = "舶來品", EnglishName = "Foreign area"},
                });
                _context.SaveChanges();
            }

            #endregion

        }
    }
}
