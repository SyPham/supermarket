using Supermarket.Models.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    [Table("Orders")]
    public class Order: IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsDelete { get; set; }
        public int? Status { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; }
        [MaxLength(50)]
        public string EmployeeId { get; set; }

        public int ConsumerId { get; set; }
        public bool PendingStatus { get; set; }
        public bool ByingStatus { get; set; }
        public bool CompleteStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
      
        [ForeignKey(nameof(ConsumerId))]
        public virtual Consumer Consumer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
