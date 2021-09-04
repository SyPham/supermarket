using AutoMapper;
using Supermarket.Data;
using Supermarket.DTO;
using Supermarket.Models;
using Supermarket.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Services
{
    public interface IGroupService: IServiceBase<Group, GroupDto>
    {
        Task<bool> UpdateStatus(int id);
    }
    public class GroupService : ServiceBase<Group, GroupDto>, IGroupService
    {
        private readonly IRepositoryBase<Group> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public GroupService(
            IRepositoryBase<Group> repo, 
            IUnitOfWork unitOfWork,
            IMapper mapper, 
            MapperConfiguration configMapper
            )
            : base(repo, unitOfWork, mapper,  configMapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configMapper = configMapper;
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
