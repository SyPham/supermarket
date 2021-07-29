using System;

namespace Supermarket.DTO
{
    public class OrderDetailHistoryDto
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
        public int PendingQty { get; set; }
        public int ByingQty { get; set; }
        public bool CancelStatus { get; set; }
        public int CompleteQty { get; set; }
    }

    public class AddToBuyListDto
    {
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
    }

    public class AddToCompleteListDto
    {
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
        public int QtyTamp { get; set; }
    }
}
