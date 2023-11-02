using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EServicesCms.Models
{
    public class ImportExport
    {
        public Models.IEService Service { get; set; }
        //public List<Models.IEService> SubServices { get; set; }
        public List<Models.IEServiceViewer> ServiceViewers { get; set; }
        public List<Models.IEServiceUserTypes> ServiceUserTypes { get; set; }
        public List<Models.IEServiceCategory> ServiceCategories { get; set; }
        public List<Models.IEServiceTab> ServiceTabs { get; set; }
        public Models.IEServiceCommentsAttachmentsConfig ServiceCommentsAttachmentsConfig { get; set; }
        public List<Models.IEServiceFormsConfig> ServiceFormsConfig { get; set; }
        public List<Models.IEServiceVideo> ServiceVideos { get; set; }
    }
    public class ImportExport2
    {
        public List<Models.IEService> Services { get; set; }
    }

    public class IEService
    {
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
        public List<Models.IEServiceViewer> ServiceViewers { get; set; }
        public List<Models.IEServiceUserTypes> ServiceUserTypes { get; set; }
        public List<Models.IEServiceCategory> ServiceCategories { get; set; }
        public List<Models.IEServiceTab> ServiceTabs { get; set; }
        public Models.IEServiceCommentsAttachmentsConfig ServiceCommentsAttachmentsConfig { get; set; }
        public List<Models.IEServiceFormsConfig> ServiceFormsConfig { get; set; }
        //public List<Models.IEServiceVideo> ServiceVideos { get; set; }
    }

    public class IEServiceViewer
    {
        public int RecordId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }

    }
    public class IEServiceUserTypes
    {
        public int RecordId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> UserTypeId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }

    }
    public class IEServiceCategory
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
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> PrintPreview { get; set; }
        public string CategoryCode { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAr { get; set; }
        public Nullable<bool> TabularLayout { get; set; }

    }
    public class IEServiceTab
    {
        public int TabId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
    }
    public class IEServiceCommentsAttachmentsConfig
    {
        public int RecordId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> Minimum { get; set; }
        public Nullable<int> Maximum { get; set; }
        public Nullable<int> MaxSize { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
    }
    public class IEServiceFormsConfig
    {
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
        public List<Models.IEServiceFormsDrilldownConfig> ServiceFormsDrilldownConfig { get; set; }
        public List<Models.IEServiceInputLookupAction> ServiceInputLookupActions { get; set; }
        public List<Models.IEServiceInputTooltip> ServiceInputTooltips { get; set; }
        public Models.IEServiceTemplate ServiceTemplates { get; set; }
        public Models.IELkLookup Lookup { get; set; }
    }
    public class IEServiceFormsDrilldownConfig
    {
        public int DrilldownId { get; set; }
        public Nullable<int> InputId { get; set; }
        public string ReferralId { get; set; }
        public string LogicalOperator { get; set; }
        public string ReferralIdValue { get; set; }
        public string AlertMessage { get; set; }
        public string AlertMessageAr { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string RowUpdatedBy { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string IpAddress { get; set; }
        public string InputControlId { get; set; }
    }
    public class IEServiceInputLookupAction
    {
        public int OptionId { get; set; }
        public int InputId { get; set; }
        public int LookupId { get; set; }
        public string LookupValue { get; set; }
        public Nullable<int> ActionId { get; set; }
        public string DescriptionAlt { get; set; }
        public string DescriptionEng { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string InputControlId { get; set; }
    }
    public class IEServiceInputTooltip
    {
        public int TooltipId { get; set; }
        public int InputId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string DescriptionAlt { get; set; }
        public string DescriptionEng { get; set; }
        public Nullable<bool> EnableGuide { get; set; }
        public Nullable<bool> EnableCard { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string InputControlId { get; set; }
    }
    public class IEServiceTemplate
    {
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
    }
    public class IEServiceVideo
    {
        public int VideoId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Name { get; set; }
        public string NameAlt { get; set; }
        public string VideoUrl { get; set; }
        public string VideoAltUrl { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
    }
    public class IELkLookup
    {
        public int LookupId { get; set; }
        public string LookUpName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public List<Models.IELookupOption> LookupOption { get; set; }
    }
    public class IELookupOption
    {
        public int OptionId { get; set; }
        public int LookupId { get; set; }
        public string Code { get; set; }
        public string DescriptionAlt { get; set; }
        public string DescriptionEng { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string LookUpName { get; set; }
    }
}