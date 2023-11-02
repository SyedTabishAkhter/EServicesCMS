
function SaveContent() {

    if ($("#TitleEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Title English" });
        ShowAlert("SaveContent1", "0");
        $("#TitleEng").focus();
        return false;
    }

    if ($("#TitleAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Title Arabic" });
        ShowAlert("SaveContent2", "0");
        $("#TitleAlt").focus();
        return false;
    }

    if ($("#DescriptionEng").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Description English" });
        ShowAlert("SaveContent3", "0");
        $("#DescriptionEng").focus();
        return false;
    }

    if ($("#DescriptionAlt").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter Description Arabic" });
        ShowAlert("SaveContent4", "0");
        $("#DescriptionAlt").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: ShowAlert('ConfirmSaveContent', '1'),
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton', '1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.TitleEng = $("#TitleEng").val();
            iObject.TitleAlt = $("#TitleAlt").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.UrlEng = $("#UrlEng").val();
            iObject.UrlAlt = $("#UrlAlt").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#saveUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {
                    Swal.fire({ icon: response.Icon, text: response.Message, confirmButtonText: ShowAlert('OkButton', '1')  })
                    //if (response.Result == true) {
                    //    window.top.location.reload(true);
                    //}
                }
            });
        }
    })
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
                    error: function (jq, status, message) {
                        Swal.fire({ icon: "error", text: jq.responseText, confirmButtonText: ShowAlert('OkButton', '1')  })
                    },
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