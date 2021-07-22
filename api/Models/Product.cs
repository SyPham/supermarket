using Supermarket.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Supermarket.Models
{
    [Table("Products")]
    public class Product: IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string VietnameseName { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public string Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Avatar { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public int StoreId { get; set; }
        public int KindId { get; set; }

        [ForeignKey(nameof(StoreId))]
        public virtual Store Store { get; set; }

        [ForeignKey(nameof(KindId))]
        public virtual Kind Kind { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }

    }
}
