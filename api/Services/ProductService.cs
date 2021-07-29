﻿using AutoMapper;
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
        Task<bool> UpdateStatus(int id);

    }
    public class ProductService : ServiceBase<Product, ProductDto>, IProductService
    {
        private OperationResult operationResult;

        private readonly IRepositoryBase<Product> _repo;
        private readonly IRepositoryBase<Account> _repoAccount;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _currentEnvironment;
        private readonly MapperConfiguration _configMapper;
        public ProductService(
            IRepositoryBase<Product> repo,
            IRepositoryBase<Account> repoAccount,
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
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _currentEnvironment = currentEnvironment;
            _configMapper = configMapper;
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
                return await _repo.FindAll()
                    .Where(x => x.StoreId == request.StoreId && x.KindId == request.KindId)
                    .Select(x => new ProductListDto
                    {
                        Id = x.Id,
                        Name = request.LangId == SystemLang.VI ? x.VietnameseName : request.LangId == SystemLang.EN ? x.EnglishName : x.ChineseName,
                        Quantity = x.Carts.Any(a => accountId == a.AccountId && a.ProductId == x.Id) ? x.Carts.FirstOrDefault(a => accountId == a.AccountId && a.ProductId == x.Id).Quantity : 0,
                        Avatar = x.Avatar,
                        OriginalPrice = x.OriginalPrice.ToString("n0"),
                        Description = x.Description
                    }).ToListAsync();
            if (request.StoreId > 0)
                return await _repo.FindAll().Where(x => x.StoreId == request.StoreId).Select(x => new ProductListDto
                {
                    Id = x.Id,
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
