//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EServicesCms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TokenClientKey
    {
        public int ClientKeyID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
