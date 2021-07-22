using AutoMapper;
using Supermarket.DTO;
using Supermarket.DTO.auth;
using Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Helpers.AutoMapper
{
    public class DtoToEFMappingProfile : Profile
    {
        public DtoToEFMappingProfile()
        {
            CreateMap<AccountDto, Account>()
                .ForMember(d => d.AccountType, o => o.Ignore());
            CreateMap<AccountTypeDto, AccountType>()
                .ForMember(d => d.Accounts, o => o.Ignore());
            CreateMap<UserForDetailDto, Account>();

            CreateMap<StoreDto, Store>();
            CreateMap<KindDto, Kind>();
            CreateMap<ProductDto, Product>();
            CreateMap<ConsumerDto, Consumer>();
            CreateMap<OrderDto, Order>();
            CreateMap<OrderDetailDto, OrderDetail>();
        }
    }
}
