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
    public interface IKindService: IServiceBase<Kind, KindDto>
    {
    }
    public class KindService : ServiceBase<Kind, KindDto>, IKindService
    {
        private readonly IRepositoryBase<Kind> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public KindService(
            IRepositoryBase<Kind> repo, 
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
    }
}
