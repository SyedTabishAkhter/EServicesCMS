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
    
    public partial class ServiceGuideSurveyQueryOption
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerEng { get; set; }
        public string AnswerAlt { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string RowInsertedBy { get; set; }
        public string RowUpdatedBy { get; set; }
        public string IpAddress { get; set; }
    }
}
