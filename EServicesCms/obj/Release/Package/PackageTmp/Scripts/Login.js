function formValidation() {
    var isValid = true;
    if (isNullOrWhiteSpaceOrEmpty($("#UserName").val())) {
        //ShowMessage("Alert", "Please enter username");
        $('#UserName').focus();
        isValid = false;
        return isValid;
    }
    if (isNullOrWhiteSpaceOrEmpty($("#Password").val())) {
        //ShowMessage("Alert", "Please enter password");
        $('#Password').focus();
        isValid = false;
        return isValid;
    }
    return isValid
}
function forgotPasswordValidation() {
    var isValid = true;
    if (IsNullOrEmpty($("#txtEmail").val())) {
        $('#txtEmail').focus();
        isValid = false;
        return isValid;
    }
    if (IsNullOrEmpty($("#ddlQuestion").val())) {
        $('#ddlQuestion').focus();
        isValid = false;
        return isValid;
    }
    if (IsNullOrEmpty($("#txtAnswer").val())) {
        $('#txtAnswer').focus();
        isValid = false;
        return isValid;
    }
    return isValid
}

$("#btnSendMe").click(function () {

    if (!forgotPasswordValidation()) {
        return false;
    }

    var iUser = {};
    iUser.Email = $('#txtEmail').val();
    iUser.SecurityQuestionId = $('#ddlQuestion').val();
    iUser.SecurityAnswer = $('#txtAnswer').val();

    var JSONdata = JSON.stringify(iUser);
    console.log(iUser);
    $.ajax(
        {
            type: 'POST',
            data: JSONdata,
            url: "/Account/AjaxForgotPassword/",
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                ShowMessage(response.title, response.message);
                if (response.success == true) {
                    $("#txtEmail").val("");
                    $("#ddlQuestion").val("");
                    $("#txtAnswer").val("");
                }
            },
            error: function (jq, status, message) { console.log(status); sessionStorage.setItem('updated', 0); },
        });
});

$("#btnResetPassword").click(function () {

    if (!forgotResetPasswordValidation()) {
        return false;
    }

    var iUser = {};
    iUser.Password = $('#txtPassword').val();
    iUser.SecurityQuestion = $('#ddlQuestion').val();
    iUser.SecurityAnswer = $('#txtAnswer').val();

    var JSONdata = JSON.stringify(iUser);
    console.log(iUser);
    $.ajax(
        {
            type: 'POST',
            data: JSONdata,
            url: "/Account/AjaxResetPassword/",
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                ShowMessage(response.title, response.message);
                window.location.href = "/Account/Index";
            },
            error: function (jq, status, message) { console.log(status); sessionStorage.setItem('updated', 0); },
        });
});
function forgotResetPasswordValidation() {
    var isValid = true;
    if (IsNullOrEmpty($("#txtPassword").val())) {
        $('#txtPassword').focus();
        isValid = false;
        return isValid;
    }
    if (IsNullOrEmpty($("#ddlQuestion").val())) {
        $('#ddlQuestion').focus();
        isValid = false;
        return isValid;
    }
    if (IsNullOrEmpty($("#txtAnswer").val())) {
        $('#txtAnswer').focus();
        isValid = false;
        return isValid;
    }
    return isValid
}
function RememberMe() {

    if ($("#RememberMe").is(":checked")) {

        var username = $('#UserName').val();
        var password = $('#Password').val();

        $.cookie('username', username, { expires: 45 });
        $.cookie('password', password, { expires: 45 });
        $.cookie('remember', true, { expires: 45 });
    }
    else {
        $.cookie('username', null);
        $.cookie('password', null);
        $.cookie('remember', false);
    }

    console.log($.cookie('remember'));
    return true;
}

function ContinueToLogin() {
    if ($("#UserName").val() == "") {
        $("#UserName").focus();
        return false;
    }
    if ($("#Password").val() == "") {
        $("#Password").focus();
        return false;
    }

    var RememberMe = false;
    if ($("#RememberMe").is(":checked")) {
        RememberMe = true;
    }

    $("#dvMessage").html(ShowAlert("CheckLoginDetails", "1"));

    var iObject = {};
    iObject.UserName = $("#UserName").val();
    iObject.Password = $("#Password").val();
    iObject.RememberMe = RememberMe;
    //alert(JSON.stringify(iObject));
    $.ajax({
        type: 'POST',
        url: loginUrl,
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(iObject),
        success: function (response) {
            //$("#dvMessage").show();
            $("#dvMessage").html(response.Message);
            $("#dvMessage").fadeTo(2000, 1000).slideUp(1000, function () {
                $("#dvMessage").slideUp(1000);
            });

            if (response.Result == true) {
                SetRememberMe(this);
                window.top.location.href = redirectUrl;
            }
        }
    });
}
function SetRememberMe() {

    if ($("#RememberMe").is(":checked")) {

        var username = $('#UserName').val();
        var password = $('#Password').val();

        $.cookie('eservicesusername', username, { expires: 45 });
        $.cookie('eservicespassword', password, { expires: 45 });
        $.cookie('eservicesremember', true, { expires: 45 });
    }
    else {
        $.cookie('eservicesusername', null);
        $.cookie('eservicespassword', null);
        $.cookie('eservicesremember', false);
    }
    console.log($.cookie('eservicesremember'));
    return true;
}
function ForgotPassword() {

    if ($("#UserName").val() == "") {
        $("#UserName").focus();
        $("#success-alert").html("Please Enter your registered Email Address.");
        $("#success-alert").fadeTo(2000, 1000).slideUp(1000, function () {
            $("#success-alert").slideUp(1000);
        });
        return false;
    }

    var iObject = {};
    iObject.UserName = $("#UserName").val();
    $.ajax({
        type: 'POST',
        url: forgotUrl,
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(iObject),
        success: function (response) {
            $("#UserName").val("");
            $("#success-alert").html(response.Message);
            $("#success-alert").fadeTo(2000, 1000).slideUp(1000, function () {
                $("#success-alert").slideUp(1000);
            });
        }
    });
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

function striptag() {
    var html = /(<([^>]+)>)/gi;
    for (i = 0; i < arguments.length; i++)
        arguments[i].value = arguments[i].value.replace(html, "")
}

function ChangeLanguage() {

    $.ajax({
        type: 'GET',
        url: langUrl,
        contentType: "application/json",
        dataType: "json",
        data: null,
        success: function (response) {
            window.top.location.reload(true);
        }
    });
}