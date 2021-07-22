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

        public int? CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
   
}
