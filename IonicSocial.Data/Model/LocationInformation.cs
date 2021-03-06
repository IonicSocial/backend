//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialApp.Data.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class LocationInformation
    {
        public int CenterID { get; set; }
        public int CityID { get; set; }
        public int LocationID { get; set; }
        public string CenterTitle { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string PinCode { get; set; }
        public System.Data.Entity.Spatial.DbGeography GeogCol1 { get; set; }
        public string GeogCol2 { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<long> InsertedBy { get; set; }
        public Nullable<System.DateTime> InsertedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual City City { get; set; }
        public virtual Location Location { get; set; }
    }
}
