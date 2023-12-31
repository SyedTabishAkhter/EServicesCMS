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
    
    public partial class ServiceFormsConfig_Log
    {
        public int LogId { get; set; }
        public int InputId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CategorId { get; set; }
        public Nullable<int> LanguageId { get; set; }
        public Nullable<int> InputTypeId { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public Nullable<bool> Required { get; set; }
        public Nullable<int> OptionId { get; set; }
        public string Attributes { get; set; }
        public string Message { get; set; }
        public Nullable<int> Maximum { get; set; }
        public Nullable<int> Minimum { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string LabelAr { get; set; }
        public string PlaceholderAr { get; set; }
        public string AttributesAr { get; set; }
        public string MessageAr { get; set; }
        public string UserField { get; set; }
        public Nullable<bool> IsExternalLookup { get; set; }
        public string ExternalLookupId { get; set; }
        public Nullable<int> MaxFileSize { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ArabicInput { get; set; }
        public Nullable<int> EnglishInput { get; set; }
        public Nullable<int> DynamicInput { get; set; }
        public Nullable<bool> HasDrilldown { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationMessageAr { get; set; }
        public string HelpMessage { get; set; }
        public string HelpMessageAr { get; set; }
        public Nullable<bool> DownloadAttachment { get; set; }
        public Nullable<bool> IsReadOnly { get; set; }
        public Nullable<int> TabId { get; set; }
        public string FilterId { get; set; }
        public string FilterValue { get; set; }
        public string JsonAttribute { get; set; }
        public Nullable<bool> ApplyWordCount { get; set; }
        public string Bookmark { get; set; }
        public Nullable<bool> SpeechToText { get; set; }
        public string ServiceGuid { get; set; }
        public string CategoryGuid { get; set; }
    }
}
