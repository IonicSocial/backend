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
    
    public partial class MediaLocation
    {
        public long MediaLocationID { get; set; }
        public long MediaID { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZIP { get; set; }
        public Nullable<int> InsertedBy { get; set; }
        public Nullable<System.DateTime> InsertedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}