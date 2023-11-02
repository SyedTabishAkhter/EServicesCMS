
function GetServices() {

    var isActive = $('input[name="IsActive"]:checked').val();

    var iSearch = {};
    iSearch.EntityId = $('#EntityId').val();
    iSearch.TypeId = $('#TypeId').val();
    iSearch.SearchCri = $('#name_filter').val();
    iSearch.DepartmentId = $('#DepartmentId').val();
    iSearch.IsActive = isActive;

    var JSONdata = JSON.stringify(iSearch);
    console.log(iSearch);

    $.ajax(
        {
            type: 'POST',
            data: JSONdata,
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
            error: function (jq, status, message) { console.log(status);},
        });
}

function DeleteService(serviceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ChangeDeleteService', '1') + '<br />' + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServices();
                }
            });

        }
    })
}

function ActivateService(serviceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ChangeActivateService', '1') +"<br /> " + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#activateUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServices();
                }
            });
        }
    })
}

function DeActivateService(serviceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ChangeDeActivateService', '1') +"<br /> " + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deactivateUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServices();
                }
            });
        }
    })
}

function GetServiceEntites() {

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

function GetServiceTypes() {

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

function ResetSearch() {
    $('#EntityId').val("");
    $('#TypeId').val("");
    $('#name_filter').val("");
    GetServices();
}

function CloseForm() {
    clearInput();
    modal.style.display = "none";
}

function SaveServiceEntities() {

    var DescriptionEng = $("#DescriptionEng").val();
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Description" });
        ShowAlert("SaveServiceEntities1", "0")
        return false;
    }

    var DescriptionAlt = $("#DescriptionAlt").val();
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Description" });
        ShowAlert("SaveServiceEntities2", "0")
        return false;
    }

    var myName = DescriptionEng;
    if (language == "0" || language == 0)
        myName = DescriptionAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceEntities", "1") + " <br/>" + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.EntityId = $("#Code").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.RemarksEng = $("#RemarksEng").val();
            iObject.RemarksAlt = $("#RemarksAlt").val();
            iObject.ClassName = $("#ClassName").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    CloseForm();
                    GetServiceEntites();
                }
            });
        }
    })
}

function DeleteServiceEntities(entityId, entityName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteServiceEntities", "1") +"<br/> " + entityName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?entityId=" + entityId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceEntites();
                }
            });
        }
    })
}

function ModifyEntities(Code, DescriptionEng, DescriptionAra, RemarksEng, RemarksAlt, ClassName) {
    clearInput();
    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAra);
    $("#RemarksEng").val(RemarksEng);
    $("#RemarksAlt").val(RemarksAlt);
    $("#ClassName").val(ClassName);
    modal.style.display = "block";
}

function Modify(Code, DescriptionEng, DescriptionAra, serviceId, ExternalServiceID, PrintPreview, PrintMessage, PrintMessageAr, TabularLayout) {
    clearInput();

    if (document.getElementById("SubServiceId2"))
        $("#SubServiceId2").val(serviceId);

    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAra);
    $("#PrintMessage").val(PrintMessage);
    $("#PrintMessageAr").val(PrintMessageAr);
    $("#id-button-borders_PrintPreview").prop('checked', false);
    $("#id-button-borders_TabularLayout").prop('checked', false);
    $("#dvPrintMessage").hide();
    if (PrintPreview == "True") {
        $("#dvPrintMessage").show();
        $("#id-button-borders_PrintPreview").prop('checked', 'checked');
    }
    if (TabularLayout == "True") {
        $("#id-button-borders_TabularLayout").prop('checked', 'checked');
    }
        
    modal.style.display = "block";
}

function clearInput() {
    $("#Code").val("");
    $("#DescriptionEng").val("");
    $("#DescriptionAlt").val("");
    if ($("#id-button-borders_PrintPreview")) {
        $("#id-button-borders_PrintPreview").prop('checked', false);
        $("#dvPrintMessage").hide();
    }
    if (document.getElementById("SubServiceId2"))
        $("#SubServiceId2").val("");
    if (document.getElementById("RemarksEng"))
        $("#RemarksEng").val("");
    if (document.getElementById("RemarksAlt"))
        $("#RemarksAlt").val("");
    if(document.getElementById("ClassName"))
        $("#ClassName").val("");
}

function SaveServiceTypes() {

    var DescriptionEng = NullOrWhiteSpace($("#DescriptionEng").val());
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Description" });
        ShowAlert("SaveServiceTypes1", "0")
        return false;
    }

    var DescriptionAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Description" });
        ShowAlert("SaveServiceTypes2", "0")
        return false;
    }

    var myName = DescriptionEng;
    if (language == "0" || language == 0)
        myName = DescriptionAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceTypes", "1") + " <br/>" + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.TypeId = $("#Code").val();
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
                    CloseForm();
                    GetServiceTypes();
                }
            });
        }
    })
}

function DeleteServiceType(TypeId, TypeName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteServiceTypes", "1") +"<br/> " + TypeName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?typeId=" + TypeId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceTypes();
                }
            });
        }
    })
}

function SaveServiceDetails() {

    var Mapping = [];
    $.each($("input[name='selChoices']:checked"), function () {
        var val = { 'EntityId': $(this).val() };
        Mapping.push(val);
    });

    if (Mapping.length == 0) {
        //Swal.fire({ icon: "info", text: "Select any Service Entity !" });
        ShowAlert("SaveServiceDetails1", "0")
        return false;
    }

    var UserTypes = [];
    $.each($("input[name='selUserTypes']:checked"), function () {
        var val = { 'UserTypeId': $(this).val() };
        UserTypes.push(val);
    });

    if (UserTypes.length == 0) {
        //Swal.fire({ icon: "info", text: "Select UserType for this Service !" });
        ShowAlert("SaveServiceDetails2", "0")
        return false;
    }

    var DescriptionAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Service Arabic Description" });
        ShowAlert("SaveServiceDetails3", "0")
        return false;
    }

    var DescriptionEng = NullOrWhiteSpace($("#DescriptionEng").val());
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Service English Description" });
        ShowAlert("SaveServiceDetails4", "0")
        return false;
    }

    //var ServiceUrl = $("#ServiceUrl").val();
    //if (ServiceUrl == "") {
    //    $("#ServiceUrl").focus();
    //    Swal.fire({ icon: "info", text: "Please Enter Service Url" });
    //    return false;
    //}

    //var EntityId = $("#EntityId").val();
    //if (EntityId == "") {
    //    $("#EntityId").focus();
    //    Swal.fire({ icon: "info", text: "Select Entity" });
    //    return false;
    //}

    var PrintPreview = $("#id-button-borders_PrintPreview").is(":checked") ? 1 : 0;
    var TabularLayout = $("#id-button-borders_TabularLayout").is(":checked") ? 1 : 0;
    var IsAnonymous = $("#id-button-borders_IsAnonymous").is(":checked") ? 1 : 0;

    var TypeId = $("#TypeId").val();
    if (TypeId == "") {
        $("#TypeId").focus();
        //Swal.fire({ icon: "info", text: "Select Type" });
        ShowAlert("SaveServiceDetails5", "0")
        return false;
    }

    var DepartmentId = $("#DepartmentId").val();
    if (DepartmentId == "") {
        $("#DepartmentId").focus();
        //Swal.fire({ icon: "info", text: "Select Department" });
        ShowAlert("SaveServiceDetails6", "0")
        return false;
    }

    var ExternalServiceID = NullOrWhiteSpace($("#ExternalServiceID").val());
    if (ExternalServiceID == "") {
        $("#ExternalServiceID").focus();
        //Swal.fire({ icon: "info", text: "Please Enter External Service Id" });
        ShowAlert("SaveServiceDetails7", "0")
        return false;
    }

    if (PrintPreview == 1) {
        if (NullOrWhiteSpace($("#PrintMessage").val()) == "") {
            $("#PrintMessage").focus();
            //Swal.fire({ icon: "info", text: "Please Enter Print Message English" });
            ShowAlert("SaveServiceDetails8", "0")
            return false;
        }
        if (NullOrWhiteSpace($("#PrintMessageAr").val()) == "") {
            $("#PrintMessageAr").focus();
            //Swal.fire({ icon: "info", text: "Please Enter Print Message Arabic" });
            ShowAlert("SaveServiceDetails9", "0")
            return false;
        }
    }

    //if ($("#soWidgetCode").val() == "") {
    //    Swal.fire({ icon: "info", text: "Please Enter Widget Code" });
    //    $("#soWidgetCode").focus();
    //    return false;
    //}

    //if ($("#ApiSourceId").val() == "") {
    //    Swal.fire({ icon: "info", text: "Please Select API Source" });
    //    $("#ApiSourceId").focus();
    //    return false;
    //}

    var myName = DescriptionEng;

    if (language == "0" || language == 0)
        myName = DescriptionAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceDetails", "1") + "<br/>" + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.ServiceId = $("#ServiceId").val();
            iObject.ParentServiceId = $("#ParentServiceId").val();
            iObject.EntityId = $("#EntityId").val();
            iObject.TypeId = $("#TypeId").val();
            iObject.ExternalServiceID = $("#ExternalServiceID").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.SortOrder = $("#SortOrder").val();
            iObject.Mapping = Mapping; 
            iObject.UserTypes = UserTypes;
            iObject.PrintPreview = PrintPreview;
            iObject.TabularLayout = TabularLayout;
            iObject.IsAnonymous = IsAnonymous;
            iObject.soWidgetCode = $("#soWidgetCode").val();
            iObject.ApiSourceId = $("#ApiSourceId").val(); 
            iObject.DepartmentId = $("#DepartmentId").val();

            iObject.PrintMessage = $("#PrintMessage").val();
            iObject.PrintMessageAr = $("#PrintMessageAr").val();

            iObject.CommentAttachments = {};
            iObject.CommentAttachments.Minimum = $("#Minimum").val();
            iObject.CommentAttachments.Maximum = $("#Maximum").val();
            iObject.CommentAttachments.MaxSize = $("#MaxSize").val();

            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    //Swal.fire(response.Message)
                    //if (response.Result == true)
                    //    window.top.location.href = response.Redirect;

                    Swal.fire({
                        icon: response.Icon,
                        text: response.Message,
                        confirmButtonText: ShowAlert('OkButton', '1') ,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            if (response.Result == true)
                                window.top.location.href = response.Redirect;
                        }
                    })
                }
            });
        }
    })
}

