using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    //Inherits from the AutoMapper Profile class and maps the Camp class to the CampModel class.
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>();

            //This was missing. Without this the mapper couldn't map CampModel objects to Camp objects.
            this.CreateMap<CampModel, Camp>(); 

            /*
             * A way to include properties in the CampModel that don't come from the Camp class.
             * 
             * this.CreateMap<Camp, CampModel>()
             * .ForMember(c => c.classPropertyName, o => o.MapFrom(m => m.className.classPropertyName));
             */
        }
    }
}
