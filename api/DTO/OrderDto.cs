using System;

namespace Supermarket.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsDelete { get; set; }
        public int? Status { get; set; }

        public string FullName { get; set; }
        public string EmployeeId { get; set; }

        public int ConsumerId { get; set; }
        public bool PendingStatus { get; set; }
        public bool ByingStatus { get; set; }
        public bool CompleteStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
     public class UpdateQuantityOrderRequest
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int DetailId { get; set; }
        public int HistoryId { get; set; }
        public int Quantity { get; set; }
    }
        public class DeleteCartOrderRequest
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        
        public int DetailId { get; set; }
        public int HistoryId { get; set; }

    }
    
}
