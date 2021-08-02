using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
    public interface IAccountService : IServiceBase<Account, AccountDto>
    {
        Task<OperationResult> LockAsync(int id);
        Task<OperationResult> ChangePasswordAsync(ChangePasswordRequest request);
        Task<AccountDto> GetByUsername(string username);
    }
    public class AccountService : ServiceBase<Account, AccountDto>, IAccountService
    {
        private readonly IRepositoryBase<Account> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private OperationResult operationResult;

        public AccountService(
            IRepositoryBase<Account> repo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration configMapper
            )
            : base(repo, unitOfWork, mapper, configMapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configMapper = configMapper;
        }
        
        /// <summary>
        /// Add account sau do add AccountGroupAccount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<OperationResult> AddAsync(AccountDto model)
        {
            try
            {
                var checkUsername = await _repo.FindAll(x => x.Username == model.Username).AnyAsync();
                if (checkUsername)
                {
                    operationResult.Message = "The username existed!";
                    return operationResult;
                }    
                var item = _mapper.Map<Account>(model);
                item.Password = item.Password.ToEncrypt();
                item.Consumer = new Consumer
                {
                    EmployeeId = model.Username,
                    Email = model.Email,
                    FullName = model.FullName
                };
                _repo.Add(item);
                await _unitOfWork.SaveChangeAsync();

                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.AddSuccess,
                    Success = true,
                    Data = model
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }
        /// <summary>
        /// Add account sau do add AccountGroupAccount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<OperationResult> UpdateAsync(AccountDto model)
        {
            try
            {
                
                var item = await _repo.FindByIdAsync(model.Id);

                if (model.Password.IsBase64() == false)
                        item.Password = model.Password.ToEncrypt();
                item.Username = model.Username;
                item.AccountTypeId = model.AccountTypeId;
                item.Group = model.Group;
                item.Consumer.EmployeeId = model.Username;
                item.Consumer.Email = model.Email;
                item.Consumer.FullName= model.FullName;
                _repo.Update(item);
                await _unitOfWork.SaveChangeAsync();

                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = MessageReponse.UpdateSuccess,
                    Success = true,
                    Data = model
                };
            }
            catch (Exception ex)
            {
                operationResult = ex.GetMessageError();
            }
            return operationResult;
        }

        public async Task<OperationResult> LockAsync(int id)
        {
            var item = await _repo.FindByIdAsync(id);
            if (item == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Không tìm thấy tài khoản này!", Success = false };
            }
            item.IsLock = !item.IsLock;
            try
            {
                _repo.Update(item);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = item.IsLock ? MessageReponse.LockSuccess : MessageReponse.UnlockSuccess,
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

        public async Task<AccountDto> GetByUsername(string username)
        {
            var result = await _repo.FindAll(x => x.Username.ToLower() == username.ToLower()).ProjectTo<AccountDto>(_configMapper).FirstOrDefaultAsync();
            return result;
        }

        public async Task<OperationResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var item = await _repo.FindByIdAsync(request.Id);
            if (item == null)
            {
                return new OperationResult { StatusCode = HttpStatusCode.NotFound, Message = "Không tìm thấy tài khoản này! Not found the account", Success = false };
            }
            item.Password = request.NewPassword.ToEncrypt();
            try
            {
                _repo.Update(item);
                await _unitOfWork.SaveChangeAsync();
                operationResult = new OperationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Successfully 成功地!",
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
}
