using Supermarket.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.Models
{
    [Table("Groups")]
    public class Group : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }

    }
}
