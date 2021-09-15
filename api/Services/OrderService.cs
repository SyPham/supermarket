using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetUtility;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
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
        Task<object> GetProductsForCartStatusByCompleteStatus(string langId);
        Task<object> GetProductsForCartStatusByBuyingAndPenidngStatus(string langId);

        Task<object> GetProductsInOrder(string langId);
        Task<object> GetProductsInOrderByAdmin(string langId);
        Task<bool> Transfer(List<AddToBuyListDto> model);
        Task<bool> CancelBuyList(List<AddToBuyListDto> model);
        Task<byte[]> ReportBuyPersion(string langId , int teamId);
        Task<byte[]> ReportBuyItemPending(string langId , int teamId);
        Task<byte[]> ReportBuyItem(string langId , int teamId);

        Task<bool> CancelPendingList(List<AddToBuyListDto> model);
        Task<bool> TransferComplete(List<AddToCompleteListDto> model);
        Task<object> GetProductsInOrderPendingByAdmin(string langId , int teamId);
        Task<object> GetUserDelevery(string langId, DateTime startDate, DateTime endDate);
        Task<object> GetProductsInOrderBuyingByAdmin(string langId, int teamId);
        Task<object> GetProductsInOrderCompleteByAdmin(string langId, int teamId, DateTime startDate, DateTime endDate);
        Task<object> GetProductsForCartStatus(string langId);
        Task<object> GetBuyingBuyPerson(string langId);

        Task<OperationResult> PlaceOrder();

        Task<OperationResult> UpdateQuantity(UpdateQuantityOrderRequest request);
        Task<OperationResult> DeleteCart(DeleteCartOrderRequest request);
        Task<OperationResult> ClearCart();

        Task<object> GetProductsInCart(string langId);
    }
    public class OrderService : ServiceBase<Order, OrderDto>, IOrderService
    {
        private OperationResult operationResult;
        private readonly IRepositoryBase<Account> _repoAccount;
        private readonly IRepositoryBase<Order> _repo;
        private readonly IRepositoryBase<OrderDetailHistory> _repoOrderHistory;
        private readonly IRepositoryBase<Cart> _repoCart;
        private readonly IRepositoryBase<OrderDetail> _repoDetail;
        private readonly IRepositoryBase<Product> _repoProduct;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public OrderService(
            IRepositoryBase<Account> repoAccount,
            IRepositoryBase<Order> repo,
            IRepositoryBase<OrderDetailHistory> repohistory,
            IRepositoryBase<OrderDetail> repoDetail,
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
            _repoDetail = repoDetail;
            _repoOrderHistory = repohistory;
            _repoProduct = repoProduct;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configMapper = configMapper;
        }
        private Bitmap Base64StringToBitmap(string base64String)
        {
            var bitmapData = Convert.FromBase64String(FixBase64ForImage(base64String));
            var streamBitmap = new System.IO.MemoryStream(bitmapData);
            var bitmap = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
            return bitmap;
        }

        private string FixBase64ForImage(string Image)
        {
            var sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty);
            sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
        public async Task<byte[]> ReportBuyPersion(string langId, int teamId)
        {
            var res = await GetBuyingBuyPersonExcel(langId, teamId);
            return ReportBuyPersion(res);
            //throw new NotImplementedException();
        }
        public async Task<byte[]> ReportBuyItem(string langId, int teamId)
        {
            var res = await GetBuyingBuyItemExcel(langId, teamId);
            return ReportBuyItem(res);
            //throw new NotImplementedException();
        }
        public async Task<byte[]> ReportBuyItemPending(string langId, int teamId)
        {
            var res = await GetPendingBuyItemExcel(langId, teamId);
            return ReportBuyItem(res);
            //throw new NotImplementedException();
        }
        public async Task<List<ProductBuyingDto>> GetPendingBuyItemExcel(string langId, int teamId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll(x => x.TeamId == teamId).ToListAsync();
            if (data == null) return new List<ProductBuyingDto>();
            //{
            //    TotalPrice = 0,
            //    Data = new List<ProductCartDto> { }
            //};v
            var res = data.Where(x => x.PendingQty > 0).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.ByingQty,
                Description = x.Product.Description,
                IsDelete = x.Product.IsDelete,
                Amount = (x.ByingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                Avatar = ConvertImageURLToBase64ExportEcel(host + x.Product.Avatar) ?? "",
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                ConsumerId = x.Consumer.Id,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).Where(y => y.IsDelete == false).ToList();
            var item = new List<ProductBuyingDto>();

            var result = res.GroupBy(x => new { x.Name })
                .Select(x => new ProductBuyingDto
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Description = x.First().Description,
                    StoreName = x.First().StoreName,
                    Avatar = x.First().Avatar,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Consumers = x.GroupBy(s => new { s.FullName, s.ConsumerId })
                    .Select(a => new ConsumerOrderDto
                    {
                        FullName = a.First().FullName,
                        Quantity = a.Sum(b => b.Quantity),
                    }).ToList(),

                }).ToList();
            return result;
        }
        public async Task<List<ProductBuyingDto>> GetBuyingBuyItemExcel(string langId, int teamId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll(x => x.TeamId == teamId).ToListAsync();
            if (data == null) return new List<ProductBuyingDto>();
            //{
            //    TotalPrice = 0,
            //    Data = new List<ProductCartDto> { }
            //};v
            var res = data.Where(x => x.ByingQty > 0).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.ByingQty,
                Description = x.Product.Description,
                IsDelete = x.Product.IsDelete,
                Amount = (x.ByingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                Avatar = ConvertImageURLToBase64ExportEcel(host + x.Product.Avatar) ?? "",
                KindId = x.Product.KindId,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                ConsumerId = x.Consumer.Id,
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).Where(y => y.IsDelete == false).ToList();
            var item = new List<ProductBuyingDto>();

            var result = res.GroupBy(x => new { x.Name })
                .Select(x => new ProductBuyingDto
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    Description = x.First().Description,
                    StoreName = x.First().StoreName,
                    Avatar = x.First().Avatar,
                    OderDetailId = x.First().OderDetailId,
                    KindName = x.First().KindName,
                    Amount = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Consumers = x.GroupBy(s => new { s.FullName, s.ConsumerId })
                    .Select(a => new ConsumerOrderDto
                    {
                        FullName = a.First().FullName,
                        Quantity = a.Sum(b => b.Quantity),
                    }).ToList(),

                }).ToList();
            return result;
        }
        private Byte[] ReportBuyItem(List<ProductBuyingDto> list)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var memoryStream = new MemoryStream();
                using (ExcelPackage p = new ExcelPackage(memoryStream))
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "Leo";
                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "ReportBuyPersion";
                    //Tạo một sheet để làm việc trên đó
                    var test = list.GroupBy(y => y.StoreName).ToList();
                    foreach (var items in test)
                    {
                        p.Workbook.Worksheets.Add(items.Key);
                        // lấy sheet vừa add ra để thao tác
                        ExcelWorksheet ws = p.Workbook.Worksheets[items.Key];
                        ws.DefaultColWidth = 25;

                        // đặt tên cho sheet
                        ws.Name = items.Key;
                        // fontsize mặc định cho cả sheet
                        ws.Cells.Style.Font.Size = 12;
                        // font family mặc định cho cả sheet
                        ws.Cells.Style.Font.Name = "Calibri";
                        var headers = new string[]{
                        "Kind","Photo", "Product Name", "Unit", "Ref. Price", "Qty",
                        "Amount", "Consumers"
                    };

                        int headerRowIndex = 1;
                        int headerColIndex = 1;
                        foreach (var header in headers)
                        {
                            int col = headerRowIndex++;
                            ws.Cells[headerColIndex, col].Value = header;
                            ws.Cells[headerColIndex, col].Style.Font.Bold = true;
                            ws.Cells[headerColIndex, col].Style.Font.Size = 12;
                            if (col == 3)
                            {
                                ws.Column(col).Width = 25;
                            }
                            else
                            {
                                ws.Column(col).AutoFit();
                            }
                        }

                        // end Style
                        int colIndex = 1;
                        int rowIndex = 1;
                        // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                        foreach (var item in items.OrderBy(x => x.KindName))
                        {
                            colIndex = 1;
                            var base64String = item.Avatar;
                            base64String = base64String.Replace(" ", "+");
                            int mod4 = base64String.Length % 4;
                            if (mod4 > 0)
                            {
                                base64String += new string('=', 4 - mod4);
                            }

                            var image = Base64StringToBitmap(base64String);
                            ws.Row(rowIndex + 1).Height = 100;
                            // rowIndex tương ứng từng dòng dữ liệu
                            rowIndex++;
                            string value = "";
                            foreach (var itemss in item.Consumers)
                            {
                                value += itemss.FullName +":" + itemss.Quantity + "," + "\n";
                            }

                            //gán giá trị cho từng cell                      
                            ws.Cells[rowIndex, colIndex++].Value = item.KindName;
                            var picture = ws.Drawings.AddPicture(rowIndex.ToString(), image);
                            //picture.From.Column = (colIndex++) - 1;
                            //picture.From.Row = rowIndex - 1;
                            picture.SetPosition(rowIndex - 1, 20, (colIndex++) - 1, 30);
                            picture.SetSize(150, 100);

                            ws.Cells[rowIndex, colIndex++].Value = item.Name;
                            ws.Cells[rowIndex, colIndex++].Value = item.Description;
                            ws.Cells[rowIndex, colIndex++].Value = item.OriginalPrice;
                            ws.Cells[rowIndex, colIndex++].Value = item.Quantity;
                            ws.Cells[rowIndex, colIndex++].Value = item.Amount;
                            ws.Cells[rowIndex, colIndex++].Value = value.Substring(0, value.LastIndexOf(","));
                            value = "";

                        }

                        int mergeFromColIndex = 1;
                        int mergeToColIndex = 1;
                        int mergeFromRowIndex = 2;
                        int mergeToRowIndex = 1;
                        foreach (var item in items.OrderBy(x => x.KindName).GroupBy(x => x.KindName).ToList())
                        {
                            mergeToRowIndex += item.Count();


                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Merge = true;
                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            mergeFromRowIndex = mergeToRowIndex + 1;
                        }

                        //make the borders of cell F6 thick
                        ws.Cells[ws.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        foreach (var item in headers.Select((x, i) => new { Value = x, Index = i }))
                        {
                            var col = item.Index + 1;
                            ws.Column(col).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Column(col).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            if (col == 2 || col == 8 || col == 3)
                            {
                                ws.Column(col).AutoFit(30);
                                ws.Column(col).Style.WrapText = true;
                            }
                            else
                            {
                                ws.Column(col).AutoFit();
                            }
                        }
                    }


                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    return bin;
                }
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                Console.Write(mes);
                return new Byte[] { };
            }
        }
        private Byte[] ReportBuyPersion(List<ProductBuyingDto> list)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var memoryStream = new MemoryStream();
                using (ExcelPackage p = new ExcelPackage(memoryStream))
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "Leo";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "ReportBuyPersion";
                    //Tạo một sheet để làm việc trên đó
                    var test = list.GroupBy(x => new
                    {
                        x.ConsumerId,
                        x.FullName,
                        x.StoreName,
                        x.totalPrice
                    }).GroupBy(y => y.First().StoreName).ToList();
                    foreach (var items in test)
                    {
                        p.Workbook.Worksheets.Add(items.Key);
                        // lấy sheet vừa add ra để thao tác
                        ExcelWorksheet ws = p.Workbook.Worksheets[items.Key];

                        // đặt tên cho sheet
                        ws.Name = items.Key;
                        // fontsize mặc định cho cả sheet
                        ws.Cells.Style.Font.Size = 12;
                        // font family mặc định cho cả sheet
                        ws.Cells.Style.Font.Name = "Calibri";
                        var headers = new string[]{
                        "EmployeeID","Name", "Product Name", "Qty", "Price",
                        "Subtotal", "Total"
                    };

                        int headerRowIndex = 1;
                        int headerColIndex = 1;
                        foreach (var header in headers)
                        {
                            int col = headerRowIndex++;
                            ws.Cells[headerColIndex, col].Value = header;
                            ws.Cells[headerColIndex, col].Style.Font.Bold = true;
                            ws.Cells[headerColIndex, col].Style.Font.Size = 12;
                        }

                        // end Style
                        int colIndex = 1;
                        int rowIndex = 1;
                        // với mỗi item trong danh sách sẽ ghi trên 1 dòng


                        int mergeFromColIndex = 1;
                        int mergeToColIndex = 1;
                        int mergeFromRowIndex = 2;
                        int mergeToRowIndex = 1;


                        foreach (var item in items)
                        {
                            foreach (var itemss in item)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0 #c0514d
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;


                                //gán giá trị cho từng cell                      
                                ws.Cells[rowIndex, colIndex++].Value = itemss.EmployeeId;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.FullName;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.Name;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.Quantity;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.OriginalPrice;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.SubtotalPrice;
                                ws.Cells[rowIndex, colIndex++].Value = itemss.totalPrice;

                            }
                            mergeToRowIndex += item.Count();
                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Merge = true;
                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[mergeFromRowIndex, mergeFromColIndex, mergeToRowIndex, mergeToColIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells[mergeFromRowIndex, 2, mergeToRowIndex, 2].Merge = true;
                            ws.Cells[mergeFromRowIndex, 2, mergeToRowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[mergeFromRowIndex, 2, mergeToRowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells[mergeFromRowIndex, 7, mergeToRowIndex, 7].Merge = true;
                            ws.Cells[mergeFromRowIndex, 7, mergeToRowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[mergeFromRowIndex, 7, mergeToRowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                            mergeFromRowIndex = mergeToRowIndex + 1;
                            //foreach (var itemss in item)
                            //{
                            //}
                        }
                        //make the borders of cell F6 thick
                        ws.Cells[ws.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells[ws.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        foreach (var item in headers.Select((x, i) => new { Value = x, Index = i }))
                        {
                            var col = item.Index + 1;
                            ws.Column(col).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Column(col).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            if (col == 2 || col == 7)
                            {
                                ws.Column(col).AutoFit(30);
                            }
                            else
                            {
                                ws.Column(col).AutoFit();
                            }
                        }
                    }


                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    return bin;
                }
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                Console.Write(mes);
                return new Byte[] { };
            }
        }
        public async Task<List<ProductBuyingDto>> GetBuyingBuyPersonExcel(string langId, int teamId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            var data = await _repoOrderHistory.FindAll().Where(x => x.TeamId == teamId).ToListAsync();
            if (data == null) return new List<ProductBuyingDto>();
            //{
            //    TotalPrice = 0,
            //    Data = new List<ProductCartDto> { }
            //};v
            var res = data.Where(x => x.ByingQty > 0).Select(x => new
            {
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice,
                Quantity = x.ByingQty,
                //Avatar = ConvertImageURLToBase64(host + x.Product.Avatar),
                Description = x.Product.Description,
                Amount = (x.ByingQty * x.Product.OriginalPrice),
                StoreId = x.Product.StoreId,
                KindId = x.Product.KindId,
                IsDelete = x.Product.IsDelete,
                ProductId = x.ProductId,
                OderDetailId = x.OrderDetailId,
                EmployeeId = x.Consumer.EmployeeId,
                StoreName = x.Product.Store.Name,
                FullName = x.Consumer.FullName,
                Date = x.DispatchDate,
                ConsumerId = x.Consumer.Id,
                totalPrice = _repoOrderHistory.FindAll().Where(y => y.ConsumerId == x.Consumer.Id && y.ByingQty > 0).ToList().Select(x => (x.ByingQty * x.Product.OriginalPrice)).Sum(),
                KindName = langId == SystemLang.VI ? x.Product.Kind.VietnameseName : langId == SystemLang.EN ? x.Product.Kind.EnglishName : x.Product.Kind.ChineseName,
            }).Where(y => y.IsDelete == false).ToList();
            var item = new List<ProductBuyingDto>();

            var result = res.GroupBy(x => new { x.Name, x.ConsumerId })
                .Select(x => new ProductBuyingDto
                {
                    Name = x.First().Name,
                    OriginalPrice = x.First().OriginalPrice.ToString("n0"),
                    //Avatar = x.First().Avatar,
                    Description = x.First().Description,
                    FullName = x.First().FullName,
                    ConsumerId = x.First().ConsumerId,
                    StoreName = x.First().StoreName,
                    OderDetailId = x.First().OderDetailId,
                    EmployeeId = x.First().EmployeeId,
                    KindName = x.First().KindName,
                    SubtotalPrice = x.Sum(a => a.Amount).ToString("n0"),
                    Quantity = x.Sum(a => a.Quantity),
                    Date = Convert.ToDateTime(x.First().Date).ToString("dd/MM/yy"),
                    totalPrice = x.First().totalPrice.ToString("n0")

                }).ToList();
            return result;
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
        public async Task<object> GetBuyingBuyPerson(string langId)
        {
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
                totalPrice = _repoOrderHistory.FindAll().Where(y => y.ConsumerId == x.Consumer.Id && y.ByingQty > 0).ToList().Select(x => (x.ByingQty * x.Product.OriginalPrice)).Sum(),
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
            // user demand: If user place order the same product, the same date, the same team then just update the [quantity]  and [order time] in card page/pending tab . No need to insert another record in db
            var order = await _repo.FindAll()
                .FirstOrDefaultAsync(x => x.CreatedBy == accountId && x.ConsumerId == accountItem.ConsumerId && x.CreatedTime.Date == DateTime.Now.Date);
           if (order == null)
            {
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
                    itemDetail.TeamId = x.TeamId;
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
                        DispatchDate = DateTime.MinValue,
                        OrderDate = orderItem.CreatedTime,
                        TeamId = x.TeamId
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
            } else
            {
                var orderDetails = order.OrderDetails;
                order.CreatedTime = DateTime.Now;
                order.ModifiedTime = DateTime.Now;

                var adddetails = new List<OrderDetail>();
                // Tao moi nhung san pham chua co trong order details
                var newProducts = cartList.Where(x =>  orderDetails.Any(b=> b.ProductId == x.ProductId && x.TeamId == b.TeamId) == false).ToList();
                foreach (var item in newProducts)
                {
                        OrderDetail itemDetail = new OrderDetail();
                        itemDetail.OrderId = order.Id;
                        itemDetail.ProductId = item.ProductId;
                        itemDetail.Quantity = item.Quantity;
                        itemDetail.Price = (decimal?)item.Product.OriginalPrice;
                        itemDetail.TeamId = item.TeamId;
                    adddetails.Add(itemDetail);
                }

                var updateProducts = cartList.Where(x => orderDetails.Any(b => b.ProductId == x.ProductId && x.TeamId == b.TeamId) == true).ToList();

                var updateDetails = new List<OrderDetail>();
                foreach (var x in updateProducts)
                {
                    foreach (var itemDetail in orderDetails)
                    {
                        if (x.ProductId == itemDetail.ProductId && x.TeamId == itemDetail.TeamId)
                        {
                            itemDetail.Quantity += x.Quantity;
                            itemDetail.Price = (decimal?)x.Product.OriginalPrice;
                            itemDetail.TeamId = x.TeamId;
                            updateDetails.Add(itemDetail);
                        }
                    }
                 
                }
                try
                {
                    _repoDetail.UpdateRange(updateDetails);
                    _repoDetail.AddRange(adddetails);
                    _repo.Update(order);
                    await _unitOfWork.SaveChangeAsync();

                    var orderHistory = adddetails.Select(x => new OrderDetailHistory
                    {
                        Id = 0,
                        OrderDetailId = x.Id,
                        ProductId = x.ProductId,
                        PendingQty = x.Quantity.Value,
                        ByingQty = 0,
                        CompleteQty = 0,
                        ConsumerId = order.ConsumerId,
                        DispatchDate = DateTime.MinValue,
                        OrderDate = order.CreatedTime,
                        TeamId = x.TeamId
                    }).ToList();
                    _repoOrderHistory.AddRange(orderHistory);

                    var updateHistory = await _repoOrderHistory.FindAll(x => updateDetails.Select(a=>a.Id).Contains(x.OrderDetailId)).ToListAsync();
                    var updateHistories = new List<OrderDetailHistory>();
                    foreach (var x in updateDetails)
                    {
                        foreach (var itemDetail in updateHistory)
                        {
                            if (x.Id == itemDetail.OrderDetailId && x.TeamId == itemDetail.TeamId)
                            {
                                itemDetail.PendingQty = x.Quantity.Value;
                                updateHistories.Add(itemDetail);
                            }
                        }

                    }
                    _repoOrderHistory.UpdateRange(updateHistories);

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
            var data = await _repo.FindAll(x => x.ConsumerId == accountItem.ConsumerId.Value && x.CreatedTime.Date == DateTime.Now.Date).FirstOrDefaultAsync();
            if (data == null) return new
            {
                TotalPrice = 0,
                Data = new List<ProductCartDto> { }
            };
            var res = data.OrderDetails.OrderByDescending(x=>x.Id).Select(x => new ProductCartDto
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

        public async Task<object> GetProductsInOrderPendingByAdmin(string langId , int teamId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == teamId).FirstOrDefaultAsync();
            //if (accountItem == null) return new
            //{
            //    TotalPrice = 0,
            //    Data = new List<ProductCartDto> { }
            //};
            //var data = await _repo.FindAll().ToListAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().Where(x => x.TeamId == teamId).ToListAsync();
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
                Avatar = x.Product.Avatar,
                IsDelete = x.Product.IsDelete,
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
            }).Where(y => y.IsDelete == false).ToList();

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
        public async Task<object> GetProductsInOrderBuyingByAdmin(string langId, int teamId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().Where(x => x.TeamId == teamId).ToListAsync();
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
                Avatar = x.Product.Avatar,
                IsDelete = x.Product.IsDelete,
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
            }).Where(y => y.IsDelete == false).ToList();

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
        public async Task<object> GetProductsInOrderCompleteByAdmin(string langId, int teamId, DateTime startDate, DateTime endDate)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var accountItem = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" +
                    _httpContextAccessor.HttpContext.Request.Host + "/api/";
            var data = await _repoOrderHistory.FindAll().Where(x => x.TeamId == teamId).ToListAsync();
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
                Avatar = x.Product.Avatar,
                IsDelete = x.Product.IsDelete,
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
            }).Where(y => y.IsDelete == false).ToList();

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

            //_sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            _sb.Append("data:image/png;base64, " + Convert.ToBase64String(_byte, 0, _byte.Length));
            return _sb.ToString();
        }
        public String ConvertImageURLToBase64ExportEcel(String url)
        {
            StringBuilder _sb = new StringBuilder();

            Byte[] _byte = this.GetImage(url);

            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            //_sb.Append("data:image/png;base64, " + Convert.ToBase64String(_byte, 0, _byte.Length));
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

        public async Task<object> GetProductsForCartStatusByCompleteStatus(string langId)
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
                  x.Product.IsDelete,
                  x.Product.Description,
                  x.PendingQty,
                  x.ByingQty,
                  x.CompleteQty,
                  x.DispatchDate,
                  x.OrderDate,
                  x.CancelStatus,
                  OriginalPrice = x.Product.OriginalPrice,
                  Price = x.Product.OriginalPrice,
                  Quantity = x.PendingQty,
                  AmountValue = (x.CompleteQty * x.Product.OriginalPrice)
              }).ToListAsync();

            var complete = model.Where(x => x.CompleteQty > 0)
          .Select(x => new
          {
              x.FullName,
              x.StoreName,
              x.KindName,
              x.ProductName,
              x.Avatar,
              x.IsDelete,
              x.Description,
              x.PendingQty,
              x.DispatchDate,
              x.OrderDate,
              x.CancelStatus,
              OriginalPrice = x.OriginalPrice.ToString("n0"),
              Quantity = x.CompleteQty,
              Amount = (x.CompleteQty * x.OriginalPrice).ToString("n0"),
              Status = x.CancelStatus == true ? "Cancel" : "Complete",
              x.AmountValue
          }).Where(y => y.IsDelete == false).ToList();
            return complete;
        }

        public async Task<object> GetProductsForCartStatusByBuyingAndPenidngStatus(string langId)
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
                  x.Product.IsDelete,
                  x.Product.Description,
                  x.PendingQty,
                  x.ByingQty,
                  x.CompleteQty,
                  x.DispatchDate,
                  x.OrderDate,
                  OriginalPrice = x.Product.OriginalPrice,
                  Price = x.Product.OriginalPrice,
                  Quantity = x.PendingQty,
                  AmountValue = (x.ByingQty * x.Product.OriginalPrice)
              })
              .OrderByDescending(x=> x.DispatchDate)
              .ThenByDescending(x=>x.OrderDate)
              .ToListAsync();
            // var pending = model.Where(x => x.PendingQty > 0).Select(x => new
            // {
            //     x.FullName,
            //     x.StoreName,
            //     x.KindName,
            //     x.ProductName,
            //     x.Avatar,
            //     x.Description,
            //     x.PendingQty,
            //     x.DispatchDate,
            //     x.OrderDate,
            //     OriginalPrice = x.OriginalPrice.ToString("n0"),
            //     Quantity = x.PendingQty,
            //     Amount = (x.PendingQty * x.OriginalPrice).ToString("n0"),
            //     Status = "Pending"
            // }).ToList();
            var buying = model.Where(x => x.ByingQty > 0).Select(x => new
            {
                x.FullName,
                x.StoreName,
                x.KindName,
                x.ProductName,
                x.Avatar,
                x.Description,
                x.PendingQty,
                x.IsDelete,
                x.DispatchDate,
                x.OrderDate,
                OriginalPrice = x.OriginalPrice.ToString("n0"),
                Quantity = x.ByingQty,
                Amount = (x.ByingQty * x.OriginalPrice).ToString("n0"),
                Status = "Buying",
                x.AmountValue
            }).Where(y => y.IsDelete == false).ToList();

            // return pending.Concat(buying);
            return buying;
        }



        public async Task<OperationResult> UpdateQuantity(UpdateQuantityOrderRequest request)
        {
            var detailItem = await _repoDetail.FindAll(x => x.Id == request.DetailId).FirstOrDefaultAsync();
            var historyItem = await _repoOrderHistory.FindAll(x => x.Id == request.HistoryId).FirstOrDefaultAsync();
            if (detailItem == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Not Found", Success = false };
            }
            if (historyItem.ByingQty > 0 || historyItem.CompleteQty > 0)
            {
                historyItem.PendingQty = request.Quantity;
                detailItem.Quantity = request.Quantity + historyItem.ByingQty + historyItem.CompleteQty;
            }
            else
            {
                detailItem.Quantity = request.Quantity;
                historyItem.PendingQty = request.Quantity;
            }

            try
            {
                _repoDetail.Update(detailItem);
                _repoOrderHistory.Update(historyItem);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.UpdateSuccess,
                    Success = true,
                    Data = historyItem
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }

        public async Task<OperationResult> DeleteCart(DeleteCartOrderRequest request)
        {
            var detailItem = await _repoDetail.FindAll(x => x.Id == request.DetailId).FirstOrDefaultAsync();
            var historyItem = await _repoOrderHistory.FindAll(x => x.Id == request.HistoryId).FirstOrDefaultAsync();

            try
            {
                if (historyItem.ByingQty > 0 || historyItem.CompleteQty > 0)
                {
                    historyItem.PendingQty = 0;
                    detailItem.Quantity = historyItem.ByingQty + historyItem.CompleteQty;
                    _repoOrderHistory.Update(historyItem);
                    _repoDetail.Update(detailItem);
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    var orderId = detailItem.OrderId;
                    _repoOrderHistory.Remove(historyItem);
                    _repoDetail.Remove(detailItem);
                    await _unitOfWork.SaveChangeAsync();

                    var orderItem = await _repo.FindAll(x => x.Id == orderId).FirstOrDefaultAsync();
                    if (orderItem.OrderDetails.Count == 0)
                    {
                        _repo.Remove(orderItem);
                        await _unitOfWork.SaveChangeAsync();
                    }


                }


                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.DeleteSuccess,
                    Success = true,
                    Data = historyItem
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
            var data = await _repo.FindAll(x => x.CreatedBy == accountId).SelectMany(x => x.OrderDetails).Select(x => new
            {
                DetailId = x.Id,
                HistoryId = _repoOrderHistory.FindAll().FirstOrDefault(a => a.OrderDetailId == x.Id).Id,
                PendingQty = _repoOrderHistory.FindAll().FirstOrDefault(a => a.OrderDetailId == x.Id).PendingQty
            }).Where(x => x.PendingQty > 0).ToListAsync();
            var results = new List<OperationResult>();
            foreach (var item in data)
            {
                var detailItem = await _repoDetail.FindAll(x => x.Id == item.DetailId).FirstOrDefaultAsync();
                var historyItem = await _repoOrderHistory.FindAll(x => x.Id == item.HistoryId).FirstOrDefaultAsync();

                try
                {
                    if (historyItem.ByingQty > 0 || historyItem.CompleteQty > 0)
                    {
                        historyItem.PendingQty = 0;
                        detailItem.Quantity = historyItem.ByingQty + historyItem.CompleteQty;
                        _repoOrderHistory.Update(historyItem);
                        _repoDetail.Update(detailItem);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    else
                    {
                        var orderId = detailItem.OrderId;
                        _repoOrderHistory.Remove(historyItem);
                        _repoDetail.Remove(detailItem);
                        await _unitOfWork.SaveChangeAsync();

                        var orderItem = await _repo.FindAll(x => x.Id == orderId).FirstOrDefaultAsync();
                        if (orderItem.OrderDetails.Count == 0)
                        {
                            _repo.Remove(orderItem);
                            await _unitOfWork.SaveChangeAsync();
                        }

                    }

                    results.Add(new OperationResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = MessageReponse.DeleteSuccess,
                        Success = true,
                        Data = historyItem
                    });
                }
                catch (Exception ex)
                {
                    operationResult = ex.GetMessageError();
                }
                results.Add(operationResult);
            }
            return results.FirstOrDefault();
        }

        public async Task<object> GetProductsInCart(string langId)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();

            return await _repo.FindAll(x => x.CreatedBy == accountId)
                .SelectMany(x => x.OrderDetails)
                .Select(x => new
            {
                ProductId = x.ProductId,
                AccountId = x.Orders.CreatedBy.Value,
                Name = langId == SystemLang.VI ? x.Product.VietnameseName : langId == SystemLang.EN ? x.Product.EnglishName : x.Product.ChineseName,
                OriginalPrice = x.Product.OriginalPrice.ToString("n0"),
                Quantity = x.Quantity,
                Avatar = x.Product.Avatar,
                IsDelete = x.Product.IsDelete,
                Store = x.Product.Store.Name,
                Team = x.Team != null ? x.Team.Name : "N/A",
                Description = x.Product.Description,
                Amount = (x.Quantity.Value * x.Product.OriginalPrice).ToString("n0"),
                AmountValue = (x.Quantity.Value * x.Product.OriginalPrice),
                DetailId = x.Id,
                OrderDate = x.Orders.CreatedTime.ToString("MM/dd/yyyy HH:mm:ss"),
                HistoryId = _repoOrderHistory.FindAll().FirstOrDefault(a => a.OrderDetailId == x.Id).Id,
                    PendingQty = _repoOrderHistory.FindAll().FirstOrDefault(a => a.OrderDetailId == x.Id).PendingQty
            }).Where(x => x.PendingQty > 0 && x.IsDelete == false).OrderBy(x=>x.ProductId).ToListAsync();
        }

    }
}
