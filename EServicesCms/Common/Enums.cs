using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EServicesCms.Common
{
    public enum AttachmentType
    {
        
    }
    
    public enum Status
    {
        Active = 1,
        InActive = 2,
    }
    public enum UserStatus
    {
        Active = 1,
        InActive = 2,
        Deleted = 3,
        UnAuthorized = 4
    }
    public enum TemplatesSources
    {
        Folder = 1,
        Database = 2,
    }
    public enum IsValid
    {
        Yes = 1,
        No = 0
    }
    public enum FormInputTypes
    {
        Textbox = 1,
        Email = 2,
        Number = 3,
        Dropdownlist = 4,
        TextArea = 5,
        Attachment = 6,
        RadioButton = 7,
        Checkbox = 8,
        Calendar = 9,
        Password = 10,
        Telephone = 11,
        URL = 12,
        map = 13,
        label = 14,
        landline = 15,
        label_info = 16,
        label_warning = 17,
        label_error = 18,
        label_success = 19,
        SingleCheckbox =20
    }

    public enum Guide
    {
        Procedures = 1,
        Faq = 2,
        Support = 3,
        Channels = 4,
        Videos = 5,
        Beneficiaries = 6,
        Types = 7
    }
}