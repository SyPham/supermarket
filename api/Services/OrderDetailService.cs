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
    public interface IOrderDetailService: IServiceBase<OrderDetail, OrderDetailDto>
    {
    }
    public class OrderDetailService : ServiceBase<OrderDetail, OrderDetailDto>, IOrderDetailService
    {
        private readonly IRepositoryBase<OrderDetail> _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public OrderDetailService(
            IRepositoryBase<OrderDetail> repo, 
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
