using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

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

        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public int StoreId { get; set; }
        public string Store { get; set; }
        public int KindId { get; set; }
        public string Kind { get; set; }

    }
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string OriginalPrice { get; set; }
        public string StoreName { get; set; }
        public string KindName { get; set; }
        public int? Quantity { get; set; }
        public int StoreId { get; set; }
        public int KindId { get; set; }

    }
    public class ProductCartDto
    {
        public int ProductId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int ConsumerId { get; set; }
        public string StoreName { get; set; }
        public int OderDetailId { get; set; }
        public string KindName { get; set; }
        public string SubtotalPrice { get; set; }
        public string totalPrice { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string OriginalPrice { get; set; }
        public bool IsDelete { get; set; }
        public int? Quantity { get; set; }
        public int StoreId { get; set; }
        public int KindId { get; set; }
        public string Amount { get; set; }
        public decimal AmountValue { get; set; }

    }
    public class ProductBuyingDto
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string EmployeeId { get; set; }
        public int ConsumerId { get; set; }
        public string StoreName { get; set; }
        public int OderDetailId { get; set; }
        public string KindName { get; set; }
        public string SubtotalPrice { get; set; }
        public string totalPrice { get; set; }
        public string Date { get; set; }
        public string Avatar { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string OriginalPrice { get; set; }

        public int Quantity { get; set; }

        public List<ConsumerOrderDto> Consumers { get; set; }

    }
    public class ProductInOrderDto
    {
        public int ProductId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string OriginalPrice { get; set; }

        public int? Quantity { get; set; }
        public int StoreId { get; set; }
        public int KindId { get; set; }
        public string Amount { get; set; }
        public decimal AmountValue { get; set; }
        public List<ConsumerOrderDto> Consumers { get; set; }

    }

    public class ConsumerOrderDto
    {
        public int? Quantity { get; set; }
        public string FullName { get; set; }

    }

    public class UploadAvatarRequest
    {
        public IFormFile File { get; set; }
        public int Id { get; set; }

    }
    public class FilterRequest
    {
        public int KindId { get; set; }
        public int StoreId { get; set; }
        public string LangId { get; set; }
    }
}