function GetServiceClassifications(serviceId) {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId,
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

function SaveServiceClassification() {

    var serviceId = $("#SubServiceId2").val();
    if (serviceId == "") {
        Swal.fire({ icon: "info", text: "Please Select Service" });
        ShowAlert("SaveServiceClassification1", "0")
        $("#SubServiceId2").focus();
        return false;
    }
    if (NullOrWhiteSpace($("#DescriptionEng").val()) == "") {
        Swal.fire({ icon: "info", text: "Please Enter Service English Classification" });
        ShowAlert("SaveServiceClassification2", "0")
        $("#DescriptionEng").focus();
        return false;
    }
    if (NullOrWhiteSpace($("#DescriptionAlt").val()) == "") {
        Swal.fire({ icon: "info", text: "Please Enter Service Arabic Classification" });
        ShowAlert("SaveServiceClassification3", "0")
        $("#DescriptionAlt").focus();
        return false;
    }
    var PrintPreview = $("#id-button-borders_PrintPreview").is(":checked") ? 1 : 0;
    if (PrintPreview == 1) {
        if (NullOrWhiteSpace($("#PrintMessage").val()) == "") {
            $("#PrintMessage").focus();
            Swal.fire({ icon: "info", text: "Please Enter Print Message English" });
            ShowAlert("SaveServiceClassification4", "0")
            return false;
        }
        if (NullOrWhiteSpace($("#PrintMessageAr").val()) == "") {
            $("#PrintMessageAr").focus();
            Swal.fire({ icon: "info", text: "Please Enter Print Message Arabic" });
            ShowAlert("SaveServiceClassification5", "0")
            return false;
        }
    }

    var TabularLayout = $("#id-button-borders_TabularLayout").is(":checked") ? 1 : 0;

    var myName = $("#DescriptionEng").val();
    if (language == "0" || language == 0)
        myName = $("#DescriptionAlt").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceClassification", "1") + "<br/> " + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.ServiceId = serviceId;
            iObject.CategoryId = $("#Code").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.PrintPreview = PrintPreview;
            iObject.PrintMessage = $("#PrintMessage").val();
            iObject.PrintMessageAr = $("#PrintMessageAr").val();
            iObject.TabularLayout = TabularLayout;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    //Swal.fire(response.Message)
                    Swal.fire({
                        icon:response.Icon,
                        text: response.Message,
                        confirmButtonText: ShowAlert('OkButton', '1') ,
                    }).then((result) => {
                        if (result.isConfirmed) {

                            if (response.Result == true) {
                                CloseForm();
                                window.top.location.reload(true);
                            }
                        }
                    })
                }
            });
        }
    })
}

function DeleteServiceClassification(serviceId, categoryId, categoryName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeleteServiceClassification', '1') + "<br/> " + categoryName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceClassifications(serviceId);
                }
            });
        }
    })
}

function ActivateServiceClassification(serviceId, categoryId, categoryName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmActServiceClassification', '1') +"<br/> " + categoryName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#activateUrl").val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceClassifications(serviceId);
                }
            });
        }
    })
}

function DeActivateServiceClassification(serviceId, categoryId, categoryName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeActServiceClassification', '1') + "<br/> " + categoryName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deactivateUrl").val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceClassifications(serviceId);
                }
            });
        }
    })
}

