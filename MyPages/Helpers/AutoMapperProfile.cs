using AutoMapper;
using MyPages.Dtos;
using MyPages.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<User, UserFormDto>();
            CreateMap<UserFormDto, User>();
        }
    }
}
