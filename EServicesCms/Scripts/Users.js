function GetRoles() {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val(),
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $('div[myName="BOND"]').remove();
                    $("#dvData").append(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function CloseForm() {
    clearInput();
    modal.style.display = "none";
}

function SaveRole() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Role Description" });
        ShowAlert("SaveRole1","0");
        $("#DescriptionEng").focus();
        return false;
    }
    if ($("#DescriptionAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Role Description" });
        ShowAlert("SaveRole3", "0");
        $("#DescriptionAlt").focus();
        return false;
    }
    
    var groups = [];

    var xscreenIds = [0,1,2,3,4,5,6,7,8,9];
    //console.log(xscreenIds.length);
    for (var iScreenId in xscreenIds) {
        //console.log(iScreenId);
        if (document.getElementsByName('switch-field-1' + iScreenId)) {
            $.each($("input[name='switch-field-1" + iScreenId + "']:checked"), function () {
                var val = {
                    'GroupId': $(this).val(), 'Description': null, 'GroupName': null, 'ScreenId': iScreenId, 'SortOrder': null, 'IsDeleted': false
                };
                groups.push(val);
                //groups.push($(this).val());
            });
        }
    }
    console.log(groups.length);
    
    if (groups.length <= 0) {
        //Swal.fire({ icon: "info", text: "Select Permissions for this Role." });
        ShowAlert("SaveRole2", "0");
        return false;
    }

    var myName = $("#DescriptionEng").val();
    if (language == "0" || language == 0)
        myName = $("#DescriptionAlt").val();
    //console.log(groups);

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveRole", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.RoleId = $("#Code").val();
            iObject.Description = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.RoleGroups = groups;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    if (response.Result == true) {
                        CloseForm();
                        GetRoles();
                    }
                }
            });
        }
    })
}

function DeleteRole(RoleId, RoleName, RoleNameAr) {

    var myName = RoleName;
    if (language == "0" || language == 0)
        myName = RoleNameAr;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteRole", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?roleId=" + RoleId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetRoles();
                }
            });
        }
    })
}

function ModifyRole(Code, DescriptionEng, DescriptionAlt) {
    clearInput();
    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAlt);
    GetGroups();
    modal.style.display = "block";
}

function clearInput() {
    $("#Code").val("0");
    $("#DescriptionEng").val("");
    //$("#DescriptionAlt").val("");
}

function GetDepartments() {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val(),
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $('div[myName="BOND"]').remove();
                    $("#dvData").append(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function DeleteDepartment(DepartmentId, Name, NameAr) {

    var myName = Name;
    if (language == "0" || language == 0)
        myName = NameAr;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteDept", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?departmentId=" + DepartmentId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetDepartments();
                }
            });
        }
    })
}

function SaveDepartment() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Department Name" });
        ShowAlert("SaveDepartment1", "0");
        $("#DescriptionEng").focus();
        return false;
    }

    if ($("#DescriptionAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Department Name" });
        ShowAlert("SaveDepartment2", "0");
        $("#DescriptionAlt").focus();
        return false;
    }

    var myName = $("#DescriptionEng").val();
    if (language == "0" || language == 0)
        myName = $("#DescriptionAlt").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveDepartment", "1") + ' <br/>' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.DepartmentId = $("#Code").val();
            iObject.DepartmentName = $("#DescriptionEng").val();
            iObject.DepartmentNameAlt = $("#DescriptionAlt").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    if (response.Result == true) {
                        CloseForm();
                        GetDepartments();
                    }
                }
            });
        }
    })
}

function ResetUserSearch() {

    var flag = false;
    if ($("#name_filter").val() != "") {
        $("#name_filter").val("");
        flag = true;
    }
    if ($("#sDepartmentId").val() != "") {
        $("#sDepartmentId").val("");
        flag = true;
    }
    if ($("#sRoleId").val() != "") {
        $("#sRoleId").val("");
        flag = true;
    }
    if (flag == true)
        PopulateGrid();
}

function ModifyDepartment(Code, DescriptionEng, DescriptionAlt) {
    clearInput();
    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAlt);
    modal.style.display = "block";
}

function GetGroups() {

    var roleId = $("#Code").val()
    
    $.ajax({
        type: 'POST',
        url: $('#groupsUrl').val() + "?roleId=" + roleId,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            "roleId": roleId
        }),
        success: function (response) {
            $("#dvPermissions").html(response.html);
        }
    }, "json");
}

function clearUserInput() {
    $("#UserId").val("0");
    $("#FullName").val("");
    $("#Email").val("");
    $("#Mobile").val("");
    $("#DepartmentId").val("");
    $("#UserName").val("");
    $("#Password").val("");
    $("#RoleId").val("");
}