function GetServiceInputs(serviceId) {

    var categoryId = $("#CategoryId").val();
    if (categoryId == null || categoryId == "" || categoryId == undefined)
        categoryId = "0";

    var subServiceId = $("#SubServiceId").val();
    if (subServiceId != null && subServiceId != "" && subServiceId != undefined)
        serviceId = subServiceId;

    var inputTypeId = $("#InputTypeId").val();
    if (inputTypeId == null || inputTypeId == "" || inputTypeId == undefined)
        inputTypeId = "0";

    var tabId = $("#TabId").val();
    if (tabId == null || tabId == "" || tabId == undefined)
        tabId = "0";

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId + "&inputTypeId=" + inputTypeId + "&tabId=" + tabId,
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

function ShowHideInputs() {

    var inputTypeId = $("#InputTypeId").val();
    console.log(inputTypeId)

    $("#dvReadonly").show();
    $("#dvMaxMin").show();
    $("#dvPhEn").show();
    $("#dvPhAr").show();
    $("#dvVmEn").show();
    $("#dvVmAr").show();
    $("#dvIsRequired").show();

    switch (inputTypeId) {

        case "1"://text
            console.log("in 1 2 11");
            $("#dvUserField").show();
            //$("#ExternalLookupId").val("");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();
            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").show();
            $("#dvSpeechToText").show();

            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();
            $("#MessageAr").val("");
            $("#dvMessageAr").hide();
            $("#dvEnglishInput").show();
            $("#dvArabicInput").show();
            $("#dvDynamicInput").show();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();
            break;

        case "2"://email
        case "11"://tel
            console.log("in 1 2 11");
            $("#dvUserField").show();
            //$("#ExternalLookupId").val("");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();
            $("#dvIsExternalLookup").hide();
            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide();
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);


            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();
            $("#MessageAr").val("");
            $("#dvMessageAr").hide();
            $("#dvArabicInput").hide();
            $("#dvEnglishInput").hide();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();
            break;

        case "4"://ddl
        case "7"://radio
        case "8"://checkbx
            console.log("in 4 7 8");
            $("#dvExternalLookup").show();
            $("#dvExternalLookupActions").show();
            $("#dvIsExternalLookup").show();
            $("#dvLookupFilterId").show();
            $("#dvLookupFilterValue").show();
            $("#UserField").val("");
            $("#dvUserField").hide();
            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide();
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);

            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();
            $("#MessageAr").val("");
            $("#dvMessageAr").hide();
            $("#dvArabicInput").show();
            $("#dvEnglishInput").show();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();

            /*LoadServiceSelectTypes();*/
            break;

        case "20"://single checkbx
            console.log("in 4 7 8");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();
            $("#UserField").val("");
            $("#dvUserField").hide();
            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide()
            $("#dvJsonAttribute").show();
            $("#dvReadonly").hide();
            
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);

            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();
            $("#MessageAr").val("");
            $("#dvMessageAr").hide();
            $("#dvArabicInput").hide();
            $("#dvEnglishInput").hide();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();

            /*LoadServiceSelectTypes();*/
            break;

        case "6"://attachment
            console.log("in 6");
            $("#dvUserField").hide();
            $("#UserField").val("");
            //$("#ExternalLookupId").val("");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();
            $("#dvMaxFileSize").show();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide();
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);

            //$('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").show();

            $("#dvMessage").show();
            $("#dvMessageAr").show();
            $("#dvArabicInput").hide();
            $("#dvEnglishInput").hide();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();
            break;

        case "5"://textarea
            console.log("in 5");
            $("#dvArabicInput").show();
            $("#dvEnglishInput").show();
            $("#dvDynamicInput").hide();
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();
            $("#dvApplyWordCount").show();
            $("#dvSpeechToText").show();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();

            $("#UserField").val("");
            $("#dvUserField").hide();

            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();

            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();

            $("#MessageAr").val("");
            $("#dvMessageAr").hide();
            $("#dvPhVm").show();
            break;

        case "14":
        case "16":
        case "17":
        case "18":
        case "19":
            console.log("in LABEL CONTROL");
            //$("#ExternalLookupId").val("");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();

            $("#UserField").val("");
            $("#dvUserField").hide();

            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide();
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);


            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();

            $("#MessageAr").val("");
            $("#dvMessageAr").hide();

            $("#dvArabicInput").hide();
            $("#dvEnglishInput").hide();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvReadonly").hide();
            $("#dvMaxMin").hide();
            $("#dvPhEn").hide();
            $("#dvPhAr").hide();
            $("#dvVmEn").hide();
            $("#dvVmAr").hide();
            $("#dvIsRequired").hide();


            break;

        default:
            console.log("in default");
            //$("#ExternalLookupId").val("");
            $("#dvExternalLookup").hide();
            $("#dvExternalLookupActions").hide();
            $("#dvIsExternalLookup").hide();
            $("#dvLookupFilterId").hide();
            $("#dvLookupFilterValue").hide();

            $("#UserField").val("");
            $("#dvUserField").hide();

            $("#MaxFileSize").val("");
            $("#dvMaxFileSize").hide();
            $("#dvApplyWordCount").hide();
            $("#dvSpeechToText").hide();
            $("#id-button-borders_ApplyWordCount").prop("checked", false);
            $("#id-button-borders_SpeechToText").prop("checked", false);

            
            $('input[name=DownloadAttachment][value=false]').prop('checked', true)
            $("#dvDownloadAttachment").hide();

            $("#Message").val("");
            $("#dvMessage").hide();

            $("#MessageAr").val("");
            $("#dvMessageAr").hide();

            $("#dvArabicInput").hide();
            $("#dvEnglishInput").hide();
            $("#dvDynamicInput").hide();

            $("#ReferralId").val("");
            $("#LogicalOperator").val("");
            $("#ReferralIdValue").val("");
            $("#dvDrilldownFilters").hide();
            $("#dvPhVm").show();
            

            break;
    }
}

function SaveForm() {

    var SubServiceId = $("#SubServiceId").val();
    if (SubServiceId == "") {
        $("#SubServiceId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Service" });
        ShowAlert("SaveForm1", "0")
        return false;
    }

    if ($("#CategoryId").is(":visible")) {
        var CategoryId = $("#CategoryId").val();
        if (CategoryId == "0" || CategoryId == "" || CategoryId == null || CategoryId == undefined) {
            $("#CategoryId").focus();
            //Swal.fire({ icon: "info", text: "Please Select CLASSIFICATION " });
            ShowAlert("SaveForm2", "0")
            return false;
        }
    }    

    var inputTypeId = $("#InputTypeId").val();
    if (inputTypeId == "") {
        $("#InputTypeId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Field Type" });
        ShowAlert("SaveForm3", "0")
        return false;
    }

    if (checkInputs() == false)
        return false;

    var englishInput = "";
    var arabicInput = "";
    var dynamicInput = "";
    var isExtLookup = false;
    var FilterId = "";
    var FilterValue = "";
    var downloadAttachment = false;
    var applyWordCount = false;
    var SpeechToText = false;
    var IsReadOnly = $('input[name="IsReadOnly"]:checked').val();


    var TabId = null;
    if ($("#TabId") != null) {
        TabId = $("#TabId").val();
        if (TabId == "") {
            $("#TabId").focus();
            //Swal.fire({ icon: "info", text: "Please Select Tab on filed to appear." });
            ShowAlert("SaveForm4", "0")
            return false;
        }
    }

    var inputTypeId = $("#InputTypeId").val();
    switch (inputTypeId) {

        case "1":
        case "5":

            englishInput = $("#EnglishInput").val();
            if (englishInput == "") {
                $("#EnglishInput").focus();
                //Swal.fire({ icon: "info", text: "Please Select English Input" });
                ShowAlert("SaveForm5", "0")
                return false;
            }
            arabicInput = $("#ArabicInput").val();
            if (arabicInput == "") {
                $("#ArabicInput").focus();
                //Swal.fire({ icon: "info", text: "Please Select Arabic Input" });
                ShowAlert("SaveForm6", "0")
                return false;
            }
            dynamicInput = $("#DynamicInput").val();
            if (dynamicInput == "") {
                dynamicInput = "0";
            }

            var isChecked = $("#id-button-borders_ApplyWordCount").is(":checked") ? 1 : 0;
            if (isChecked == 1) {
                applyWordCount = true;

                if ($("#Minimum").val() == "" || parseInt($("#Minimum").val()) <= 0) {
                    $("#Minimum").focus();
                    //Swal.fire({ icon: "info", text: "Please Enter the Minimum Value for Word Count" });
                    ShowAlert("SaveForm7", "0")
                    return false;
                }
                if ($("#Maximum").val() == "" || parseInt($("#Maximum").val()) <= 0) {
                    $("#Maximum").focus();
                    //Swal.fire({ icon: "info", text: "Please Enter the Maximum Value for Word Count" });
                    ShowAlert("SaveForm8", "0")
                    return false;
                }
            }

            var isCheckedS2T = $("#id-button-borders_SpeechToText").is(":checked") ? 1 : 0;
            if (isCheckedS2T == 1) {
                SpeechToText = true;
            }

            break;
        case "4":
        case "7":
        case "8":
            if ($("#IsExternalLookup").val() == "") {
                $("#IsExternalLookup").focus();
                //Swal.fire({ icon: "info", text: "Please Select Is External Lookup ?" });
                ShowAlert("SaveForm9", "0")
                return false;
            }
            if ($("#ExternalLookupId").val() == "") {
                $("#ExternalLookupId").focus();
                //Swal.fire({ icon: "info", text: "Please Select External Lookup Id" });
                ShowAlert("SaveForm10", "0")
                return false;
            }

            if ($("#IsExternalLookup").val() == "1") {
                isExtLookup = true;
            }

            FilterId = $("#FilterId").val();
            FilterValue = $("#FilterValue").val();

            if (FilterId != "0" && FilterValue == "")
            {
                //Swal.fire({ icon: "info", text: "Please enter Filter Value" });
                ShowAlert("SaveForm11", "0")
                return false;
            }
            if (FilterId == "0" && FilterValue != "") {
                //Swal.fire({ icon: "info", text: "Please Select Filter Id" });
                ShowAlert("SaveForm12", "0")
                return false;
            }

            englishInput = $("#EnglishInput").val();
            if (englishInput == "") {
                $("#EnglishInput").focus();
                //Swal.fire({ icon: "info", text: "Please Select English Input" });
                ShowAlert("SaveForm13", "0")
                return false;
            }
            arabicInput = $("#ArabicInput").val();
            if (arabicInput == "") {
                $("#ArabicInput").focus();
                //Swal.fire({ icon: "info", text: "Please Select Arabic Input" });
                ShowAlert("SaveForm15", "0")
                return false;
            }
            dynamicInput = $("#DynamicInput").val();
            if (dynamicInput == "") {
                dynamicInput = "0";
            }

            break;

        case "6":
            if ($("#MaxFileSize").val() == "") {
                $("#MaxFileSize").focus();
                //Swal.fire({ icon: "info", text: "Please set Maximum File Size for Attachments" });
                ShowAlert("SaveForm14", "0")
                return false;
            }

            downloadAttachment = $('input[name="DownloadAttachment"]:checked').val();
            break;
    }

    var myName = $("#Label").val();
    if (language == "0" || language == 0)
        myName = $("#LabelAr").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveForm", "1") + "<br/> " + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var Required = $('input[name="Required"]:checked').val();

            var iObject = {};

            if ($("#dvJsonAttribute").is(":visible")) {
                var JsonAttribute = $("#JsonAttribute").val();
                if (JsonAttribute == "" || JsonAttribute == null || JsonAttribute == undefined) {
                    $("#JsonAttribute").focus();
                    //Swal.fire({ icon: "info", text: "Please Enter JsonAttribute " });
                    ShowAlert("SaveForm16", "0")
                    return false;
                }
                else {
                    iObject.JsonAttribute = JsonAttribute;
                }
            }

            iObject.InputId = $("#InputId").val();

            if (SubServiceId != null && SubServiceId != "" && SubServiceId != undefined)
                iObject.ServiceId = SubServiceId;
            else
                iObject.ServiceId = $("#ServiceId").val();

            if ($("#CategoryId"))
                iObject.CategorId = $("#CategoryId").val();

            iObject.InputTypeId = $("#InputTypeId").val();
            iObject.SortOrder = $("#SortOrder").val();
            iObject.Required = Required;
            iObject.DownloadAttachment = downloadAttachment;
            iObject.ArabicInput = arabicInput;
            iObject.EnglishInput = englishInput;
            iObject.DynamicInput = dynamicInput;
            iObject.Label = $("#Label").val();
            iObject.LabelAr = $("#LabelAr").val();
            iObject.Placeholder = $("#Placeholder").val();
            iObject.PlaceholderAr = $("#PlaceholderAr").val();
            iObject.Message = $("#Message").val();
            iObject.MessageAr = $("#MessageAr").val();
            iObject.UserField = $("#UserField").val();
            iObject.ExternalLookupId = $("#ExternalLookupId").val();
            iObject.IsExternalLookup = isExtLookup;
            iObject.Minimum = $("#Minimum").val();
            iObject.Maximum = $("#Maximum").val();
            iObject.MaxFileSize = $("#MaxFileSize").val();
            //iObject.ReferralId = $("#ReferralId").val();
            //iObject.LogicalOperator = $("#LogicalOperator").val();
            //iObject.ReferralIdValue = $("#ReferralIdValue").val();

            iObject.ValidationMessage = $("#ValidationMessage").val();
            iObject.ValidationMessageAr = $("#ValidationMessageAr").val();
            iObject.HelpMessage = $("#HelpMessage").val();
            iObject.HelpMessageAr = $("#HelpMessageAr").val();
            iObject.Bookmark = $("#Bookmark").val();
            iObject.IsReadOnly = IsReadOnly;
            iObject.ApplyWordCount = applyWordCount;
            iObject.TabId = TabId;
            iObject.FilterId = FilterId;
            iObject.FilterValue = FilterValue;
            iObject.SpeechToText = SpeechToText;

            console.log(JSON.stringify(iObject));
            
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    //Swal.fire(response.Message)
                    //if (response.Result == true)
                    //    window.top.location.href = response.Redirect;

                    Swal.fire({
                        icon:response.Icon,
                        text: response.Message,
                        confirmButtonText: ShowAlert('OkButton', '1') ,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            if (response.Result == true)
                                window.top.location.href = response.Redirect;
                        }
                    })

                }
            });
        }
    })
}

