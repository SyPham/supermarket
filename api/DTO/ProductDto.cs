using Microsoft.AspNetCore.Http;
using System;
namespace Supermarket.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string VietnameseName { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public decimal OriginalPrice { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public int StoreId { get; set; }
        public int KindId { get; set; }
     
    }
    public class UploadAvatarRequest
    {
        public IFormFile File { get; set; }
        public int Id { get; set; }

    }
}
