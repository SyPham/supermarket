using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Supermarket.Constants;
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
        Task<object> GetAllByStore(int id);
        Task<object> GetAllByStoreLang(int id, string lang );
        Task<object> GetAllByLang(string langId);
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

        public async Task<object> GetAllByStore(int id)
        {
            return await _repo.FindAll().Where(x => x.Store_ID == id).ToListAsync();
        }

        public async Task<object> GetAllByStoreLang(int id, string lang)
        {
            if (lang == "vi")
            {
                return await _repo.FindAll().Where(x => x.Store_ID == id).Select(x => new KindDto { 
                Id = x.Id,
                Name = x.VietnameseName
                }).ToListAsync();
            } else  if (lang == "en")
            {
                return await _repo.FindAll().Where(x => x.Store_ID == id).Select(x => new KindDto
                {
                    Id = x.Id,
                    Name = x.EnglishName
                }).ToListAsync();
            }
             else {
                return await _repo.FindAll().Where(x => x.Store_ID == id).Select(x => new KindDto
                {
                    Id = x.Id,
                    Name = x.ChineseName
                }).ToListAsync();
            }
        }

        public async Task<object> GetAllByLang(string langId)
        {
            var data = await _repo.FindAll().Select(x => new
            {
                Id = x.Id,
                Name = langId == SystemLang.VI ? x.VietnameseName : langId == SystemLang.EN ? x.EnglishName : x.ChineseName,
            }).ToListAsync();
            return data;
        }
    }
}
