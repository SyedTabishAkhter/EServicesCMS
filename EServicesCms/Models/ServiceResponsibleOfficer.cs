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
    
    public partial class ServiceResponsibleOfficer
    {
        public int ContactId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string NameEng { get; set; }
        public string NameAlt { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string ServiceGuid { get; set; }
    }
}
