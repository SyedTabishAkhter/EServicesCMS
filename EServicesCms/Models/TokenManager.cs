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
    
    public partial class TokenManager
    {
        public int TokenID { get; set; }
        public string TokenKey { get; set; }
        public Nullable<System.DateTime> IssuedOn { get; set; }
        public Nullable<System.DateTime> ExpiresOn { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> UserID { get; set; }
    }
}