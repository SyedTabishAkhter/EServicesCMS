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
    
    public partial class ES_GetServiceCategories_Result
    {
        public int CategoryId { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public bool PrintPreview { get; set; }
        public string CategoryCode { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAlt { get; set; }
        public bool TabularLayout { get; set; }
        public string ServiceGuid { get; set; }
        public string CategoryGuid { get; set; }
    }
}