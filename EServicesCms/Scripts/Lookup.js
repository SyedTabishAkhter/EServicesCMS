function GetLookup() {

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
function CloseForm2() {
    clearInput2();
    modal.style.display = "none";
}

function SaveLookup() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter External LookupId" });
        ShowAlert("LkExternalLookupId", "0");
        $("#DescriptionEng").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        html: ShowAlert('LkSaveLookup', '1') +'<br/> '+ $("#DescriptionEng").val(),//'Do you want to Save this LookupId ' + $("#DescriptionEng").val() + ' ?',
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.LookupId = $("#Code").val();
            iObject.LookupName = $("#DescriptionEng").val();
            //iObject.DescriptionAlt = $("#DescriptionAlt").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1') })
                    if (response.Result == true) {
                        CloseForm();
                        GetLookup();
                    }
                }
            });
        }
    })
}

function Delete(LookupId, Description) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('LkDeleteLookup', '1') + '<br/>' + Description,//'Do you want to Delete this LookupId ' + Description + ' ?',
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.LookupId = LookupId;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1') })
                    GetLookup();
                }
            });
        }
    })
}

function Modify(Code, DescriptionEng, DescriptionAra) {
    clearInput();
    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    //$("#DescriptionAlt").val(DescriptionAra);
    modal.style.display = "block";
}

function clearInput() {
    $("#Code").val("");
    $("#DescriptionEng").val("");
    //$("#DescriptionAlt").val("");
}

function clearInput2() {
    $("#DescriptionEng").val("");
    $("#DescriptionAlt").val("");
    $("#OptionId").val("0");
    $("#Code").val("0");
}

function GetUserAttribute() {

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

function DeleteAttribute(AttributeId, Description) {


    Swal.fire({
        icon: 'question',
        html: ShowAlert('LkDeleteAttribute', '1') + '<br/> ' + Description,//'Do you want to Delete this attribute ' + Description + ' ?',
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.AttributeId = AttributeId;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1') })
                    GetUserAttribute();
                }
            });
        }
    })
}

function SaveAttribute() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter SSO User Attribute" });
        ShowAlert("LkAttributeSSO", "0")
        $("#DescriptionEng").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        html: ShowAlert("LkSaveAttribute", "1") + ' <br/>' + $("#DescriptionEng").val(),//'Do you want to Save this Attribute ' + $("#DescriptionEng").val() + ' ?',
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.AttributeId = $("#Code").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
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
                        GetUserAttribute();
                    }
                }
            });
        }
    })
}

function GetLookupList() {

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

function SaveLookupItem() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Item Name English" });
        ShowAlert("LkItemNameEn", "0")
        $("#DescriptionEng").focus();
        return false;
    }

    if ($("#DescriptionAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Item Name Arabic" });
        ShowAlert("LkItemNameAr", "0")
        $("#DescriptionAlt").focus();
        return false;
    }

    var myName = $("#DescriptionEng").val();
    if (language == "0" || language == 0)
        myName = $("#DescriptionAlt").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert('LkConfirmSaveItem', '1') + ' <br/>' + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.LookupId = $("#lookupId").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.OptionId = $("#OptionId").val();
            iObject.Code = $("#Code").val();
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
                        CloseForm2();
                        GetLookupList();
                    }
                }
            });
        }
    })
}

function DeleteLookupList(OptionId, Code, DescriptionAlt, DescriptionEng) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('LkConfirmDeleteItem', '1') + '<br/> ' + DescriptionEng,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.LookupId = $("#lookupId").val();
            iObject.DescriptionEng = DescriptionEng;
            iObject.DescriptionAlt = DescriptionAlt;
            iObject.OptionId = OptionId;
            iObject.Code = Code;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetLookupList();
                }
            });
        }
    })
}

function ModifyLookupList(OptionId, Code, DescriptionAlt, DescriptionEng) {
    clearInput2();
    $("#OptionId").val(OptionId);
    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAlt);
    modal.style.display = "block";
}


