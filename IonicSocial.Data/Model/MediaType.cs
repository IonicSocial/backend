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
    
    public partial class MediaType
    {
        public int MediaTypeID { get; set; }
        public string Type { get; set; }
        public long InsertedBy { get; set; }
        public System.DateTime InsertedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}