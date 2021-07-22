﻿using AutoMapper;
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
            CreateMap<Account, UserForDetailDto>()
               .ForMember(d => d.FullName, o => o.MapFrom(s => s.ConsumerId != null ? s.Consumer.FullName : s.Username));
            CreateMap<Store, StoreDto>();
            CreateMap<Kind, KindDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Consumer, ConsumerDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDetail, OrderDetailDto>();

            CreateMap<Cart, CartDto>();

        }
    }
}
