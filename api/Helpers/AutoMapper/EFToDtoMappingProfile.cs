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
    public class EFToDtoMappingProfile : Profile
    {
        public EFToDtoMappingProfile()
        {

            CreateMap<Account, AccountDto>();
            CreateMap<AccountType, AccountTypeDto>();
            CreateMap<Account, UserForDetailDto>();

            CreateMap<Store, StoreDto>();
            CreateMap<Kind, KindDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Consumer, ConsumerDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDetail, OrderDetailDto>();

        }
    }
}
