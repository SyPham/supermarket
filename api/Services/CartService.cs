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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Supermarket.Services
{
    public interface ICartService : IServiceBase<Cart, CartDto>
    {
        Task<OperationResult> UpdateQuantity(UpdateQuantityRequest request);
        Task<OperationResult> AddToCart(AddToCartRequest request);
        Task<OperationResult> DeleteCart(DeleteCartRequest request);
        Task<OperationResult> ClearCart();

        Task<object> GetProductsInCart(string langId);
        Task<int> CartTotal();
    }
    public class CartService : ServiceBase<Cart, CartDto>, ICartService
    {
        private readonly IRepositoryBase<Cart> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private OperationResult operationResult;

        public CartService(
            IRepositoryBase<Cart> repo,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            MapperConfiguration configMapper
            )
            : base(repo, unitOfWork, mapper, configMapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configMapper = configMapper;
        }

        public async Task<object> GetProductsInCart(string langId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();

            return await _repo.FindAll(x => x.AccountId == accountId).Select(x => new ProductCartDto
            {
                ProductId = x.ProductId,
                AccountId = x.AccountId,
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice.ToString("n0"),
                Quantity = x.Quantity,
                Avatar = x.Product.Avatar,
                Description = x.Product.Description,
                Amount = (x.Quantity.Value * x.Product.OriginalPrice).ToString("n0"),
                AmountValue = (x.Quantity.Value * x.Product.OriginalPrice)
            }).ToListAsync();
        }

        public async Task<OperationResult> UpdateQuantity(UpdateQuantityRequest request)
        {
            var item = await _repo.FindAll(x => x.ProductId == request.ProductId && x.AccountId == request.AccountId).FirstOrDefaultAsync();
            if (item == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Not Found", Success = false };
            }
            item.Quantity = request.Quantity;
            try
            {
                _repo.Update(item);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.UpdateSuccess,
                    Success = true,
                    Data = item
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }
        public async Task<OperationResult> AddToCart(AddToCartRequest request)
        {
            var item = await _repo.FindAll(x => x.ProductId == request.ProductId && x.AccountId == request.AccountId).FirstOrDefaultAsync();
            if (item == null)
            {
                var add = _mapper.Map<Cart>(request);
                add.CreatedTime = DateTime.Now;
                try
                {
                    _repo.Add(add);
                    await _unitOfWork.SaveChangeAsync();
                    operationResult = new OperationResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = MessageReponse.AddSuccess,
                        Success = true,
                        Data = item
                    };
                }
                catch (Exception ex)
                {
                    operationResult = ex.GetMessageError();
                }
                return operationResult;
            }
            else
            {
                item.Quantity = request.Quantity;
                try
                {
                    _repo.Update(item);
                    await _unitOfWork.SaveChangeAsync();
                    operationResult = new OperationResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = MessageReponse.UpdateSuccess,
                        Success = true,
                        Data = item
                    };
                }
                catch (Exception ex)
                {
                    operationResult = ex.GetMessageError();
                }
                return operationResult;
            }

        }

        public async Task<int> CartTotal()
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();

            var item = await _repo.FindAll(x => x.AccountId == accountId).SumAsync(x => x.Quantity);
            return item ?? 0;
        }

        public async Task<OperationResult> DeleteCart(DeleteCartRequest request)
        {
            var item = await _repo.FindAll(x => x.ProductId == request.ProductId && x.AccountId == request.AccountId).FirstOrDefaultAsync();
            if (item == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Not Found", Success = false };
            }
            try
            {
                _repo.Remove(item);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.DeleteSuccess,
                    Success = true,
                    Data = item
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }

        public async Task<OperationResult> ClearCart()
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();

            var items = await _repo.FindAll(x =>x.AccountId == accountId).ToListAsync();
            if (items.Count == 0)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Not Found", Success = false };
            }
            try
            {
                _repo.RemoveMultiple(items);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.DeleteSuccess,
                    Success = true,
                    Data = items
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }
    }
}
