
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
    //iSearch.SearchCri = $("#name_filter").val();
    //iSearch.DepartmentId = $("#sDepartmentId").val();
    //iSearch.RoleId = $("#sRoleId").val();
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

    $('#dynamic-table').show();
}

function GetServiceGuideProcedures() {

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
    $("#UrlEng").val("");
    $("#UrlAlt").val("");
    $("#SortOrder").val("");
}

function SaveProcedures() {

    var TitleEng = NullOrWhiteSpace($("#TitleEng").val());
    if (TitleEng == "") {
        $("#TitleEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Title" });
        ShowAlert("SaveProcedures1", "0")
        return false;
    }

    var TitleAlt = NullOrWhiteSpace($("#TitleAlt").val());
    if (TitleAlt == "") {
        $("#TitleAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Title" });
        ShowAlert("SaveProcedures2", "0")
        return false;
    }

    var DescriptionEng = NullOrWhiteSpace($("#DescriptionEng").val());
    if (DescriptionEng == "") {
        $("#DescriptionEng").focus();
        //Swal.fire({ icon: "info", text: "Please Enter English Description" });
        ShowAlert("SaveProcedures3", "0")
        return false;
    }

    var DescriptionAlt = NullOrWhiteSpace($("#DescriptionAlt").val());
    if (DescriptionAlt == "") {
        $("#DescriptionAlt").focus();
        //Swal.fire({ icon: "info", text: "Please Enter Arabic Description" });
        ShowAlert("SaveProcedures4", "0")
        return false;
    }

    var myName = TitleEng;
    if (language == "0" || language == 0)
        myName = TitleAlt;

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmSaveProcedures", "1") + "<br/> " + myName,
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
            iObject.UrlEng = $("#UrlEng").val();
            iObject.UrlAlt = $("#UrlAlt").val();
            iObject.DescriptionEng = $("#DescriptionEng").val();
            iObject.DescriptionAlt = $("#DescriptionAlt").val();
            iObject.SortOrder = $("#SortOrder").val();
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
                    GetServiceGuideProcedures();
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
                $("#UrlEng").val(response.UrlEng);
                $("#UrlAlt").val(response.UrlAlt);
                $("#SortOrder").val(response.SortOrder);
                modal.style.display = "block";
            },
            error: function (jq, status, message) { console.log(status); },
        });
}

function Delete(recordId, Title) {

    Swal.fire({
        icon: 'question',
        html: ShowAlert("ConfirmDeleteeProcedures", "1") +"<br/> " + Title,
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
                    GetServiceGuideProcedures();
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
            GetServiceGuideProcedures();
        }
    });
}

function Publish() {


    Swal.fire({
        icon: 'question',
        text: ShowAlert("ConfirmPublish", "1"),
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
        text: ShowAlert("ConfirmPublishCards", "1"),
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