function DeleteServiceInput(inputId, Label) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeleteServiceInput', '1') + "<br/> " + Label,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?inputId=" + inputId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceInputs(response.ServiceId);
                }
            });
        }
    })
}

function ActivateServiceInput(inputId, Label) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDActivateServiceInput', '1') +"<br/> " + Label,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#activateUrl").val() + "?inputId=" + inputId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceInputs(response.ServiceId);
                }
            });
        }
    })
}

function DeActivateServiceInput(inputId, Label) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeActivateServiceInput', '1') +"<br/> " + Label,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deactivateUrl").val() + "?inputId=" + inputId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceInputs(response.ServiceId);
                }
            });
        }
    })
}

function clearSubInput() {
    $("#Code").val("");
    $("#SubDescriptionEng").val("");
    $("#SubDescriptionAlt").val("");
    $("#SubExternalServiceID").val("");
    $("#soWidgetCode").val("");
    //$('input:radio[name="UseParentExternalServiceId"]').attr('checked', false);
}

function SaveSubService() {

    if (NullOrWhiteSpace($("#SubDescriptionEng").val()) == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Service English Description" });
        ShowAlert("SaveSubService1", "0")
        $("#SubDescriptionEng").focus();
        return false;
    }
    if (NullOrWhiteSpace($("#SubDescriptionAlt").val()) == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Service Arabic Description" });
        ShowAlert("SaveSubService2", "0")
        $("#SubDescriptionAlt").focus();
        return false;
    }
    if (NullOrWhiteSpace($("#SubExternalServiceID").val()) == "") {
        //Swal.fire({ icon: "info", text: "Please Enter External Service Id" });
        ShowAlert("SaveSubService3", "0")
        $("#SubExternalServiceID").focus();
        return false;
    }
    /*if (NullOrWhiteSpace($("#soWidgetCode").val()) == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Widget Code" });
        ShowAlert("SaveSubService4", "0")
        $("#soWidgetCode").focus();
        return false;
    }*/

    var useParentExternalServiceId = $('input[name="UseParentExternalServiceId"]:checked').val();

    var saveUrl = $("#createSubServiceUrl").val();
    if ($("#Code").val() != "") {
        saveUrl = $("#updateSubServiceUrl").val();
    }

    var PrintPreview = $("#id-button-borders_PrintPreview").is(":checked") ? 1 : 0;
    if (PrintPreview == 1) {
        if (NullOrWhiteSpace($("#PrintMessage").val()) == "") {
            $("#PrintMessage").focus();
            //Swal.fire({ icon: "info", text: "Please Enter Print Message English" });
            ShowAlert("SaveSubService5", "0")
            return false;
        }
        if (NullOrWhiteSpace($("#PrintMessageAr").val()) == "") {
            $("#PrintMessageAr").focus();
            //Swal.fire({ icon: "info", text: "Please Enter Print Message Arabic" });
            ShowAlert("SaveSubService6", "0")
            return false;
        }
    }

    var myName = $("#SubDescriptionEng").val();
    if (language == "0" || language == 0)
        myName = $("#SubDescriptionAlt").val();

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveSubService", "1") + "<br/> " + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.ServiceId = $("#Code").val();
            iObject.ParentServiceId = $("#ParentCode").val();
            iObject.ExternalServiceID = $("#SubExternalServiceID").val();
            iObject.DescriptionEng = $("#SubDescriptionEng").val();
            iObject.DescriptionAlt = $("#SubDescriptionAlt").val();
            iObject.UseParentExternalServiceId = useParentExternalServiceId;
            iObject.PrintPreview = PrintPreview;
            iObject.soWidgetCode = $("#soWidgetCode").val();
            iObject.PrintMessage = $("#PrintMessage").val();
            iObject.PrintMessageAr = $("#PrintMessageAr").val();

            iObject.CommentAttachments = {};
            iObject.CommentAttachments.Minimum = $("#Minimum").val();
            iObject.CommentAttachments.Maximum = $("#Maximum").val();
            iObject.CommentAttachments.MaxSize = $("#MaxSize").val();

            console.log(saveUrl);
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: saveUrl,
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    //Swal.fire(response.Message)
                    //if (response.Result == true)
                    //    window.top.location.reload(true);

                    Swal.fire({
                        icon:response.Icon,
                        text: response.Message,
                        confirmButtonText: ShowAlert('OkButton', '1') ,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            if (response.Result == true)
                                window.top.location.reload(true);
                        }
                    })
                }
            });
        }
    })
}

