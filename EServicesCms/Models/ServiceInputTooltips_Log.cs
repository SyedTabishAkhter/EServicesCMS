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
    
    public partial class ServiceInputTooltips_Log
    {
        public int LogId { get; set; }
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
}