function SaveUser() {

    if ($("#FullName").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter User FullName" });
        ShowAlert("SaveUser1", "0");
        $("#FullName").focus();
        return false;
    }
    if ($("#FullNameAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter User FullName" });
        ShowAlert("SaveUser6", "0");
        $("#FullNameAlt").focus();
        return false;
    }
    if ($("#Email").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter User Email" });
        ShowAlert("SaveUser2", "0");
        $("#Email").focus();
        return false;
    }
    //if ($("#Mobile").val() == "") {
    //    Swal.fire({ icon: "info", text: "Please Enter User Mobile" });
    //    $("#Mobile").focus();
    //    return false;
    //}
    if ($("#DepartmentId").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Select User Department" });
        ShowAlert("SaveUser3", "0");
        $("#DepartmentId").focus();
        return false;
    }
    if ($("#UserName").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter UserName" });
        ShowAlert("SaveUser4", "0");
        $("#UserName").focus();
        return false;
    }
    //if ($("#Password").val() == "") {
    //    Swal.fire({ icon: "info", text: "Please Enter User Password" });
    //    $("#Password").focus();
    //    return false;
    //}
    //else {
    //    var pass = $("#Password").val();
    //    if (pass.length < 8) {
    //        Swal.fire({ icon: "info", text: 'You have entered less than 8 characters for password' });
    //        $("#Password").focus();
    //        return false;
    //    }
    //}
    if ($("#RoleId").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Select User Role" });
        ShowAlert("SaveUser5", "0");
        $("#RoleId").focus();
        return false;
    }

    var myName = $("#FullName").val();
    if (language == "0" || language == 0)
        myName = $("#FullNameAlt").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveUser", "1") + ' <br/>' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.UserId = $("#UserId").val();
            iObject.RoleId = $("#RoleId").val();
            iObject.DepartmentId = $("#DepartmentId").val();
            iObject.FullName = $("#FullName").val();
            iObject.FullNameAlt = $("#FullNameAlt").val();
            iObject.Email = $("#Email").val();
            iObject.Mobile = $("#Mobile").val();
            iObject.UserName = $("#UserName").val();
            //iObject.Password = $("#Password").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    if (response.Result == true) {
                        CloseUserForm();
                        PopulateGrid();
                    }
                }
            });
        }
    })
}

function CloseUserForm() {
    clearUserInput();
    modal.style.display = "none";
}

function GetData() {
    setTimeout('PopulateGrid();', 1000);
}

function PopulateGrid() {

    if (table) {
        table.destroy();
        $('#dynamic-table tbody').empty();
        console.log("DONE");
    }

    var thArray = [];
    $('#dynamic-table > thead > tr > th').each(function () {
        thArray.push($(this).text())
    })

    var iSearch = {};
    iSearch.SearchCri = $("#name_filter").val();
    iSearch.DepartmentId = $("#sDepartmentId").val();
    iSearch.RoleId = $("#sRoleId").val();
    console.log(JSON.stringify(iSearch));

    table = $('#dynamic-table').DataTable({
        "processing": true,
        "serverSide": true,
        "paging": true,
        "ajax": {
            "type": "POST",
            "dataType": 'json',
            "url": $("#gridUrl").val(),
            "data": iSearch
        },
        pagingType: "full_numbers",
        deferRender: true,
        aaSorting: [],
        "destroy": true,
        "searching": false,
        "bLengthChange": false,
        "responsive": true,
        "colReorder": true,
        "retrieve": true,
        'bPaginate': true,
        'iDisplayLength': 10,
        "scrollX": false,
        "autoWidth": false,
        initComplete: function (row, data, index) {
        },
        "order": [[0, 'asc']],
        "fnDrawCallback": function () {
            table.rows().every(function (rowIdx, tableLoop, rowLoop) {
                var row = $(this.node());
                for (var i = 0; i < thArray.length; i++) {
                    var iDataLabel = row.find('td').eq(i);
                    if (iDataLabel != null)
                        iDataLabel = iDataLabel.attr("data-label", thArray[i]);
                }
            });
        },
        "language": {
            "paginate": {
                "previous": ShowAlert("Previouspage", "1"),
                "next": ShowAlert("Nextpage", "1"),
                "last": ShowAlert("Lastpage", "1"),
                "first": ShowAlert("Firstpage", "1")
            },
            "emptyTable": ShowAlert("NoData", "1"),
            "info": ShowAlert("TblPaging", "1"),
        }
    });
    
    $("#dynamic-table_wrapper").addClass("dtBg");
    $("#dynamic-table_info").addClass("dtFooter2");
    $("#dynamic-table_paginate").addClass("dtFooter2");
    $('#dynamic-table').show();
}

function ModifyUser(UserId, RoleId, DepartmentId, FullName, Email, Mobile, UserName, Password, FullNameAr) {
    clearUserInput();
    $("#UserId").val(UserId);
    $("#RoleId").val(RoleId);
    $("#DepartmentId").val(DepartmentId);
    $("#FullName").val(FullName);
    $("#Email").val(Email);
    $("#Mobile").val(Mobile);
    $("#UserName").val(UserName);
    $("#Password").val(Password);
    $("#FullNameAlt").val(FullNameAr);
    modal.style.display = "block";
}

function DeleteUser(UserId, FullName, FullNameAr) {

    var myName = FullName;
    if (language == "0" || language == 0)
        myName = FullNameAr;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("DeleteSaveUser", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?userId=" + UserId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    PopulateGrid();
                }
            });
        }
    })
}

function ActivateUser(UserId, FullName, FullNameAr) {

    var myName = FullName;
    if (language == "0" || language == 0)
        myName = FullNameAr;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmActivateUser", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#actUrl").val() + "?userId=" + UserId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    PopulateGrid();
                }
            });
        }
    })
}

function DeActivateUser(UserId, FullName, FullNameAr) {

    var myName = FullName;
    if (language == "0" || language == 0)
        myName = FullNameAr;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeActivateUser", "1") + '<br/> ' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deActUrl").val() + "?userId=" + UserId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    PopulateGrid();
                }
            });
        }
    })
}

