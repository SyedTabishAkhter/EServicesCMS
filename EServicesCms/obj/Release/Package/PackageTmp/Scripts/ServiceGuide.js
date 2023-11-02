function GetServiceGuideHints() {

    var iObject = {};
    iObject.dummy = 0;
    console.log(JSON.stringify(iObject));

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val(),
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(iObject),
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

function SaveHint() {

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Description English" });
        ShowAlert("LkSgHint1", "0");
        $("#DescriptionEng").focus();
        return false;
    }

    if ($("#DescriptionAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Description Arabic" });
        ShowAlert("LkSgHint2", "0");
        $("#DescriptionAlt").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmSaveHint', '1') + ' <br/>' + $("#DescriptionEng").val(),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.HintId = $("#Code").val();
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
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    if (response.Result == true) {
                        CloseForm();
                        GetServiceGuideHints();
                    }
                }
            });
        }
    })
}

function Delete(HintId, Description, DescriptionAlt) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeleteHint', '1') +'<br/> ' + Description,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.HintId = HintId;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceGuideHints();
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
    $("#DescriptionAlt").val("");
}

function clearInput2() {
    $("#DescriptionEng").val("");
    $("#DescriptionAlt").val("");
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
        html: ShowAlert('ConfirmDeleteAttribute', '1') + '<br/> ' + Description,
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
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetUserAttribute();
                }
            });
        }
    })
}

function SaveAttribute() {

    if ($("#DescriptionEng").val() == "") {
        Swal.fire({ icon: "info", text: "Please Enter SSO User Attribute" });
        $("#DescriptionEng").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: 'Do you want to Save this Attribute ' + $("#DescriptionEng").val(),
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
        Swal.fire({ icon: "info", text: "Please Enter Item Name English" });
        $("#DescriptionEng").focus();
        return false;
    }

    if ($("#DescriptionAlt").val() == "") {
        Swal.fire({ icon: "info", text: "Please Enter Item Name Arabic" });
        $("#DescriptionAlt").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: 'Do you want to Save this Item ' + $("#DescriptionEng").val(),
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
        text: 'Do you want to Delete this LookupId Item ' + DescriptionEng,
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

function Publish() {


    Swal.fire({
        icon: 'question',
        text: ShowAlert('ConfirmPublish', '1'),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax(
                {
                    type: 'POST',
                    url: $('#publishUrl').val(),
                    contentType: "application/json",
                    dataType: "json",
                    data: null,// JSON.stringify(iObject),
                    success: function (response) {
                        Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    },
                    error: function (jq, status, message) { console.log(status); },
                });

        }
    })


}

function clearSubInput() {
   
}