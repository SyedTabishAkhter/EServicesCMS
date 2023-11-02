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
    
    public partial class Services_Log
    {
        public int LogId { get; set; }
        public int ServiceId { get; set; }
        public Nullable<int> ParentServiceId { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string ExternalServiceID { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public string ServiceUrl { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string DefinitionEng { get; set; }
        public string DefinitionAlt { get; set; }
        public string DurationEng { get; set; }
        public string DurationAlt { get; set; }
        public string FeesEng { get; set; }
        public string FeesAlt { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> UseParentExternalServiceId { get; set; }
        public string ParentExternalServiceID { get; set; }
        public Nullable<bool> PrintPreview { get; set; }
        public string soWidgetCode { get; set; }
        public Nullable<bool> TabularLayout { get; set; }
        public Nullable<bool> IsAnonymous { get; set; }
        public Nullable<int> ApiSourceId { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAr { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<bool> IsRequisition { get; set; }
        public string ServiceGuid { get; set; }
        public string NameEng { get; set; }
        public string NameAlt { get; set; }
        public string CardUrlEng { get; set; }
        public string CardUrlAlt { get; set; }
        public string FormUrlEng { get; set; }
        public string FormUrlAlt { get; set; }
    }
}
