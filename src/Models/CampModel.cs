using CoreCodeCamp.Data;
using System;
using System.Collections.Generic;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Models
{
    /*
     * This is just another version of the Camp class. This is done when we just want to show the user some
     * specific properties and not all of them.
     */
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;

        //By prefixing these properties with the word "Location" (the class that contains these properties)
        //AutoMap binds them for us. Meaning --> It is not necessary to create a LocationModel with the specific
        //porperties we want the users to see.
        public string LocationVenueName { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        //What happens if we do not like how the property names look like? Is there another way to include
        //Location properties without creating a LocationModel? Yes, check the CampProfile.cs file.

        public ICollection<TalkModel> Talks { get; set; }
    }
}
