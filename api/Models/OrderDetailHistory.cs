using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace Supermarket.Models
{
    [Table("OrderDetailHistories")]
    public class OrderDetailHistory
    {
        public OrderDetailHistory()
        {
            this.DispatchDate = DateTime.Now;
            //this.CreatedTime = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss tt");
            this.OrderDate = DateTime.Now;

        }
        [Key]
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
        public int PendingQty { get; set; }
        public int ByingQty { get; set; }
        public bool CancelStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DispatchDate { get; set; }
        public int CompleteQty { get; set; }
        public int? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }
        [ForeignKey(nameof(ConsumerId))]
        public virtual Consumer Consumer { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
     
    }
}
