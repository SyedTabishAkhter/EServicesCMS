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
    
    public partial class ServiceTemplates_Log
    {
        public int LogId { get; set; }
        public int TemplateId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public Nullable<int> InputId { get; set; }
        public string FileUrlAlt { get; set; }
        public string InputControlId { get; set; }
        public string ServiceGuid { get; set; }
        public string CategoryGuid { get; set; }
    }
}
