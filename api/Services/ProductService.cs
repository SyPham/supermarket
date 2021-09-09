using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Supermarket.Services
{
    public interface IProductService : IServiceBase<Product, ProductDto>
    {
        Task<OperationResult> UploadAvatar(UploadAvatarRequest request);
        Task<object> GetProductsForConsumer(FilterRequest request);
        Task<object> GetProductsForAdmin(FilterRequest request);
        Task<bool> UpdateStatus(int id);

        Task ImportExcel(List<ProductDto> dto);

    }
    public class ProductService : ServiceBase<Product, ProductDto>, IProductService
    {
        private OperationResult operationResult;

        private readonly IRepositoryBase<Product> _repo;
        private readonly IRepositoryBase<Account> _repoAccount;
        private readonly IRepositoryBase<Store> _repoStore;
        private readonly IRepositoryBase<Kind> _repoKind;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _currentEnvironment;
        private readonly MapperConfiguration _configMapper;
        public ProductService(
            IRepositoryBase<Product> repo,
            IRepositoryBase<Account> repoAccount,
            IRepositoryBase<Store> repoStore,
            IRepositoryBase<Kind> repoKind,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment currentEnvironment,
            MapperConfiguration configMapper
            )
            : base(repo, unitOfWork, mapper, configMapper)
        {
            _repo = repo;
            _repoAccount = repoAccount;
            _repoStore = repoStore;
            _repoKind = repoKind;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _currentEnvironment = currentEnvironment;
            _configMapper = configMapper;
        }

        public async Task ImportExcel(List<ProductDto> dto)
        {
            try
            {
                var list = new List<ProductDto>();
                var listChuaAdd = new List<ProductDto>();
                var result = dto.DistinctBy(x => new
                {
                    x.VietnameseName,
                    x.ChineseName,
                    x.EnglishName
                }).Where(x => x.ChineseName != "").ToList();

                foreach (var item in result)
                {
                    var storeId = await _repoStore.FindAll().FirstOrDefaultAsync(x => x.Name.ToUpper().Equals(item.Store.ToUpper()));
                    var kindId = await _repoKind.FindAll().FirstOrDefaultAsync(x => x.VietnameseName.ToUpper().Equals(item.Kind.ToUpper()) || x.ChineseName.ToUpper().Equals(item.Kind.ToUpper()) || x.EnglishName.ToUpper().Equals(item.Kind.ToUpper()));
                    if (storeId != null && kindId != null)
                    {
                        item.StoreId = storeId.Id;
                        item.KindId = kindId.Id;
                        list.Add(item);
                    }
                    //if (kindId != null)
                    //{
                    //}
                    // var ink = await AddInk(item);
                }

                var listAdd = new List<Product>();
                foreach (var productItem in list)
                {
                    if (!await CheckExistProductName(productItem))
                    {
                        var pro = new Product();
                        pro.VietnameseName = productItem.VietnameseName;
                        pro.EnglishName = productItem.EnglishName;
                        pro.ChineseName = productItem.ChineseName;
                        pro.Description = productItem.Description;
                        pro.OriginalPrice = productItem.OriginalPrice;
                        pro.StoreId = productItem.StoreId;
                        pro.Status = productItem.Status;
                        pro.KindId = productItem.KindId;
                        pro.CreatedBy = productItem.CreatedBy;
                        pro.Avatar = $"image/default.png";
                        _repo.Add(pro);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private async Task<bool> CheckExistProductName(ProductDto pro)
        {
            return await _repo.FindAll().AnyAsync(x => x.VietnameseName.ToUpper().Equals(pro.VietnameseName.ToUpper()) && x.ChineseName.ToUpper().Equals(pro.ChineseName.ToUpper()) && x.EnglishName.ToUpper().Equals(pro.EnglishName.ToUpper()) && x.StoreId == pro.StoreId && x.KindId == pro.KindId);
        }
        public async Task<object> GetProductsForAdmin(FilterRequest request)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var item = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            if (item == null)
                return new List<ProductListDto>();
            if (request.StoreId == 0 && request.KindId == 0)
                return await _repo.FindAll().ToListAsync();
            if (request.StoreId > 0 && request.KindId > 0)
                return await _repo.FindAll().Where(x => x.StoreId == request.StoreId && x.KindId == request.KindId).ToListAsync();
            if (request.StoreId > 0)
                return await _repo.FindAll(x => x.Status).Where(x => x.StoreId == request.StoreId).ToListAsync();
            //return new List<ProductListDto>();
            throw new NotImplementedException();
        }
        public async Task<OperationResult> UploadAvatar(UploadAvatarRequest request)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var item = await _repo.FindAll(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (item == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Not Found!", Success = false };
            }

            FileExtension fileExtension = new FileExtension();

            // Nếu có đổi ảnh thì xóa ảnh cũ và thêm ảnh mới
            var avatarUniqueFileName = string.Empty;
            var avatarFolderPath = "FileUploads\\images\\product\\avatar";
            string uploadAvatarFolder = Path.Combine(_currentEnvironment.WebRootPath, avatarFolderPath);

            IFormFile filesAvatar = request.File;

            if (filesAvatar != null)
            {
                if (!item.Avatar.IsNullOrEmpty())
                    fileExtension.Remove($"{_currentEnvironment.WebRootPath}{item.Avatar.Replace("/", "\\").Replace("/", "\\")}");
                avatarUniqueFileName = await fileExtension.WriteAsync(filesAvatar, $"{uploadAvatarFolder}\\{avatarUniqueFileName}");
                item.Avatar = $"/FileUploads/images/product/avatar/{avatarUniqueFileName}";
            }

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
                // Nếu tạo ra file rồi mã lưu db bị lỗi thì xóa file vừa tạo đi
                if (!avatarUniqueFileName.IsNullOrEmpty())
                    fileExtension.Remove($"{uploadAvatarFolder}\\{avatarUniqueFileName}");

                // Không thêm được thì xóa file vừa tạo đi
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }

        public async Task<object> GetProductsForConsumer(FilterRequest request)
        {
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var accountId = JWTExtensions.GetDecodeTokenById(token).ToInt();
            var item = await _repoAccount.FindAll(x => x.Id == accountId).FirstOrDefaultAsync();
            if (item == null)
                return new List<ProductListDto>();
           
            if (request.StoreId > 0 && request.KindId > 0)
                return await _repo.FindAll(x => x.Status)
                    .Where(x => x.StoreId == request.StoreId && x.KindId == request.KindId)
                    .Select(x => new ProductListDto
                    {
                        Id = x.Id,
                        StoreName = x.Store.Name,
                        KindName = request.LangId == SystemLang.VI ? x.Kind.VietnameseName : request.LangId == SystemLang.EN ? x.Kind.EnglishName : x.Kind.ChineseName,
                        Name = request.LangId == SystemLang.VI ? x.VietnameseName : request.LangId == SystemLang.EN ? x.EnglishName : x.ChineseName,
                        Quantity = x.Carts.Any(a =>  accountId == a.AccountId && a.ProductId == x.Id) ? x.Carts.FirstOrDefault(a => accountId == a.AccountId && a.ProductId == x.Id).Quantity : 0,
                        Avatar = x.Avatar,
                        OriginalPrice = x.OriginalPrice.ToString("n0"),
                        Description = x.Description
                    }).ToListAsync();
            if (request.StoreId > 0)
                return await _repo.FindAll(x=> x.Status).Where(x => x.StoreId == request.StoreId).Select(x => new ProductListDto
                {
                    Id = x.Id,
                     StoreName = x.Store.Name,
                        KindName = request.LangId == SystemLang.VI ? x.Kind.VietnameseName : request.LangId == SystemLang.EN ? x.Kind.EnglishName : x.Kind.ChineseName,
                    Name = request.LangId == SystemLang.VI ? x.VietnameseName : request.LangId == SystemLang.EN ? x.EnglishName : x.ChineseName,
                    Quantity = x.Carts.Any(a => accountId == a.AccountId && a.ProductId == x.Id) ? x.Carts.FirstOrDefault(a => accountId == a.AccountId && a.ProductId == x.Id).Quantity : 0,
                    Avatar = x.Avatar,
                    OriginalPrice = x.OriginalPrice.ToString("n0"),
                    Description = x.Description
                }).ToListAsync();
            return new List<ProductListDto>();
        }

        public async Task<bool> UpdateStatus(int id)
        {
            try
            {
                var data = _repo.FindById(id);
                data.Status = !data.Status;
                _repo.Update(data);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }
    }
}
