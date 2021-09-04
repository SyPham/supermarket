using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.Models
{
    public class Cart
    {
        public int AccountId { get; set; }
        public int TeamId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public DateTime CreatedTime { get; set; }

        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}
