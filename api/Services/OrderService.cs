﻿using AutoMapper;
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Services
{
    public interface IOrderService : IServiceBase<Order, OrderDto>
    {
        Task<object> GetProductsInOrder(string langId);
        Task<object> GetProductsInOrderByAdmin(string langId);
        Task<bool> Transfer(List<AddToBuyListDto> model);
        Task<bool> CancelBuyList(List<AddToBuyListDto> model);
        Task<bool> CancelPendingList(List<AddToBuyListDto> model);
        Task<bool> TransferComplete(List<AddToCompleteListDto> model);
        Task<object> GetProductsInOrderPendingByAdmin(string langId);
        Task<object> GetUserDelevery(string langId, DateTime startDate, DateTime endDate);
        Task<object> GetProductsInOrderBuyingByAdmin(string langId);
        Task<object> GetProductsInOrderCompleteByAdmin(string langId, DateTime startDate, DateTime endDate);
        Task<object> GetProductsForCartStatus(string langId);
        Task<object> GetBuyingBuyPerson(string langId);

        Task<OperationResult> PlaceOrder();
    }
    public class OrderService : ServiceBase<Order, OrderDto>, IOrderService
    {
        private OperationResult operationResult;
        private readonly IRepositoryBase<Account> _repoAccount;
        private readonly IRepositoryBase<Order> _repo;
        private readonly IRepositoryBase<OrderDetailHistory> _repoOrderHistory;
        private readonly IRepositoryBase<Cart> _repoCart;
        private readonly IRepositoryBase<Product> _repoProduct;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public OrderService(
            IRepositoryBase<Account> repoAccount,
            IRepositoryBase<Order> repo,
            IRepositoryBase<OrderDetailHistory> repohistory,
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
            _repoOrderHistory = repohistory;
            _repoProduct = repoProduct;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configMapper = configMapper;
        }
        public async Task<bool> CancelBuyList(List<AddToBuyListDto> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var data = _repoOrderHistory.FindAll().Where(x => x.ProductId == item.ProductId && x.ConsumerId == item.ConsumerId).ToList();
                    foreach (var transfer in data)
                    {
                        transfer.CompleteQty = transfer.ByingQty;
                        transfer.ByingQty = 0;
                        transfer.CancelStatus = true;
                        transfer.DispatchDate = DateTime.Now;
                        _repoOrderHistory.Update(transfer);

                    }
                }
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
        }
        public async Task<bool> CancelPendingList(List<AddToBuyListDto> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var data = _repoOrderHistory.FindAll().Where(x => x.ProductId == item.ProductId && x.ConsumerId == item.ConsumerId).ToList();
                    foreach (var transfer in data)
                    {
                        transfer.CompleteQty = transfer.PendingQty;
                        transfer.PendingQty = 0;
                        transfer.CancelStatus = true;
                        transfer.DispatchDate = DateTime.Now;
                        _repoOrderHistory.Update(transfer);

                    }
                }
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<object> GetUserDelevery(string langId, DateTime startDate, DateTime endDate)
        {
            var data = await _repoOrderHistory.FindAll().ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.Where(x => x.CompleteQty > 0 && x.DispatchDate.Date >= startDate.Date && x.DispatchDate.Date <= endDate.Date).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.CompleteQty,
                //Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.CompleteQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                Date = x.DispatchDate,
                ConsumerId = x.Consumer.Id,
                totalPrice = _repoOrderHistory.FindAll().Where(y => y.ConsumerId == x.Consumer.Id && y.CompleteQty > 0 && x.DispatchDate.Date >= startDate.Date && x.DispatchDate.Date <= endDate.Date).ToList().Select(x => (x.CompleteQty * x.Product.OriginalPrice)).Sum(),
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.Name, x.ConsumerId })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    //Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    FullName = x.First().FullName,
                    ConsumerId = x.First().ConsumerId,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    SubtotalPrice = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Date = Convert.ToDateTime(x.First().Date).ToString("dd/MM/yy"),
                    totalPrice  = x.First().totalPrice.ToString("n0")

                }) ;
            var totalPrice = res.Sum(x => x.Amount).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };
            throw new NotImplementedException();
        }
        public async Task<object> GetBuyingBuyPerson(string langId)
        {
            var data = await _repoOrderHistory.FindAll().ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.Where(x => x.ByingQty > 0 ).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.ByingQty,
                //Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.ByingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                Date = x.DispatchDate,
                ConsumerId = x.Consumer.Id,
                totalPrice = _repoOrderHistory.FindAll().Where(y => y.ConsumerId == x.Consumer.Id && y.ByingQty > 0 ).ToList().Select(x => (x.ByingQty * x.Product.OriginalPrice)).Sum(),
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.Name, x.ConsumerId })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    //Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    FullName = x.First().FullName,
                    ConsumerId = x.First().ConsumerId,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    SubtotalPrice = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Date = Convert.ToDateTime(x.First().Date).ToString("dd/MM/yy"),
                    totalPrice = x.First().totalPrice.ToString("n0")

                });
            var totalPrice = res.Sum(x => x.Amount).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };
            throw new NotImplementedException();
        }
        public async Task<bool> TransferComplete(List<AddToCompleteListDto> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var total = item.QtyTamp;
                    var data = _repoOrderHistory.FindAll().Where(x => x.ProductId == item.ProductId && x.ConsumerId == item.ConsumerId && x.ByingQty > 0).ToList();
                    foreach (var transfer in data)
                    {
                        if (total == 0)
                            break;
                        if (transfer.ByingQty == 0)
                            break;
                        if (transfer.ByingQty < total)
                        {
                            transfer.CompleteQty = transfer.CompleteQty + transfer.ByingQty;
                            transfer.ByingQty = 0;
                            transfer.DispatchDate = DateTime.Now;
                            total = total - transfer.CompleteQty;
                        }
                        else
                        {
                            transfer.ByingQty = transfer.ByingQty - total;
                            transfer.CompleteQty = transfer.CompleteQty + total;
                            transfer.DispatchDate = DateTime.Now;
                            total = 0;
                        }
                        _repoOrderHistory.Update(transfer);

                    }
                }
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
        }
        public async Task<bool> Transfer(List<AddToBuyListDto> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var data = _repoOrderHistory.FindAll().Where(x => x.ProductId == item.ProductId && x.ConsumerId == item.ConsumerId).ToList();
                    foreach (var transfer in data)
                    {
                        transfer.ByingQty = transfer.PendingQty;
                        transfer.PendingQty = 0;
                        _repoOrderHistory.Update(transfer);

                    }
                }
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
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

                var orderHistory = orderItem.OrderDetails.Select(x => new OrderDetailHistory
                {
                    Id = 0,
                    OrderDetailId = x.Id,
                    ProductId = x.ProductId,
                    PendingQty = x.Quantity.Value,
                    ByingQty = 0,
                    CompleteQty = 0,
                    ConsumerId = orderItem.ConsumerId,
                    OrderDate = orderItem.CreatedTime
                }).ToList();
                _repoOrderHistory.AddRange(orderHistory);
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
            var data = await _repo.FindAll(x => x.ConsumerId == accountItem.ConsumerId.Value).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.OrderDetails.Select(x => new ProductCartDto
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice.ToString("n0"),
                Quantity = x.Quantity,
                Avatar = x.Product.Avatar,
                Description = x.Product.Description,
                Amount = (x.Quantity.Value * x.Product.OriginalPrice).ToString("n0")
            }).ToList();

            var totalPrice = data.TotalPrice.Value.ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = res
            };
        }
        public async Task<object> GetProductsInOrderByAdmin(string langId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            if (accountItem == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var data = await _repo.FindAll().ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.SelectMany(x => x.OrderDetails).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.Quantity,
                Avatar = x.Product.Avatar,
                Description = x.Product.Description,
                Amount = (x.Quantity.Value * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                StoreName = x.Product.Store.Name,
                FullName = x.Orders.Consumer.FullName,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.ProductId, x.StoreId, x.KindId })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    StoreName = x.First().StoreName,
                    KindName = x.First().KindName,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Consumers = x.GroupBy(s => s.FullName).Select(a => new
                    {
                        FullName = a.Key,
                        Quantity = a.Sum(b => b.Quantity)
                    }),
                });
            var totalPrice = result.Sum(x => x.Amount.ToInt()).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };
        }

        public async Task<object> GetProductsInOrderPendingByAdmin(string langId)
        {
            //string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            //var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            //if (accountItem == null) return new
            //{
            //    TotalPrice = 0,
            //    Data = new List<ProductCartDto> { }
            //};
            //var data = await _repo.FindAll().ToListAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().ToListAsync() ;
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.Where(x => x.PendingQty > 0).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.PendingQty,
                Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.PendingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                ConsumerId = x.Consumer.Id,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.Name })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Consumers = x.GroupBy(s => new { s.FullName, s.ConsumerId}).Select(a => new
                    {
                        FullName = a.First().FullName,
                        ProductId = a.First().ProductId,
                        Quantity = a.Sum(b => b.Quantity),
                        ConsumerId = a.Key.ConsumerId
                    }),
                });
            var totalPrice = res.Sum(x => x.Amount).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };
        }
        public async Task<object> GetProductsInOrderBuyingByAdmin(string langId)
        {
            string host = _httpContextAccessor.HttpContext.Request.Scheme +"://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.Where(x => x.ByingQty > 0).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.ByingQty,
                Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.ByingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                ConsumerId = x.Consumer.Id,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.Name })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Consumers = x.GroupBy(s => new { s.FullName, s.ConsumerId }).Select(a => new
                    {
                        FullName = a.First().FullName,
                        Name = a.First().Name,
                        ProductId = a.First().ProductId,
                        Quantity = a.Sum(b => b.Quantity),
                        ConsumerId = a.Key.ConsumerId,
                        QtyTamp = 0
                    }),
                });
            var totalPrice = res.Sum(x => x.Amount).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };
        }
        public async Task<object> GetProductsInOrderCompleteByAdmin(string langId, DateTime startDate, DateTime endDate)
        {
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().ToListAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.Where(x => x.CompleteQty > 0 && x.DispatchDate.Date >= startDate.Date && x.DispatchDate.Date <= endDate.Date).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.CompleteQty,
                Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.CompleteQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                Date = x.DispatchDate,
                CancelStatus = x.CancelStatus,
                ConsumerId = x.Consumer.Id,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).ToList();

            var result = res.GroupBy(x => new { x.Name, x.ConsumerId })
                .Select(x => new
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    FullName = x.First().FullName,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    CancelStatus = x.First().CancelStatus,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Date = Convert.ToDateTime(x.First().Date).ToString("MM/yy")
                });
            var totalPrice = res.Sum(x => x.Amount).ToString("n0");
            return new
            {
                TotalPrice = totalPrice,
                Data = result
            };

        }

        public async Task<object> GetProductsForCartStatus(string langId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            var data = await _repo.FindAll().ToListAsync();
            var model = await _repoOrderHistory.FindAll(x => accountItem.ConsumerId == x.ConsumerId)
              .Select(x => new
              {
                  x.Consumer.FullName,
                  StoreName = x.Product.Store.Name,
                  KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
                  ProductName = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                  x.Product.Avatar,
                  x.Product.Description,
                  x.PendingQty,
                  x.ByingQty,
                  x.CompleteQty,
                  x.DispatchDate,
                  x.OrderDate,
                  OriginalPrice = x.Product.OriginalPrice,
                  Quantity = x.PendingQty
              }).ToListAsync();
            var pending = model.Where(x => x.PendingQty > 0).Select(x => new
                {
                    x.FullName,
                    x.StoreName,
                    x.KindName,
                    x.ProductName,
                    x.Avatar,
                    x.Description,
                    x.PendingQty,
                    x.DispatchDate,
                    x.OrderDate,
                    x.OriginalPrice,
                    Quantity = x.PendingQty,
                    Amount = (x.PendingQty * x.OriginalPrice).ToString("n0"),
                    Status = "Pending"
                }).ToList();
            var buying = model.Where(x => x.ByingQty > 0).Select(x => new
              {
                  x.FullName,
                  x.StoreName,
                  x.KindName,
                  x.ProductName,
                  x.Avatar,
                  x.Description,
                  x.PendingQty,
                  x.DispatchDate,
                  x.OrderDate,
                  x.OriginalPrice,
                  Quantity = x.ByingQty,
                  Amount = (x.ByingQty * x.OriginalPrice).ToString("n0"),
                  Status = "Buying"
              }).ToList();
            var complete = model.Where(x => x.CompleteQty > 0)
          .Select(x => new
          {
              x.FullName,
              x.StoreName,
              x.KindName,
              x.ProductName,
              x.Avatar,
              x.Description,
              x.PendingQty,
              x.DispatchDate,
              x.OrderDate,
              x.OriginalPrice,
              Quantity = x.CompleteQty,
              Amount = (x.CompleteQty * x.OriginalPrice).ToString("n0"),
              Status = "Complete"
          }).ToList();
            return pending.Concat(buying).Concat(complete);
        }
        public String ConvertImageURLToBase64(String url)
        {
            StringBuilder _sb = new StringBuilder();

            Byte[] _byte = this.GetImage(url);

            _sb.Append("data:image/png;base64, " + Convert.ToBase64String(_byte, 0, _byte.Length));

            return _sb.ToString();
        }
        private byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }

        
    }
}
