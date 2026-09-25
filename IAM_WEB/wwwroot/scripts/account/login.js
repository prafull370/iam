
$(document).ready(function () {
    $('#txtotp').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });
    $('#visibilityId').on("click", function () {
        var input = $('#pswd');
        var type = input.attr('type') === 'password' ? 'text' : 'password';
        input.attr('type', type);
    });

    $('.toggle-password').on("click", function () {
        var targetId = $(this).data("target");
        var input = $('#' + targetId);
        var type = input.attr('type') === 'password' ? 'text' : 'password';
        input.attr('type', type);
    });
    $("#btnSubmit").removeClass('d-none');
    $("#loginBtn").addClass('d-none');
    $("#divPassword").addClass('d-none');
    $("#divOTP").addClass('d-none');
    $("#txtLoginType").val('0');
    $("#txtotp").val('');
    $("#pswd").val('');
    $("#divForgotPassword").addClass('d-none');
});


// Submit Login Form
function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }

    var formData = new FormData(form[0]); // Serialize form data
    const returnUrl = new URLSearchParams(window.location.search).get("returnUrl");
    $.ajax({
        url: "/Account/Login" + (returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : ""), // Use updated action URL
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {
            $("#dvProcessing").hide();
            var url = "";

            if (response.Status) {
                $('#loginBtn').prop("disabled", true);

                if (response.RedirectUrl != '' && response.RedirectUrl !== null) {
                    url = response.RedirectUrl;
                }
                else if (response.Redirect) {
                    url = "/Account/ChangePassword";
                }
                else if (response.Is_SuperAdmin) {
                    url = "/IAM/Company/List";
                }
                else {
                    url = "/Home/Index";
                }

                // Optionally reload after 2 seconds
                setTimeout(() => {
                    // Add Route Accordingly
                    window.location.href = url;
                }, 100);
            }
            else if (!response.Status) {
                //showAlert(response.Message, "Error", "OkCancel");
                if (response.Message.includes("already logged")) {

                    Swal.fire({
                        title: "User Already Logged In",
                        text: "Do you want to continue?",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Yes",
                        cancelButtonText: "No"
                    }).then((result) => {

                        // Hide login button after user decision
                        $("#loginBtn").addClass('d-none');

                        if (result.isConfirmed) {

                            var formData = new FormData($("#" + formId)[0]);
                            formData.append("ForceLogin", true);

                            $.ajax({
                                url: "/Account/Login",
                                type: "POST",
                                data: formData,
                                contentType: false,
                                processData: false,
                                success: function (response) {

                                    if (response.Status) {

                                        let url = "/Home/Index";

                                        if (response.RedirectUrl) {
                                            url = response.RedirectUrl;
                                        }
                                        else if (response.Redirect) {
                                            url = "/Account/ChangePassword";
                                        }
                                        else if (response.Is_SuperAdmin) {
                                            url = "/IAM/Company/List";
                                        }

                                        window.location.href = url;
                                    }
                                }
                            });

                        }

                    });

                }
                else {
                    showAlert(response.Message, "Error", "OkCancel");
                }
            }
            else {
                showAlert(response.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.statusText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}
function SendOTP() {
    $("#txtResendOTP").addClass('d-none');
    let LoginId = $("#username").val();
    $.ajax({
        url: "/Account/SendOTP",
        "type": "GET",
        "datatype": "json",
        data: { "Id": LoginId },
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {
            $("#dvProcessing").hide();
            if (response.Status) {
                showAlert(response.Message, "Success", "OkCancel");
                setTimeout(function () {
                    $("#txtResendOTP").removeClass('d-none');
                }, 30000);
            }
            else {
                showAlert(response.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.statusText);
            showAlert(errorResponse.ResponseData, "Error", "OkCancel");
        }
    });
}
// Change Password Form
function ChangePassForm(formId) {
    event.preventDefault(); // Prevent default form submission

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }

    var formData = new FormData(form[0]); // Serialize form data

    $.ajax({
        url: "/Account/ChangePassword",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {
            $("#dvProcessing").hide();

            if (response.Status) {
                $('#updateBtn').prop("disabled", true);

                setTimeout(() => {
                    window.location.href = "/Account/Login";
                }, 10);
            }
            else if (!response.Status) {
                showAlert(response.Message, "Warning", "OkCancel");
            }
            else {
                showAlert(response.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.statusText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}

function GetLoginTypeInfo() {

    let LoginId = $("#username").val();
    $.ajax({
        url: "/Account/Get_Login_Type",
        "type": "GET",
        "datatype": "json",
        data: { "Id": LoginId },
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {
            $("#dvProcessing").hide();
            if (response.Status) {
                $("#loginBtn").removeClass('d-none');
                $("#btnSubmit").addClass('d-none');
                $("#username").attr('readonly', true);
                if (response.ResponseData.Login_Type_Id === 1) {
                    $("#divPassword").removeClass('d-none');
                    $("#divOTP").addClass('d-none');
                    $("#txtLoginType").val('1');
                    $("#txtotp").val('000000');
                    $("#pswd").val('');
                    $("#txtResendOTP").addClass('d-none');
                    $("#divForgotPassword").removeClass('d-none');
                }
                else if (response.ResponseData.Login_Type_Id === 2) {
                    $("#divPassword").addClass('d-none');
                    $("#divOTP").removeClass('d-none');
                    $("#txtLoginType").val('2');
                    $("#txtotp").val('');
                    $("#pswd").val('Test@1234');
                    $("#divForgotPassword").addClass('d-none');
                    SendOTP();
                    setTimeout(function () {
                        $("#txtResendOTP").removeClass('d-none');
                    }, 30000);
                }
                else if (response.ResponseData.Login_Type_Id === 3) {
                    $("#divPassword").addClass('d-none');
                    $("#divOTP").removeClass('d-none');
                    $("#txtLoginType").val('3');
                    $("#txtotp").val('');
                    $("#pswd").val('Test@1234');
                    $("#divForgotPassword").addClass('d-none');
                    SendOTP();
                    setTimeout(function () {
                        $("#txtResendOTP").removeClass('d-none');
                    }, 30000);
                }
            }
            else {
                $("#loginBtn").addClass('d-none');
                $("#btnSubmit").removeClass('d-none');
                showAlert(response.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.statusText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}