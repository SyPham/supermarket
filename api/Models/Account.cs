﻿using Supermarket.Models.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.Models
{
    [Table("Accounts")]
    public class Account: IDateTracking
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Username { get; set; }
        [MaxLength(255)]
        public string Password { get; set; }
        public int Group_ID { get; set; }
        public int Team_ID { get; set; }
        public bool IsLock { get; set; }
        public int? ConsumerId { get; set; }
        public int? AccountTypeId { get; set; }
      
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedTime { get ; set ; }
        public DateTime? ModifiedTime { get ; set ; }


        [ForeignKey(nameof(AccountTypeId))]
        public virtual AccountType AccountType { get; set; }
        [ForeignKey(nameof(Group_ID))]
        public virtual Group Group { get; set; }
        [ForeignKey(nameof(Team_ID))]
        public virtual Team Team { get; set; }
        [ForeignKey(nameof(ConsumerId))]
        public virtual Consumer Consumer { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }


    }
}