function GetSubServices(serviceId) {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId,
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

function ModifySubService(ServiceId, DescriptionEng, DescriptionAra, ExternalServiceID, UseParentExternalServiceId, serviceId, Minimum, Maximum, MaxSize, PrintPreview, soWidgetCode,PrintMessage,PrintMessageAr) {

    clearSubInput();

    $("#Code").val(ServiceId);
    $("#SubDescriptionEng").val(DescriptionEng);
    $("#SubDescriptionAlt").val(DescriptionAra);
    $("#SubExternalServiceID").val(ExternalServiceID);
    $("#soWidgetCode").val(soWidgetCode);

    if (UseParentExternalServiceId == "True") {
        $('input[name=UseParentExternalServiceId][value=true]').prop('checked', true)
        //$('input:radio[name="UseParentExternalServiceId"]').attr('checked', 'checked');
    }
    else
        $('input[name=UseParentExternalServiceId][value=false]').prop('checked', true)
       

    $("#Minimum").val(Minimum);
    $("#Maximum").val(Maximum);
    $("#MaxSize").val(MaxSize);
    $("#PrintMessage").val(PrintMessage);
    $("#PrintMessageAr").val(PrintMessageAr);
   
    $("#id-button-borders_PrintPreview").prop('checked', false);
    $("#dvPrintMessage").hide();
    if (PrintPreview == "True") {
        $("#id-button-borders_PrintPreview").prop('checked', 'checked');
        $("#dvPrintMessage").show();
    }
    
    modal.style.display = "block";
}

function DeleteSubService(serviceId, parentServiceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeleteSubService', '1') + "<br/> " + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetSubServices(parentServiceId);
                }
            });
        }
    })
}

function ActivateSubService(serviceId, parentServiceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmActivateSubService', '1') +"<br/> " + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#activateUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetSubServices(parentServiceId);
                }
            });
        }
    })
}

function DeActivateSubService(serviceId, parentServiceId, serviceName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeActivateSubService', '1') +"<br/> " + serviceName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deactivateUrl").val() + "?serviceId=" + serviceId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetSubServices(parentServiceId);
                }
            });
        }
    })
    
}

function GetSubServiceClassifications(serviceId) {

    var subServiceId = $("#SubServiceId").val();
    if (subServiceId != null && subServiceId != "") {
        GetServiceClassifications(subServiceId);
    }
    else {
        GetServiceClassifications(serviceId);
    }
}

function GetServiceClassificationsList(serviceId) {

    var subServiceId = $("#SubServiceId").val();
    if (subServiceId != null && subServiceId != "") {

        $("#CategoryId").empty();

        var select2 = $("#CategoryId");

        var url = $("#loadClassificationUrl").val();

        $.getJSON(url, { serviceId: subServiceId },
            function (data) {
                if (data == null)
                    return;

            select2.append($('<option/>', {
                value: 0,
                text: "Select Classification"
            }));
            $.each(data, function (index, itemData) {
                select2.append($('<option/>', {
                    value: itemData.Value,
                    text: itemData.Text,
                }));
            });
        });

        $("#CategoryId").trigger('change');

        GetServiceInputs(subServiceId);
    }
    else {
        GetServiceInputs(serviceId);
    }
}

function LoadServiceSelectTypes(selVal) {

    modal.style.display = "block";

    var serviceId1 = $("#ServiceId").val();
    var subServiceId1 = $("#SubServiceId").val();
    var categoryId1 = $("#CategoryId").val();

    if (categoryId1 == "" || categoryId1 == null || categoryId1 == undefined) {
        categoryId1 = "0";
    }

    if (subServiceId1 != "" && subServiceId1 != null && subServiceId1 != undefined) {
        serviceId1 = subServiceId1;
    }

    if (serviceId1 == "" || serviceId1 == null || serviceId1 == undefined) {
        return;
    }

    var InputId = $("#InputId").val();
    if (InputId == "" || InputId == null || InputId == undefined) {
        InputId = "0";
    }

    $("#ReferralId").empty();
    $("#ReferralIdValue").val("");
    $("#LogicalOperator").val("");

    var select2 = $("#ReferralId");

    var url = $("#loadServiceSelectTypesUrl").val();

    $.getJSON(url, { serviceId: serviceId1, categoryId: categoryId1, inputId: InputId},
        function (data) {
            select2.append($('<option/>', {
                value: 0,
                text: "Select Referral Id"
            }));
            $.each(data, function (index, itemData) {
                select2.append($('<option/>', {
                    value: itemData.Value,
                    text: itemData.Text,
                }));
            });
        });
    return;
}

function LoadFilterLookupTypes() {

    setTimeout('LoadFilterLookupTypes2();', 2000);
}

function LoadFilterLookupTypes2(selVal) {


    $("#FilterId").empty();
    
    var serviceId1 = $("#ServiceId").val();
    var subServiceId1 = $("#SubServiceId").val();
    var categoryId1 = $("#CategoryId").val();

    if (categoryId1 == "" || categoryId1 == null || categoryId1 == undefined) {
        categoryId1 = "0";
    }

    if (subServiceId1 != "" && subServiceId1 != null && subServiceId1 != undefined) {
        serviceId1 = subServiceId1;
    }

    if (serviceId1 == "" || serviceId1 == null || serviceId1 == undefined) {
        return;
    }

    var InputId = $("#InputId").val();
    if (InputId == "" || InputId == null || InputId == undefined) {
        InputId = "0";
    }

    var select2 = $("#FilterId");

    var url = $("#loadServiceSelectTypesUrl").val();

    $.getJSON(url, { serviceId: serviceId1, categoryId: categoryId1, inputId: InputId },
        function (data) {
            select2.append($('<option/>', {
                value: 0,
                text: "-Filter-"
            }));
            $.each(data, function (index, itemData) {
                select2.append($('<option/>', {
                    value: itemData.Value,
                    text: itemData.Text,
                }));
            });
        });
    console.log(FilterId);
    setTimeout('$("#FilterId").val(FilterId)', 1000);
    return;
}

function GetServiceTabs(serviceId) {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId,
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

function SaveServiceTabs() {

    var DescriptionEng = NullOrWhiteSpace($("#DescriptionEng").val());
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Description" });
        ShowAlert("SaveServiceTabs1", "0")
        return false;
    }

    var DescriptionAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Description" });
        ShowAlert("SaveServiceTabs2", "0")
        return false;
    }

    var myName = DescriptionEng;
    if (language == "0" || language == 0)
        myName = DescriptionAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceTabs", "1") + "<br/> " + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.TabId = $("#Code").val();
            iObject.ServiceId = $("#ServiceId").val();
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
                    CloseForm();
                    GetServiceTabs($("#ServiceId").val());
                }
            });
        }
    })
}

function DeleteServiceTab(TypeId, TypeName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteServiceTabs", "1") +"<br/> " + TypeName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?tabId=" + TypeId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceTabs($("#ServiceId").val());
                }
            });
        }
    })
}

function ModifyTab(Code, DescriptionEng, DescriptionAra) {
    clearInput();

    $("#Code").val(Code);
    $("#DescriptionEng").val(DescriptionEng);
    $("#DescriptionAlt").val(DescriptionAra);

    modal.style.display = "block";
}

