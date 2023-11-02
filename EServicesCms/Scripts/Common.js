function noBack() {
    window.history.forward();
}

function ShowMessage(messageTitle, messageTxt) {
    $.gritter.add({
        title: messageTitle,
        text: messageTxt,
        sticky: false,
        class_name: 'gritter-warning'
    });
    setTimeout("$.gritter.removeAll();", 5000);
    return false;
}

function checkInputs() {
    var isValid = true;
    $('input').filter('[required]').each(function () {
        if ($(this).val() === '') {
            $(this).focus();
            isValid = false;
            return false;
        }
    });
    
    return isValid;
}

function CheckBackSpace(event) {
    if (event.keyCode == 8 || event.keyCode == 46) {
        //$("#ReferenceNo").val("");
        //$("#RefNo").val("");
        //$("#ResultCode").val("");
        //$("#ResultCodeDescription").val("");
        //$("#ResultDescription").val("");
        //$("#PaymentMethod").val("");
        //$("#PaymentMethodCode").val("");
        //$("#ReceiptNo").val("");
        //$("#ChargeFee").val("");
        //$("#FinancingFee").val("");
        //$("#Amount").val("");
        //$("#PaymentDate").val("");
    }
    return true;
}

function isNullOrWhiteSpaceOrEmpty(str) {
    var returnValue = false;
    if (!str
        || str == null
        || str == ""
        || str === 'null'
        || str === ''
        || str === '{}'
        || str === 'undefined'
        || str.length === 0) {
        returnValue = true;
    }
    return returnValue;
}

function NullOrWhiteSpace(str) {
    var returnValue = false;
    //if (str.indexOf(' ') >= 0)
    //{
    //    str = "";
    //}
    if (str.trim() === '') {
        str = "";
    }
    return str;
}

function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

function fnAllowChar(jStrControlID) {
    if (document.getElementById(jStrControlID).value != "") {
        if (isNaN(document.getElementById(jStrControlID).value) == false)
            document.getElementById(jStrControlID).value = "";
    }
}

function fnValidateTextArea(szId, szLen) {
    var nLen = parseInt(szLen);
    var strValue = document.all.item(szId).value;
    if (strValue.length > nLen)
        document.all.item(szId).value = strValue.substr(0, nLen);
}

function fnMasking(strContrlId) {

    if (document.all.item(strContrlId) == null)
        return;
    var szBuffer = document.all.item(strContrlId).value;
    if (szBuffer == "")
        return;

    var szResString = "";
    var nLen = szBuffer.length;
    var szCharTyped = szBuffer.substr(nLen - 1, 1);
    for (nCnt = 0; nCnt < nLen; nCnt++) {
        szCharTyped = szBuffer.substr(nCnt, 1);

        if (szCharTyped == "'")
            szCharTyped = "";

        if (szCharTyped == '"')
            szCharTyped = "";

        if (szCharTyped == "~")
            szCharTyped = "";

        if (szCharTyped == "|")
            szCharTyped = "";

        if (szCharTyped == "^")
            szCharTyped = "";

        if (szCharTyped == "`")
            szCharTyped = "";

        if (szCharTyped == "!")
            szCharTyped = "";

        if (szCharTyped == "#")
            szCharTyped = "";

        if (szCharTyped == "$")
            szCharTyped = "";

        if (szCharTyped == "%")
            szCharTyped = "";

        if (szCharTyped == "&")
            szCharTyped = "";

        if (szCharTyped == "*")
            szCharTyped = "";

        if (szCharTyped == ":")
            szCharTyped = "";

        if (szCharTyped == ";")
            szCharTyped = "";

        if (szCharTyped == "@")
            szCharTyped = "";

        if (szResString == "")
            szResString = szCharTyped;
        else
            szResString = szResString + szCharTyped;
    }
    document.all.item(strContrlId).value = szResString;
}

