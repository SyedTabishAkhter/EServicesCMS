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
    
    public partial class LkCountry
    {
        public int CountryId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public string CountryCode { get; set; }
        public string PhoneCode { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
    }
}