function GetInputDrilldown(inputId) {

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxDrldwnUrl').val() + "?InputControlId=" + inputId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $("#dvData2").empty();
                    $("#dvData2").html(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function SaveInputDrilldown() {

    if ($("#Code").val() == "0") {
        $("#ReferralId").focus();
        //Swal.fire({ icon: "info", text: "Please Save the form and then create drilldown filter." });
        ShowAlert("SaveInputDrilldown1", "0")
        return false;
    }

    var ReferralId = $("#ReferralId").val();
    if (ReferralId == "" || ReferralId == "0") {
        $("#ReferralId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Referral Id" });
        ShowAlert("SaveInputDrilldown2", "0")
        return false;
    }

    var LogicalOperator = $("#LogicalOperator").val();
    if (LogicalOperator == "") {
        $("#LogicalOperator").focus();
        //Swal.fire({ icon: "info", text: "Please Select Logical Operator" });
        ShowAlert("SaveInputDrilldown3", "0")
        return false;
    }

    var ReferralIdValue = NullOrWhiteSpace($("#ReferralIdValue").val());
    if (ReferralIdValue == "") {
        $("#ReferralIdValue").focus();
        //Swal.fire({ icon: "info", text: "Please Eter Referral Id Value" });
        ShowAlert("SaveInputDrilldown4", "0")
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmSaveInputDrilldown", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.DrilldownId = $("#DrilldownId").val();
            iObject.InputId = $("#Code").val();
            iObject.InputControlId = $("#InputControlId").val();
            iObject.ReferralId = ReferralId;
            iObject.LogicalOperator = LogicalOperator;
            iObject.ReferralIdValue = ReferralIdValue;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveDrldwnUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    $("#DrilldownId").val("0");
                    $("#ReferralId").val("");
                    $("#LogicalOperator").val("");
                    $("#ReferralIdValue").val("");
                    modal.style.display = "none";
                    GetInputDrilldown($("#InputControlId").val());
                }
            });
        }
    })
}

function DeleteInputDrilldown(did, didName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteInputDrilldown", "1") + "<br/> " + didName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteDrldwnUrl").val() + "?did=" + did,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetInputDrilldown($("#InputControlId").val());
                }
            });
        }
    })
}

function ModifyDrilldown(Code, ReferralName, ReferralId, LogicalOperator, ReferralIdValue) {


    var serviceId1 = $("#ServiceId").val();
    var subServiceId1 = $("#SubServiceId").val();
    var categoryId1 = $("#CategoryId").val();

    if (categoryId1 == "" || categoryId1 == null || categoryId1 == undefined) {
        categoryId1 = "0";
    }

    if (subServiceId1 != "" && subServiceId1 != null && subServiceId1 != undefined) {
        serviceId1 = subServiceId1;
    }

    if (serviceId1 == "" || serviceId1 == null || serviceId1 == undefined) {
        return;
    }

    $("#ReferralId").empty();
    $("#ReferralIdValue").val("");
    $("#LogicalOperator").val("");

    var select2 = $("#ReferralId");

    var url = $("#loadServiceSelectTypesUrl").val();

    $.getJSON(url, { serviceId: serviceId1, categoryId: categoryId1 },
        function (data) {
            select2.append($('<option/>', {
                value: 0,
                text: "Select Referral Id"
            }));
            $.each(data, function (index, itemData) {
                select2.append($('<option/>', {
                    value: itemData.Value,
                    text: itemData.Text,
                }));
            });

            $("#DrilldownId").val(Code);
            $("#ReferralId").val(ReferralId);
            $("#LogicalOperator").val(LogicalOperator);
            $("#ReferralIdValue").val(ReferralIdValue);
        });

    ////$("#DrilldownId").val(Code);
    ////$("#ReferralId").val(ReferralId);
    ////$("#LogicalOperator").val(LogicalOperator);
    ////$("#ReferralIdValue").val(ReferralIdValue);

    

    modal.style.display = "block";
}

function GetLookupValues() {

    if ($("#ExternalLookupId").val() == "") {
        //Swal.fire({ icon: 'error', text: 'Please select any lookup' });
        ShowAlert("GetLookupValues1", "0")
        return false;
    }

    var actionId = null;
    if ($("#ActionId").val() != "")
        actionId = $("#ActionId").val();

    var filterValue = "";
    if ($("#FilterValue").val() != "")
        filterValue = $("#FilterValue").val();

    $.ajax({
        type: 'POST',
        //url: $('#lookupActionUrl').val() + "?inputId=" + $("#InputId").val() + "&lookupId=" + $("#ExternalLookupId").val() + "&actionId=" + actionId + "&filterValue=" + filterValue,
        url: $('#lookupActionUrl').val() + "?inputId=" + $("#InputControlId").val() + "&lookupId=" + $("#ExternalLookupId").val() + "&actionId=" + actionId + "&filterValue=" + filterValue,
        contentType: "application/json; charset=utf-8",
        data: null,
        success: function (response) {
            console.log(response);
            $("#dvPermissions").html(response.html);
            if (response.actionId != "")
                $("#ActionId").val(response.actionId);
            //if (response.remarks != "")
            //    $("#ActionRemarksEng").val(response.remarks);
            //if (response.remarksAr != "")
            //    $("#ActionRemarksAlt").val(response.remarksAr);
            lkmodal.style.display = "block";
        }
    }, "json");
}

function CloseFormLkAction() {
    clearLkActionInput();
    lkmodal.style.display = "none";
}

function clearLkActionInput() {
    $("#ActionId").val("");
    //$("#ActionRemarksEng").val("");
    //$("#ActionRemarksAlt").val("");
    $("input[name='switch-field-1" + $("#ExternalLookupId").val() + "']").prop('checked', false);
}

function SaveLookupAction() {

    if ($("#ActionId").val() == "") {
        //Swal.fire({ icon: "info", text: "Please select Action" });
        ShowAlert("SaveLookupAction1", "0")
        $("#ActionId").focus();
        return false;
    }
    var lookupValues = [];

    if (document.getElementsByName('switch-field-1' + $("#ExternalLookupId").val())) {
        $.each($("input[name='switch-field-1" + $("#ExternalLookupId").val() + "']:checked"), function () {
            var val = {
                'Code': $(this).val(), 'DescriptionAlt': $("#DescriptionAlt" + $(this).val()).val(), 'DescriptionEng': $("#DescriptionEng" + $(this).val()).val()
            };
            lookupValues.push(val);
        });
    }

    console.log(lookupValues);

    if (lookupValues.length <= 0) {
        //Swal.fire({ icon: "info", text: "Select Lookup Values for this Action." });
        ShowAlert("SaveLookupAction2", "0")
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmSaveLookupAction", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};

            iObject.InputId = $("#InputId").val();
            iObject.LookupId = $("#ExternalLookupId").val();
            iObject.ActionId = $("#ActionId").val();
            iObject.InputControlId = $("#InputControlId").val();
            //iObject.DescriptionAlt = $("#ActionRemarksEng").val();
            //iObject.DescriptionEng = $("#ActionRemarksAlt").val();
            iObject.LookupValues = lookupValues;
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveLookupActionUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    if (response.Result == true) {
                        CloseFormLkAction();
                    }
                }
            });
        }
    })
}

function DeleteLkLookUpAction() {

    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmDeleteLookupAction", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                //url: $("#deleteLookupActionUrl").val() + "?inputId=" + $("#InputId").val() + "&lookupId=" + $("#ExternalLookupId").val(),
                url: $("#deleteLookupActionUrl").val() + "?inputId=" + $("#InputControlId").val() + "&lookupId=" + $("#ExternalLookupId").val(),
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    CloseFormLkAction();
                }
            });
        }
    })
}

function ChangeSortOrder(inputId,controlId) {

    var sortOrder = $("#" + controlId).val();
    $.ajax({
        type: 'POST',
        url: $("#changeSortOrderUrl").val() + "?inputId=" + inputId + "&sortOrder=" + sortOrder,
        contentType: "application/json",
        dataType: "json",
        data: null,
        success: function (response) {
            console.log(response.Message)
        }
    });
}

function ChangeSortOrder2(categoryId, recordId, controlId) {

    var sortOrder = $("#" + controlId).val();
    $.ajax({
        type: 'POST',
        url: $("#changeSortOrderUrl").val() + "?categoryId=" + categoryId + "&recordId=" + recordId + "&sortOrder=" + sortOrder,
        contentType: "application/json",
        dataType: "json",
        data: null,
        success: function (response) {
            console.log(response.Message);
            if (categoryId == "6")
                GetServiceEntites();
            if (categoryId == "7")
                GetServiceTypes();
        }
    });
}