function fnValidateNumeric(strCtrlId) {
    if (document.all.item(strCtrlId) == false)
        return;
    var szBuffer = document.all.item(strCtrlId).value;
    if (szBuffer == "")
        return;
    var szResString = "";
    var nLen = szBuffer.length;
    var szCharTyped = szBuffer.substr(nLen - 1, 1);
    for (nCnt = 0; nCnt < nLen; nCnt++) {
        szCharTyped = szBuffer.substr(nCnt, 1);
        if (!isNaN(szCharTyped) || szCharTyped == " ") {
            if (szCharTyped == " ")
                szCharTyped = "";
            if (szResString == "")
                szResString = szCharTyped;
            else
                szResString = szResString + szCharTyped;
        }
        else
            continue;
    }
    document.all.item(strCtrlId).value = szResString;

}

function CloseCpForm() {
    $("#NewPassword").val("");
    $("#ReconfirmPassword").val("");
    modalCp.style.display = "none";
}

function ClearCpInput() {
    $("#NewPassword").val("");
    $("#ReconfirmPassword").val("");
}

function ChangePassword() {

    if ($("#NewPassword").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Enter your New Password" });
        ShowAlert("ChangePassword1","0")
        $("#NewPassword").focus();
        return false;
    }
    else {
        var pass = $("#NewPassword").val();
        if (pass.length < 8) {
            //Swal.fire({ icon: "info", text: 'You have entered less than 8 characters for password' });
            ShowAlert("ChangePassword2", "0")
            $("#Password").focus();
            return false;
        }
    }

    if ($("#ReconfirmPassword").val() == "") {
        //Swal.fire({ icon: "info", text: "Please Reconfirm your New Password" });
        ShowAlert("ChangePassword3", "0")
        $("#ReconfirmPassword").focus();
        return false;
    }
    else {
        var pass = $("#ReconfirmPassword").val();
        if (pass.length < 8) {
            //Swal.fire({ icon: "info", text: 'You have entered less than 8 characters for password' });
            ShowAlert("ChangePassword4", "0")
            $("#Password").focus();
            return false;
        }
    }

    if ($("#NewPassword").val() != $("#ReconfirmPassword").val()) {
        //Swal.fire({ icon: "info", text: 'Password mismatch. Please reconfirm the password.' });
        ShowAlert("ChangePassword5","0")
        $("#Password").focus();
        return false;
    }

    Swal.fire({
        icon: 'question',
        text: ShowAlert('ChangePasswordConfirm','1'),// 'Do you want to Change the password of your account ?',
        //showCloseButton: true,
        showCancelButton: true,
        //focusConfirm: true,
        confirmButtonText: ShowAlert('YesButton','1'),
        cancelButtonText: ShowAlert('NoButton', '1'),
    }).then((result) => {
        if (result.isConfirmed) {
            var iObject = {};
            iObject.NewPassword = $("#NewPassword").val();
            iObject.ReconfirmPassword = $("#ReconfirmPassword").val();
            console.log(JSON.stringify(iObject));
            $.ajax({
                type: 'POST',
                url: $("#changePasswordUrl").val(),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(iObject),
                success: function (response) {

                    Swal.fire({
                        icon: response.Icon,
                        text: response.Message,
                        confirmButtonText: ShowAlert('OkButton','1'),
                    }).then((result) => {
                        if (result.isConfirmed) {
                            if (response.Result == true) {
                                document.getElementById('btnLogout').click();
                            }
                        }
                    })
                }
            });
        }
    })
}

function NoPermission(a = null, b = null, c = null, d = null, e = null) {
    //Swal.fire("You do not have permission to perform this action");
    ShowAlert("NoPermission","0");
}

function stripHtmlToText(controlId) {
    var html = $("#" + controlId).val();

    if (html != "") {
        var SCRIPT_REGEX = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
        while (SCRIPT_REGEX.test(html)) {
            html = html.replace(SCRIPT_REGEX, "");
        }

        var STYLE_REGEX = /<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>/gi;
        while (STYLE_REGEX.test(html)) {
            html = html.replace(SCRIPT_REGEX, "");
        }

        var regex = /(<([^>]+)>)/ig;
        var result = html.replace(regex, "");
        $("#" + controlId).val(result);
    }
}