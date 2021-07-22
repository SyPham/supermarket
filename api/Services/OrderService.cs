using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetUtility;
using Supermarket.Constants;
using Supermarket.Data;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Models;
using Supermarket.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Supermarket.Services
{
    public interface IOrderService : IServiceBase<Order, OrderDto>
    {
        Task<object> GetProductsInOrder(string langId);
        Task<OperationResult> PlaceOrder();
    }
    public class OrderService : ServiceBase<Order, OrderDto>, IOrderService
    {
        private OperationResult operationResult;
        private readonly IRepositoryBase<Account> _repoAccount;
        private readonly IRepositoryBase<Order> _repo;
        private readonly IRepositoryBase<Cart> _repoCart;
        private readonly IRepositoryBase<Product> _repoProduct;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public OrderService(
            IRepositoryBase<Account> repoAccount,
            IRepositoryBase<Order> repo,
            IRepositoryBase<Cart> repoCart,
            IRepositoryBase<Product> repoProduct,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            MapperConfiguration configMapper
            )
            : base(repo, unitOfWork, mapper, configMapper)
        {
            _repoAccount = repoAccount;
            _repo = repo;
            _repoCart = repoCart;
            _repoProduct = repoProduct;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configMapper = configMapper;
        }

        public async Task<OperationResult> PlaceOrder()
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();

            var cartList = await _repoCart.FindAll(x => x.AccountId == accountId).ToListAsync();
            decimal totalPrice = 0;
            foreach (var item in cartList)
            {
                var price = item.Quantity.Value * item.Product.OriginalPrice;
                totalPrice += price;
            }
            var orderItem = new Order
            {
                Code = DateTime.Now.Millisecond.ToString() + accountId,
                FullName = accountItem.Consumer.FullName,
                EmployeeId = accountItem.Consumer.EmployeeId,
                ConsumerId = accountItem.ConsumerId.Value,
                Status = 1,
                CreatedBy = accountId,
                TotalPrice = totalPrice
            };
            orderItem.OrderDetails = new List<OrderDetail>();
            foreach (var x in cartList)
            {
                OrderDetail itemDetail = new OrderDetail();
                itemDetail.OrderId = orderItem.Id;
                itemDetail.ProductId = x.ProductId;
                itemDetail.Quantity = x.Quantity;
                itemDetail.Price = (decimal?)x.Product.OriginalPrice;
                orderItem.OrderDetails.Add(itemDetail);
            }
            try
            {

                _repo.Add(orderItem);
                await _unitOfWork.SaveChangeAsync();
                // Đặt hàng xong thì xóa giỏ hàng
                var cartRemove = await _repoCart.FindAll(x => x.AccountId == accountId).ToListAsync();
                _repoCart.RemoveMultiple(cartRemove);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.AddSuccess,
                    Success = true,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // Không thêm được thì xóa file vừa tạo đi
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }
        public async Task<object> GetProductsInOrder(string langId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            if (accountItem == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var data = await _repo.FindAll(x => x.ConsumerId == accountItem.ConsumerId.Value).ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.SelectMany(x=> x.OrderDetails).Select(x => new ProductCartDto
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice.ToString("n0"),
                Quantity = x.Quantity,
                Avatar = x.Product.Avatar,
                Description = x.Product.Description,
                Amount = (x.Quantity.Value * x.Product.OriginalPrice).ToString("n0")
            }).ToList();

            return new
            {
                TotalPrice = 0,
                Data = res
            };
        }

    }
}
