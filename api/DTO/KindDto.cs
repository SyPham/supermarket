using System;

namespace Supermarket.DTO
{
    public class KindDto 
    {
        public int Id { get; set; }
        public string VietnameseName { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public int CreatedBy { get; set; }
        public int Store_ID { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
