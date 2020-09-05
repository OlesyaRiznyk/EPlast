﻿using AutoMapper;
using EPlast.BLL.DTO.UserProfiles;

namespace EPlast.BLL.Mapping.User
{
    public class ShortUserInformationProfile : Profile
    {
        public ShortUserInformationProfile()
        {
            CreateMap<DataAccess.Entities.User, ShortUserInformationDTO>().
                ForMember(x => x.Birthday, q => q.MapFrom(w => w.UserProfile.Birthday)).
                ForMember(g => g.Gender, q => q.MapFrom(w => w.UserProfile.Gender)).
                ReverseMap();
        }
    }
}
