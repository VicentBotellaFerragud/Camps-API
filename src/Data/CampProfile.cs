﻿using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    //Inherits from the AutoMapper Profile class and maps the Camp class into the CampModel class.
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>();

            /*
             * A way to include properties in the CampModel that don't come from the Camp class.
             * 
             * this.CreateMap<Camp, CampModel>()
             * .ForMember(c => c.classPropertyName, o => o.MapFrom(m => m.className.classPropertyName));
             */
        }
    }
}
