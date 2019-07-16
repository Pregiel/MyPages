using AutoMapper;
using MyPages.Entities;
using MyPages.Models;

namespace MyPages.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Page, CreateNewPageModel>();
            CreateMap<CreateNewPageModel, Page>();

            CreateMap<Page, UpdatePageModel>();
            CreateMap<UpdatePageModel, Page>();
        }
    }
}
