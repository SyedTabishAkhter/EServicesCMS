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
    
    public partial class UserAuthDetail
    {
        public long AuthId { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string userdisplayname { get; set; }
        public string mobile { get; set; }
        public string token { get; set; }
        public string jwtoken { get; set; }
        public string responseToId { get; set; }
        public string samlResponse { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string RowInsertedBy { get; set; }
        public Nullable<System.DateTime> RowInsertDate { get; set; }
        public string RowUpdatedBy { get; set; }
        public Nullable<System.DateTime> RowUpdateDate { get; set; }
        public string IpAddress { get; set; }
        public Nullable<System.DateTime> tokenExpiry { get; set; }
        public string nameId { get; set; }
        public string userType { get; set; }
        public string loginType { get; set; }
        public string nationality { get; set; }
        public string userAttributes { get; set; }
    }
}
