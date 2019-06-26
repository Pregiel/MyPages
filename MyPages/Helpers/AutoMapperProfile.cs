using AutoMapper;
using MyPages.Dtos;
using MyPages.Entities;
using MyPages.Models;

namespace MyPages.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Folder, CreateNewItemModel>();
            CreateMap<CreateNewItemModel, Folder>();

            CreateMap<Page, CreateNewItemModel>();
            CreateMap<CreateNewItemModel, Page>();

            CreateMap<Folder, UpdateItemModel>();
            CreateMap<UpdateItemModel, Folder>();

            CreateMap<Page, UpdateItemModel>();
            CreateMap<UpdateItemModel, Page>();

            CreateMap<Folder, ItemDto>()
                .ForMember(dest => dest.ItemType,
                opt => opt.MapFrom(src => "Folder"));

            CreateMap<Page, ItemDto>()
                .ForMember(dest => dest.ItemType,
                opt => opt.MapFrom(src => "Page"));
        }
    }
}
