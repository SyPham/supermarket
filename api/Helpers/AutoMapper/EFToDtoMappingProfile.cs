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

            CreateMap<Account, AccountDto>()
               .ForMember(d => d.Email, o => o.MapFrom(s => s.ConsumerId != null ? s.Consumer.Email ?? "" : ""))
               .ForMember(d => d.AccountType, o => o.MapFrom(s => s.AccountTypeId != null ? s.AccountType.Name ?? "" : ""))
               .ForMember(d => d.Group, o => o.MapFrom(s => s.Group_ID != null ? s.Group.Name ?? "" : ""))
               .ForMember(d => d.Team, o => o.MapFrom(s => s.Team_ID != null ? s.Team.Name ?? "" : ""))
               .ForMember(d => d.FullName, o => o.MapFrom(s => s.ConsumerId != null ? s.Consumer.FullName : s.Username));

            CreateMap<AccountType, AccountTypeDto>();
            CreateMap<Account, UserForDetailDto>()
               .ForMember(d => d.FullName, o => o.MapFrom(s => s.ConsumerId != null ? s.Consumer.FullName : s.Username));
            CreateMap<Store, StoreDto>();
            CreateMap<Team, TeamDto>();
            CreateMap<Group, GroupDto>();
            CreateMap<Kind, KindDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Consumer, ConsumerDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailHistory, OrderDetailHistoryDto>();

            CreateMap<Cart, CartDto>();

        }
    }
}
