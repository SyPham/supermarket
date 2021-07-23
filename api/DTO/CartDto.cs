using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.DTO
{
    public class CartDto
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedTime { get; set; }
    }
    public class UpdateQuantityRequest
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class AddToCartRequest
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class DeleteCartRequest
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
    }
}