function IsPrintEnabled() {
    var PrintPreview = $("#id-button-borders_PrintPreview").is(":checked") ? 1 : 0;
    console.log(PrintPreview);
    if (PrintPreview == 1)
        $("#dvPrintMessage").show();
    else
        $("#dvPrintMessage").hide();
}

function IsAnonymousEnabled() {
    var Viewers = $("#id-button-borders_IsAnonymous").is(":checked") ? 1 : 0;
    console.log(Viewers);
    if (Viewers == 1) {
        GetServiceViewers();
        $("#dvViewers").show();
    }
    else {
        $("#dvViewersData").empty();
        $("#dvViewers").hide();
        //DeleteAllViewers();
    }
}

function GetServiceViewers() {

    var serviceId = $('#ServiceId').val();

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxGetViewersUrl').val() + "?serviceId=" + serviceId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $("#dvViewersData").empty();
                    $("#dvViewersData").html(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function AddServiceViewers() {

    var serviceId = $('#ServiceId').val();
    var viewerId = NullOrWhiteSpace($('#ViewerId').val());
    if (viewerId == "") {
        $('#ViewerId').focus();
        //Swal.fire({ icon: "info", text: "Please enter valid EmailId" });
        ShowAlert("AddServiceViewers1", "0")
        return false;
    }

    if (!isEmail(viewerId))
    {
        $('#ViewerId').focus();
        Swal.fire({ icon: "info", text: "Please enter valid EmailId" });
        ShowAlert("AddServiceViewers2", "0")
        return false;
    }

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxAddViewersUrl').val() + "?serviceId=" + serviceId + "&viewerId=" + viewerId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.Result == true) {
                    $('#ViewerId').val("");
                    GetServiceViewers();
                }
                else {
                    Swal.fire({ icon: "info", text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function DeleteViewer(did, didName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert('ConfirmDeleteViewer', '1') + "<br/> " + didName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#ajaxDeleteViewersUrl").val() + "?did=" + did,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceViewers();
                }
            });
        }
    })
}

function DeleteAllViewers() {

    var serviceId = $('#ServiceId').val();

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxDeleteAllViewersUrl').val() + "?serviceId=" + serviceId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                
            },
            error: function (jq, status, message) { console.log(status); },
        });
}


function GetTemplates() {

    var serviceId = $('#SubServiceId').val();
    var categoryId = $('#CategoryId').val();
    if (categoryId == "" || categoryId == null || categoryId == undefined)
        categoryId = "0";

    var InputId = $("#InputId").val();
    //var InputId = $("#InputId").val();
    if (InputId == "" || InputId == null || InputId == undefined)
        InputId = "";

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId + "&inputId=" + InputId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $("#dvTemplatesData").empty();
                    $("#dvTemplatesData").html(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function SaveServiceTemplate() {

    var SubServiceId = $("#SubServiceId").val();
    if (SubServiceId == "") {
        $("#SubServiceId").focus();
        Swal.fire({ icon: "info", text: "Please Select Service" });
        ShowAlert("SaveServiceTemplate1", "0")
        return false;
    }
    var CategoryId = "0";
    if ($("#CategoryId").is(":visible")) {
        CategoryId = $("#CategoryId").val();
        if (CategoryId == "0" || CategoryId == "" || CategoryId == null || CategoryId == undefined) {
            $("#CategoryId").focus();
            //Swal.fire({ icon: "info", text: "Please Select CLASSIFICATION " });
            ShowAlert("SaveServiceTemplate2", "0")
            return false;
        }
    }

    var InputId = $("#InputId").val();
    if (InputId == "" || InputId == "0") {
        $("#InputId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Attachment for Template" });
        ShowAlert("SaveServiceTemplate3", "0")
        return false;
    }

    var Name = $("#InputId option:selected").text();
    $("#Name").val(Name);

    //var Name = $("#Name").val();
    //if (Name == "") {
    //    $("#Name").focus();
    //    Swal.fire({ icon: "info", text: "Please Enter Template Name" });
    //    return false;
    //}

    var template = $('input[name="fileInput"]').get(0).files[0];
    if (template == null || template == undefined) {
        //Swal.fire({ icon: "info", text: "Please Select English Template" });
        ShowAlert("SaveServiceTemplate4", "0")
        return false;
    }

    var ext = $('#fileInput').val().replace(/^.*\./, '');
    var arrayExtensions = ["docx"];
    if (arrayExtensions.lastIndexOf(ext) == -1) {
        //Swal.fire({ icon: "info", text: "Only " + arrayExtensions.join(', ') + " format is allowed" });
        ShowAlert("SaveServiceTemplate5", "0")
        return false;
    }

    var templateAlt = $('input[name="fileInputAlt"]').get(0).files[0];
    if (templateAlt == null || templateAlt == undefined) {
        //Swal.fire({ icon: "info", text: "Please Select Arabic Template" });
        ShowAlert("SaveServiceTemplate6", "0")
        return false;
    }

    ext = $('#fileInputAlt').val().replace(/^.*\./, '');
    var arrayExtensions = ["docx"];
    if (arrayExtensions.lastIndexOf(ext) == -1) {
        //Swal.fire({ icon: "info", text: "Only " + arrayExtensions.join(', ') + " format is allowed" });
        ShowAlert("SaveServiceTemplate5", "0")
        return false;
    }

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveServiceTemplate", "1") + "<br/> " + Name,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            template = $('input[name="fileInput"]').get(0).files[0];
            templateAlt = $('input[name="fileInputAlt"]').get(0).files[0];

            var formData = new FormData();
            formData.append('File', template);
            formData.append('FileAlt', templateAlt);
            formData.append('ServiceId', SubServiceId);
            formData.append('CategoryId', CategoryId);
            formData.append('Name', Name);
            formData.append('InputControlId', InputId);
            //formData.append('InputControlId', $("#InputControlId").val());

            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetTemplates();
                }
            });
        }
    })
    
}

function DeleteTemplate(templateId, Name, FileUrl) {


    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteServiceTemplate", "1") + " <br/>" + Name,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?templateId=" + templateId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetTemplates();
                }
            });
        }
    })
}

function DownloadTemplate(fileUrl) {
    window.open(fileUrl, "_self");
}

function GetServiceClassificationsListTemplates(serviceId) {

    var subServiceId = $("#SubServiceId").val();
    if (subServiceId != null && subServiceId != "") {

        $("#CategoryId").empty();

        var select2 = $("#CategoryId");

        var url = $("#loadClassificationUrl").val();

        $.getJSON(url, { serviceId: subServiceId },
            function (data) {
                if (data == null)
                    return;

                select2.append($('<option/>', {
                    value: 0,
                    text: "Select Classification"
                }));
                $.each(data, function (index, itemData) {
                    select2.append($('<option/>', {
                        value: itemData.Value,
                        text: itemData.Text,
                    }));
                });
            });

        $("#CategoryId").trigger('change');

    }
}

function GetInputs() {
    var subServiceId = $("#SubServiceId").val();

    var categoryId = $("#CategoryId").val();
    if (categoryId == null || categoryId == "" || categoryId == undefined)
        categoryId = "0";

    if (subServiceId != null && subServiceId != "") {

        $("#InputId").empty();

        var select2 = $("#InputId");

        select2.empty();

        var url = $("#loadInputsUrl").val();

        $.getJSON(url, { serviceId: subServiceId, categoryId : categoryId },
            function (data) {
                if (data == null)
                    return;

                select2.append($('<option/>', {
                    value: 0,
                    text: "-Select-"
                }));
                $.each(data, function (index, itemData) {
                    select2.append($('<option/>', {
                        value: itemData.Value,
                        text: itemData.Text,
                    }));
                });
            });

        $("#InputId").trigger('change');
    }
}

