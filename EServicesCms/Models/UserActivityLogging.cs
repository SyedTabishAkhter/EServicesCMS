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
    
    public partial class UserActivityLogging
    {
        public long RecordId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string LogType { get; set; }
        public string Keyword { get; set; }
        public string Username { get; set; }
        public string UserAgent { get; set; }
        public string UserDefined01 { get; set; }
        public string UserDefined02 { get; set; }
        public string UserDefined03 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string RowUpdatedBy { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string IpAddress { get; set; }
        public string UserType { get; set; }
        public string LoginType { get; set; }
    }
}