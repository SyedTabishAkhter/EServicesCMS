using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EServicesCms.Models
{
    public class BaseRequest
    {
        public int dummy { get; set; }
    }
    public class BaseResponse
    {
        public int Status { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string Message { get; set; }
        public JsonResult iResult { get; set; }
    }
    public class DataTableAjaxPostModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
    }
    public class SearchParams : Models.DataTableAjaxPostModel
    {
        public string SearchCri { get; set; }
        public string ViewId { get; set; }
        public int? TypeId { get; set; }
        public int? EntityId { get; set; }
        public string UserName { get; set; }
        public Int16? DepartmentId { get; set; }
        public Int16? RoleId { get; set; }
        public int? CategoryId { get; set; }
        public int? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }
    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }
    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    public class jQueryDataTableParamModel
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }
    public class Login
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class AuthUser
    {
        public int UserId { get; set; }
        public Nullable<short> UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Nullable<short> CountryId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SecurityAnswer { get; set; }
        public Nullable<short> SecurityQuestionId { get; set; }
        public Nullable<bool> IsEmailVerified { get; set; }
        public Nullable<bool> IsMobileNoVerified { get; set; }
        public string UserKey { get; set; }
        public Nullable<bool> IsFirstLogin { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CelebrityId { get; set; }
        public string Picture { get; set; }
    }
    public class ForgotPassword
    {
        public string Email { get; set; }
        public string SecurityAnswer { get; set; }
        public Nullable<short> SecurityQuestionId { get; set; }
    }
    public class UsersFilter : Models.DataTableAjaxPostModel
    {
        public int? ServiceProviderId { get; set; }
        public string SearchCri { get; set; }
    }
    public class ResetPassword
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReconfirmPassword { get; set; }
        public string SecurityAnswer { get; set; }
        public Nullable<short> SecurityQuestionId { get; set; }
    }
    public class Paging
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
    public class KeyValue
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public int SortOrder { get; set; }
    }
    public class Dashboard
    {
        public string KPI { get; set; }
        public int Value { get; set; }
    }
    public class ChartData
    {
        public string SeriesName { get; set; }
        public string XValue { get; set; }
        public int YValue { get; set; }
    }
    public class SeriesModel<T>
    {
        public string name { get; set; }
        public string type { get; set; }
        public string color { get; set; }
        public ICollection<T> data { get; set; }
    }
    public class SeriesData
    {
        public int y { get; set; }
        public int inspectorId { get; set; }
    }
    public class ChartModel<T>
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string[] XAxisCategories { get; set; }
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string YAxisTooltipValueSuffix { get; set; }
        public ICollection<SeriesModel<T>> Series { get; set; }
    }
    public class AuthenticateRequest : BaseRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public class ServiceObject
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
        public string EntityNameEng { get; set; }
        public string EntityNameAlt { get; set; }
        public string TypeNameEng { get; set; }
        public string TypeNameAlt { get; set; }
        public string soWidgetCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> TabularLayout { get; set; }
        public Nullable<bool> IsAnonymous { get; set; }
        public Nullable<bool> PrintPreview { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAr { get; set; }
        public Nullable<int> ApiSourceId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        
        public Nullable<bool> UseParentExternalServiceId { get; set; }
        public string ParentExternalServiceID { get; set; }
        public scaConfig CommentAttachments { get; set; }
        public List<ServiceEntitiesMapping> Mapping { get; set; }
        public List<ServiceUserType> UserTypes { get; set; }
    }
    public class ServiceUserTypeObject
    {
        public int UserTypeId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
    }
    public class ServiceEntityObject
    {
        public int EntityId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public string RemarksEng { get; set; }
        public string RemarksAlt { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
    }
    public class ServiceTypeObject
    {
        public int TypeId { get; set; }
        public string DescriptionEng { get; set; }
        public string DescriptionAlt { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
    }
    public class ServiceCategoryObject
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
        public Nullable<bool> IsActive { get; set; }
        public string ExternalServiceID { get; set; }
        public string CategoryCode { get; set; }
        public Nullable<bool> PrintPreview { get; set; }
        public Nullable<bool> TabularLayout { get; set; }
        public string PrintMessage { get; set; }
        public string PrintMessageAr { get; set; }
    }
    public class ServiceFormsConfigObject
    {
        public int InputId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> ParentServiceId { get; set; }
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
        public string ServiceName { get; set; }
        public string ServiceNameAlt { get; set; }
        public string CategoryName { get; set; }
        public string InputTypeName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ArabicInput { get; set; } = 3;
        public Nullable<int> EnglishInput { get; set; } = 2;
        public Nullable<int> DynamicInput { get; set; } = 0;
        public Nullable<int> TabId { get; set; }
        public string ExternalServiceID { get; set; }
        //public string ReferralId { get; set; }
        //public string LogicalOperator { get; set; }
        //public string ReferralIdValue { get; set; }
        public bool? HasDrilldown { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationMessageAr { get; set; }
        public string HelpMessage { get; set; }
        public string HelpMessageAr { get; set; }
        public string FilterId { get; set; }
        public string FilterValue { get; set; }
        public string JsonAttribute { get; set; }
        public string Bookmark { get; set; }
        public Nullable<bool> DownloadAttachment { get; set; } = false;
        public Nullable<bool> IsReadOnly { get; set; } = false;
        public Nullable<bool> ApplyWordCount { get; set; } = false;
        public Nullable<bool> SpeechToText { get; set; } = false;
    }
    public class ServiceFormsDrilldownObject
    {
        public int DrilldownId { get; set; }
        public int InputId { get; set; }
        public string InputControlId { get; set; }
        public string ReferralId { get; set; }
        public string ReferralName { get; set; }
        public string LogicalOperator { get; set; }
        public string ReferralIdValue { get; set; }
        public string AlertMessage { get; set; }
        public string AlertMessageAr { get; set; }
    }
    public class ServiceRequestsObject
    {
        public long UniqueId { get; set; }
        public string RequestId { get; set; }
        public string UserId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string StatusEng { get; set; }
        public string StatusAlt { get; set; }
        public string Category { get; set; }
        public string Service { get; set; }
        public string PreviousRequestId { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string RowUpdatedBy { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string IpAddress { get; set; }
        public int TotalRows { get; set; }
    }
    public class UserScreenActionsObject
    {
        public string ActionName { get; set; }
        public string UniqueId { get; set; }
        public string Remarks { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string IpAddress { get; set; }
        public int TotalRows { get; set; }
    }
    public class LoggerObject
    {
        public string VersionNo { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public int TotalRows { get; set; }
    }
    public class UserObject
    {
        public int UserId { get; set; }
        public Nullable<short> RoleId { get; set; }
        public string FullName { get; set; }
        public string FullNameAlt { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<short> SecurityQuestionId { get; set; }
        public string SecurityAnswer { get; set; }
        public Nullable<bool> IsFirstLogin { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<short> DepartmentId { get; set; }
        public int TotalRows { get; set; }
    }
    public class LkRoleObject
    {
        public short RoleId { get; set; }
        public string Description { get; set; }
        public string DescriptionAlt { get; set; }
        public List <LkGroup> RoleGroups { get; set; }
    }
    public class ChangePassword
    {
        public string NewPassword { get; set; }
        public string ReconfirmPassword { get; set; }
    }
    public class scaConfig
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public int? MaxSize { get; set; }
    }

    public class LkLkLoopUpActionObject
    {
        public int InputId { get; set; }
        public int LookupId { get; set; }
        public int ActionId { get; set; }
        public string InputControlId { get; set; }
        //public string LookupValue { get; set; }
        //public string DescriptionAlt { get; set; }
        //public string DescriptionEng { get; set; }
        public List<LookupOption> LookupValues { get; set; }
    }
    public class LookupData
    {
        public string Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
    public class Result
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class GetLookupListResponse
    {
        public Result Result { get; set; }
        public List<LookupData> Data { get; set; }
    }
    public class ServiceTemplateRequest
    {
        public int TemplateId { get; set; }
        public int InputId { get; set; }
        public string InputControlId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string FileUrlAlt { get; set; }
        public HttpPostedFileBase File { get; set; }
        public HttpPostedFileBase FileAlt { get; set; }
    }
    public class ServiceGuideData
    {
        public ServiceGuide serviceGuide { get; set; }
        public List<ServiceGuideProcedure> serviceGuideProcedures { get; set; }
        public List<ServiceGuideChannel> serviceGuideChannels { get; set; }
        public List<ServiceGuideFaq> serviceGuideFaqs { get; set; }
        public List<ServiceGuideSupport> serviceGuideSupports { get; set; }
        public List<ServiceGuideVideo> serviceGuideVideos { get; set; }
    }

    public class BalTokenRequest
    {
        public string username;
        public string password;
    }
    public class BalTokenResponse
    {
        public string access_token;
        public string token_type;
        public long expires_in;
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string expiresOn { get; set; }
    }
    public class TokenBaseResponse
    {
        public int StatusCode { get; set; }
        public object Content { get; set; }
    }

    public class DashboardFilters
    {
        public int? yearValue { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int? categoryId { get; set; }
        public string monthName { get; set; }
    }
}