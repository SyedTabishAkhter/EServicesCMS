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
    
    public partial class ES_GetServicesList_Result
    {
        public int ServiceId { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string ExternalServiceID { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public string ServiceUrl { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> ParentServiceId { get; set; }
        public Nullable<bool> UseParentExternalServiceId { get; set; }
        public string ParentExternalServiceID { get; set; }
        public bool PrintPreview { get; set; }
        public string soWidgetCode { get; set; }
        public bool TabularLayout { get; set; }
        public bool IsAnonymous { get; set; }
        public int ApiSourceId { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAlt { get; set; }
        public bool IsRequisition { get; set; }
        public string ServiceGuid { get; set; }
        public string NameEng { get; set; }
        public string NameAlt { get; set; }
        public string CardUrlEng { get; set; }
        public string CardUrlAlt { get; set; }
        public string FormUrlEng { get; set; }
        public string FormUrlAlt { get; set; }
    }
}