function GetToolTips() {

    var serviceId = $('#SubServiceId').val();
    var categoryId = $('#CategoryId').val();
    if (categoryId == "" || categoryId == null || categoryId == undefined)
        categoryId = "0";

    var InputId = $("#InputId").val();
    //var InputId = $("#InputId").val();
    if (InputId == "" || InputId == null || InputId == undefined)
        InputId = "";

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId + "&categoryId=" + categoryId + "&inputId=" + InputId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $("#dvTemplatesData").empty();
                    $("#dvTemplatesData").html(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function GetInputFields() {
    var subServiceId = $("#SubServiceId").val();

    var categoryId = $("#CategoryId").val();
    if (categoryId == null || categoryId == "" || categoryId == undefined)
        categoryId = "0";

    if (subServiceId != null && subServiceId != "") {

        $("#InputId").empty();

        var select2 = $("#InputId");

        var url = $("#loadInputsUrl").val();

        $.getJSON(url, { serviceId: subServiceId, categoryId: categoryId },
            function (data) {
                if (data == null)
                    return;

                select2.append($('<option/>', {
                    value: 0,
                    text: "-Select-"
                }));
                $.each(data, function (index, itemData) {
                    select2.append($('<option/>', {
                        value: itemData.Value,
                        text: itemData.Text,
                    }));
                });
            });

        $("#InputId").trigger('change');
    }
}

function SaveServiceTooltip() {

    var SubServiceId = $("#SubServiceId").val();
    if (SubServiceId == "") {
        $("#SubServiceId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Service" });
        ShowAlert("SaveServiceTooltip1", "0")
        return false;
    }
    var CategoryId = "0";
    if ($("#CategoryId").is(":visible")) {
        CategoryId = $("#CategoryId").val();
        if (CategoryId == "0" || CategoryId == "" || CategoryId == null || CategoryId == undefined) {
            $("#CategoryId").focus();
            //Swal.fire({ icon: "info", text: "Please Select CLASSIFICATION " });
            ShowAlert("SaveServiceTooltip2", "0")
            return false;
        }
    }

    var InputId = $("#InputId").val();
    if (InputId == "" || InputId == "0") {
        $("#InputId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Field" });
        ShowAlert("SaveServiceTooltip3", "0")
        return false;
    }


    var Name = NullOrWhiteSpace($("#DescriptionEng").val());
    if (Name == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Tooltip English Description" });
        ShowAlert("SaveServiceTooltip4", "0")
        return false;
    }

    var NameAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (NameAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Tooltip Arabic Description" });
        ShowAlert("SaveServiceTooltip5", "0")
        return false;
    }

    var enableGuide = false;
    var showGuide = $("#id-button-borders_Guide").is(":checked") ? 1 : 0;
    if (showGuide == 1) {
        enableGuide = true;
    }

    var enableCard = false;
    var showCard = $("#id-button-borders_Card").is(":checked") ? 1 : 0;
    if (showCard == 1) {
        enableCard = true;
    }


    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfmrimSaveServiceTooltip", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var formData = new FormData();
            formData.append('DescriptionAlt', NameAlt);
            formData.append('DescriptionEng', Name);
            formData.append('EnableGuide', enableGuide);
            formData.append('EnableCard', enableCard);
            formData.append('InputControlId', InputId);

            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    $("#DescriptionEng").val("");
                    $("#DescriptionAlt").val("");
                    $("#id-button-borders_Guide").prop("checked", false);
                    $("#id-button-borders_Card").prop("checked", false);
                    GetToolTips();
                }
            });
        }
    })

}

function DeleteTooltip(templateId, Name) {


    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfmrimDeleteServiceTooltip", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?tooltipId=" + templateId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetToolTips();
                }
            });
        }
    })
}

function GetServiceVideos() {

    var serviceId = $('#SubServiceId').val();
    if (serviceId == "" || serviceId == null || serviceId == undefined)
        serviceId = "0";

    $.ajax(
        {
            type: 'POST',
            url: $('#ajaxUrl').val() + "?serviceId=" + serviceId,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                if (response.success == true) {
                    $("#dvTemplatesData").empty();
                    $("#dvTemplatesData").html(response.result);
                }
                else {
                    ShowMessage("Error", response.result);
                }
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function SaveServiceVideo() {

    var SubServiceId = $("#SubServiceId").val();
    if (SubServiceId == "") {
        $("#SubServiceId").focus();
        //Swal.fire({ icon: "info", text: "Please Select Service" });
        ShowAlert("SaveServiceVideo1", "0")
        return false;
    }

    var Name = NullOrWhiteSpace($("#Name").val());
    if (Name == "") {
        $("#Name").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Video Name English" });
        ShowAlert("SaveServiceVideo2", "0")
        return false;
    }

    var NameAlt = NullOrWhiteSpace($("#NameAlt").val());
    if (NameAlt == "") {
        $("#NameAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Video Name Arabic" });
        ShowAlert("SaveServiceVideo3", "0")
        return false;
    }

    var VideoUrl = NullOrWhiteSpace($("#VideoUrl").val());
    if (VideoUrl == "") {
        $("#VideoUrl").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Service Video Url English" });
        ShowAlert("SaveServiceVideo4", "0")
        return false;
    }
    if (!isValidURL(VideoUrl))
    {
        $("#VideoUrl").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Valid Video Url English" });
        ShowAlert("SaveServiceVideo5", "0")
        return false;
    }

    var VideoAltUrl = NullOrWhiteSpace($("#VideoAltUrl").val());
    if (VideoAltUrl == "") {
        $("#VideoAltUrl").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Service Video Url Arabic" });
        ShowAlert("SaveServiceVideo6", "0")
        return false;
    }
    if (!isValidURL(VideoAltUrl)) {
        $("#VideoUrl").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Valid Video Url Arabic" });
        ShowAlert("SaveServiceVideo7", "0")
        return false;
    }


    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmSaveServiceVideo", "1"),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var formData = new FormData();
            formData.append('Name', Name);
            formData.append('NameAlt', NameAlt);
            formData.append('VideoUrl', VideoUrl);
            formData.append('VideoAltUrl', VideoAltUrl);
            formData.append('ServiceId', SubServiceId);

            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    $("#Name").val("");
                    $("#NameAlt").val("");
                    $("#VideoUrl").val("");
                    $("#VideoAltUrl").val("");
                    GetServiceVideos();
                }
            });
        }
    })

}

function DeleteVideo(templateId, Name) {


    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteServiceVideo", "1") +"<br/> " + Name+"  ?",
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?videoId=" + templateId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceVideos();
                }
            });
        }
    })
}

function isValidURL(str) {
    var a = document.createElement('a');
    a.href = str;
    return (a.host && a.host != window.location.host);
}

function ExportService(TypeName) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmExportService", "1") +" <br/>" + TypeName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $("#aExport")[0].click();
        }
    })

}

function ImportService() {

    var template = $('input[name="fileInput"]').get(0).files[0];
    if (template == null || template == undefined) {
        //Swal.fire({ icon: "info", text: "Please Select Service Json File to Import." });
        ShowAlert("ImportService1", "0")
        return false;
    }

    var ext = $('#fileInput').val().replace(/^.*\./, '');

    var arrayExtensions = ["json"];
    if (arrayExtensions.lastIndexOf(ext) == -1) {
        //Swal.fire({ icon: "info", text: "Only " + arrayExtensions.join(', ') + " format is allowed" });
        ShowAlert("ImportService2", "0")
        return false;
    }

    var filename = $('input[type=file]').val().replace(/.*(\/|\\)/, '');

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmImportService", "1") +"<br/> " + filename,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            template = $('input[name="fileInput"]').get(0).files[0];
            var formData = new FormData();
            formData.append('File', template);

            $("#imgLoading").show();
            $("#btnImport").hide();
            $("#btnImportClose").hide();

            $.ajax({
                type: 'POST',
                url: $("#importUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (response) {
                    $("#imgLoading").hide();
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  });
                    if (response.Result) {
                        $("#btnImportClose").click();
                        GetServices();
                    }
                    else {
                        $("#btnImport").show();
                        $("#btnImportClose").show();
                    }
                }
            });
        }
    })

}