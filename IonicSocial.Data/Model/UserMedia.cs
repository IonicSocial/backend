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
    
    public partial class UserMedia
    {
        public long MediaID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public int MediaType { get; set; }
        public string ThumbPath { get; set; }
        public string MediaPath { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string InsertedBy { get; set; }
        public System.DateTime InsertedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
