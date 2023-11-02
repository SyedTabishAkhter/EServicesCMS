function GetServiceGuideSupport() {

    var iObject = {};
    iObject.dummy = 0;
    console.log(JSON.stringify(iObject));

    $.ajax(
        {
            type: 'POST',
            url: $('#gridUrl').val(),
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

function clearInput() {
    $("#Code").val("");
    $("#TitleEng").val("");
    $("#TitleAlt").val("");
    $("#DescriptionEng").val("");
    $("#DescriptionAlt").val("");
    $("#TypeId").val("");
    $("#SortOrder").val(""); 
    $("#ClassName").val("");
}

function SavesSupport() {

    var TypeId = NullOrWhiteSpace($("#TypeId").val());
    if (TypeId == "") {
        $("#TypeId").focus();
        //Swal.fire({ icon: "info", text: "Please Support Type" });
        ShowAlert("SavesSupport1", "0")
        return false;
    }

    var TitleEng = NullOrWhiteSpace($("#TitleEng").val());
    if (TitleEng == "") {
        $("#TitleEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Title" });
        ShowAlert("SavesSupport2", "0")
        return false;
    }

    var TitleAlt = NullOrWhiteSpace($("#TitleAlt").val());
    if (TitleAlt == "") {
        $("#TitleAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Title" });
        ShowAlert("SavesSupport3", "0")
        return false;
    }

    var DescriptionEng = NullOrWhiteSpace($("#DescriptionEng").val());
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Description" });
        ShowAlert("SavesSupport4", "0")
        return false;
    }

    var DescriptionAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Description" });
        ShowAlert("SavesSupport5", "0")
        return false;
    }

    var myName = TitleEng;
    if (language == "0" || language == 0)
        myName = TitleAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSavesSupport", "1") + "<br/> " + myName,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {

            var iObject = {};
            iObject.RecordId = $("#Code").val();
            iObject.TitleEng = $("#TitleEng").val();
            iObject.TitleAlt = $("#TitleAlt").val();
            iObject.TypeId = $("#TypeId").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.SortOrder = $("#SortOrder").val();
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
                    GetServiceGuideSupport();
                }
            });
        }
    })
}


function Modify(Code) {


    //var iObject = {};
    //iObject.dummy = 0;
    //console.log(JSON.stringify(iObject));

    $.ajax(
        {
            type: 'GET',
            url: $('#getUrl').val() + "?recordId=" + Code,
            contentType: "application/json",
            dataType: "json",
            data: null,// JSON.stringify(iObject),
            success: function (response) {
                console.log(response);

                clearInput();
                $("#Code").val(response.RecordId);
                $("#TitleEng").val(response.TitleEng);
                $("#TitleAlt").val(response.TitleAlt);
                $("#DescriptionEng").val(response.DescriptionEng);
                $("#DescriptionAlt").val(response.DescriptionAlt);
                $("#TypeId").val(response.TypeId);
                $("#SortOrder").val(response.SortOrder);
                $("#ClassName").val(response.ClassName);
                modal.style.display = "block";
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function Delete(recordId, Title) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteSupport", "1") + "<br/> " + Title,
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: $("#deleteUrl").val() + "?recordId=" + recordId,
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    GetServiceGuideSupport();
                }
            });
        }
    })
}


function ChangeSortOrder(categoryId, recordId, controlId) {

    var sortOrder = $("#" + controlId).val();
    $.ajax({
        type: 'POST',
        url: $("#changeSortOrderUrl").val() + "?categoryId=" + categoryId + "&recordId=" + recordId + "&sortOrder=" + sortOrder,
        contentType: "application/json",
        dataType: "json",
        data: null,
        success: function (response) {
            console.log(response.Message);
            GetServiceGuideSupport();
        }
    });
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

function PublishCards() {


    Swal.fire({
        icon: 'question',
        text: ShowAlert('ConfirmPublishCards', '1'),
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
                    url: $('#publishCardsUrl').val(),
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