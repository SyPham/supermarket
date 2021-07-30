using Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.DTO
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsLock { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountType { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
     public class ChangePasswordRequest
    {
        public int Id { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
      
    }
}
