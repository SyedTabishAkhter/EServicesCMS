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
    
    public partial class User
    {
        public int UserId { get; set; }
        public Nullable<short> RoleId { get; set; }
        public string FullName { get; set; }
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
        public Nullable<short> DepartmentId { get; set; }
        public string FullNameAlt { get; set; }
    
        public virtual LkRole LkRole { get; set; }
    }
}