function GetApiSources() {

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

function CloseApiForm() {
    clearApiInput();
    modal.style.display = "none";
}

function clearApiInput() {
    $("#ApiSourceName").val("");
    $("#ApiAccessURL").val("");
    $("#ApiKey").val("");
    $("#ApiUsername").val("");
    $("#ApiPassword").val("");
    $("#ApiSourceId").val("0");
}

function SaveApiSource() {

    if ($("#ApiSourceName").val() == "") {
        Swal.fire({ icon: "info", text: "Please Enter Api Source Name" });
        $("#ApiSourceName").focus();
        return false;
    }
    if ($("#ApiAccessURL").val() == "") {
        Swal.fire({ icon: "info", text: "Please Enter Api Access URL" });
        $("#ApiAccessURL").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: 'Do you want to Save this API Source ' + $("#ApiSourceName").val(),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.ApiSourceId = $("#ApiSourceId").val();
            iObject.ApiSourceName = $("#ApiSourceName").val();
            iObject.ApiAccessURL = $("#ApiAccessURL").val();
            iObject.ApiKey = $("#ApiKey").val();
            iObject.ApiUsername = $("#ApiUsername").val();
            iObject.ApiPassword = $("#ApiPassword").val();
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
                        CloseApiForm();
                        GetApiSources();
                    }
                }
            });
        }
    })
}

function DeleteApiSource(SourceId, Description) {

    Swal.fire({
        icon: 'question',
        text: 'Do you want to Delete this API Source ' + Description,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.ApiSourceId = SourceId;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetApiSources();
                }
            });
        }
    })
}

function ModifyApiSource(SourceId, Name, Url, Key, UserName, Password) {
    clearApiInput();
    $("#ApiSourceId").val(SourceId);
    $("#ApiSourceName").val(Name);
    $("#ApiAccessURL").val(Url);
    $("#ApiKey").val(Key);
    $("#ApiUsername").val(UserName);
    $("#ApiPassword").val(Password);
    modal.style.display = "block";
}







function PopulateLabelsGrid() {

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
    iSearch.ViewId = $("#FViewId").val();
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
        'iDisplayLength': 15,
        "scrollX": false,
        "autoWidth": false,
        stateSave: true,
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
            var info = table.page.info();
            console.log(info)
            $("#pageNo").val(info.page);
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
    $('#dynamic-table').show();

    var pNo = localStorage.getItem("pageNo");
    console.log("pNo=" + pNo)
    if (pNo != null) {
       
    }
}

function ModifyLabel(AbbrId, ViewId, LabelId, DescriptionEng, DescriptionAlt) {
    clearLabelsInput();
    $("#AbbrId").val(AbbrId);
    $("#ViewId").val(ViewId);
    $("#LabelId").val(LabelId);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAlt);
    modal.style.display = "block";
}

function SaveLabel() {

    if ($("#DescriptionEng").val() == "") {
        $("#DescriptionEng").focus();
        return false;
    }
    if ($("#DescriptionAlt").val() == "") {
        $("#DescriptionAlt").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmSaveLabel", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.AbbrId = $("#AbbrId").val();
            iObject.ViewId = $("#ViewId").val();
            iObject.LabelId = $("#LabelId").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1') })
                    if (response.Result == true) {
                        CloseLabelForm();                        
                        localStorage.setItem("pageNo", $("#pageNo").val());
                        PopulateLabelsGrid();
                    }
                }
            });
        }
    })
}

function CloseLabelForm() {
    clearLabelsInput();
    modal.style.display = "none";
}

function clearLabelsInput() {
    $("#AbbrId").val("0");
    $("#ViewId").val("");
    $("#LabelId").val("");
    $("#DescriptionEng").val("");
    $("#DescriptionAlt").val("");
}

function GetData() {
    setTimeout('PopulateLabelsGrid();', 1000);
}

function ResetLabelSearch() {

    $("#name_filter").val("");
    $("#ViewId").val("");
    PopulateLabelsGrid();
}