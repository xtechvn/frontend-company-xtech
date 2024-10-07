var _LoginOrRegister =
{
    ClientDashBoard: function () {
        if ($("#client-btn").hasClass('active')) {
            $("#client-btn").removeClass('active')
            $("#client-dashboard").hide();
        }
        else
        {
            $("#client-btn").addClass('active')
            $("#client-dashboard").show();
        }
    },
    ShowForm: function ()
    {
        if ($(".box-login").hasClass('Hide-form')) {
            $(".box-login").removeClass('Hide-form');
            $(".box-register").addClass('Hide-form');
            $(".title-page").text("Đăng nhập");
            $("#title-page").text("Đăng nhập");
        }
        else
        {
            $(".box-register").removeClass('Hide-form');
            $(".box-login").addClass('Hide-form');
            $(".title-page").text("Đăng ký");
            $("#title-page").text("Đăng ký");
        }
    }
}

var _register =
{
    Register: function ()
    {
        var model =
        {
            ClientName : $("#fullname_create").val(),
            Email : $("#email_create").val(),
            Phone : $("#phone_create").val(),
            Password : $("#password_create").val(),
            ConfirmPassword : $("#confirmPW_create").val()
        }
       
        if (model.ClientName == undefined || model.ClientName.trim() == '') {
            $("#username-create-span").text("Tên người dùng không được để trống")
            $("#username-create-span").show();
            return;
        }
        else
        {
            $("#username-create-span").hide();
        }


        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (model.Email == undefined || model.Email.trim() == '') {
            $("#email-create-span").text("Email không được để trống*")
            $("#email-create-span").show();
            return;
        }
        else if (!emailRegex.test(model.Email)) {
            $("#email-create-span").text("Email không đúng định dạng*")
            $("#email-create-span").show();
            return;
        }
        else {
            $("#email-create-span").css("display", "none");
        } 


        const PhoneNumberRegex = /^\d{10,11}$/;
        if (model.Phone == undefined || model.Phone.trim() == '') {
            $("#Phone-create-span").text("Số điện thoại không được để trống*")
            $("#Phone-create-span").show();
            return;
        }
        else if (!PhoneNumberRegex.test(model.Phone)) {
            $("#Phone-create-span").text("Số điện thoại không đúng định dạng*")
            $("#Phone-create-span").show();
            return;
        }
        else {
            $("#Phone-create-span").css("display", "none");
        }

        const PasswordRegex = /\s+/g;
        if (model.Password == undefined || model.Password.trim() == '') {
            $("#password-create-span").text("Mật khẩu không được để trống")
            $("#password-create-span").show();
            return;
        }
        else if (PasswordRegex.test(model.Password))
        {
            $("#password-create-span").text("Mật khẩu không được chứa dấu cách")
            $("#password-create-span").show();
            return;
        }
        else if (model.Password.length < 8)
        {
            $("#password-create-span").text("Mật khẩu không được ít hơn 8 kí tự")
            $("#password-create-span").show();
        }
        else {
            $("#password-create-span").hide();
        }


        if (model.ConfirmPassword == undefined || model.ConfirmPassword.trim() == '') {
            $("#confirmPW-create-span").text("Mật khẩu xác nhận không được để trống")
            $("#confirmPW-create-span").show();
            return;
        }
        else if (PasswordRegex.test(model.ConfirmPassword)) {
            $("#confirmPW-create-span").text("Mật khẩu không được chứa dấu cách")
            $("#confirmPW-create-span").show();
            return;
        }
        else if (model.ConfirmPassword.length < 8) {
            $("#confirmPW-create-span").text("Mật khẩu xác nhận không được ít hơn 8 kí tự")
            $("#confirmPW-create-span").show();
        }
        else {
            $("#confirmPW-create-span").hide();
        }

        $.ajax({
            url: "LoginOrRegister/Register",
            type: "post",
            data: { model: model },
            success: function (result) {
                _login.LoginSucess = true
                $('.btn-login').removeAttr('disabled');
                if (result != undefined && result.status == 0) {
                    _msgalert.success("Đăng ký thành công", "thông báo");
                    sessionStorage.setItem("idClient", result.data.idClient);
                    sessionStorage.setItem("idAccount", result.data.idAccount);
                    sessionStorage.setItem("email", result.data.email);
                    sessionStorage.setItem("userName", result.data.userName);
                    setTimeout(function () {
                        window.location.href = result.data.returnUrl;
                    }, 1500);
                }
                else
                {
                    _msgalert.error(result.msg, "thông báo");
                }

            }
        });

    }
}

var _logout =
{
    logout: function ()
    {
        sessionStorage.setItem("idClient", '');
        sessionStorage.setItem("idAccount", '');
        sessionStorage.setItem("email", '');
        sessionStorage.setItem("userName", '');
        $.ajax({
            url: "LoginOrRegister/SignOut",
            type: "post",
            success: function (result) {
                window.location.href = "/";
            }
        });
    }
}

var _login = {
    Token: '',
    DotText: '',
    OnLoggingText: '<i class="fa fa-sign-in"></i>Đang đăng nhập',
    LoginSucess: false,

    Login: function () {
        $('.btn-login').attr('disabled', true);
        var model = {
            UserName: $('#user-name').val(),
            Password: $('#password').val(),
            RememberMe: false,
            ReturnUrl: $('#form-login').attr('data-url')
        }
        if ($('#remember-me').is(":checked")) {
            model.RememberMe = true
        }
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (model.UserName == undefined || model.UserName.trim() == '') {
            $("#user-name-span").text("Tài khoản không được để trống*")
            $("#user-name-span").show();
        }
        else if (!emailRegex.test(model.UserName))
        {
            $("#user-name-span").text("Email không đúng định dạng*")
            $("#user-name-span").show();
        }
        else
        {
            $("#user-name-span").css("display", "none");
        }

        if (model.Password == undefined || model.Password.trim() == '') {
            $("#password-span").text("Mật khẩu không được để trống*")
            $("#password-span").show();
        }
        else
        {
            $("#password-span").css("display","none");
        }

        $.ajax({
            url: "LoginOrRegister/ConfirmLogin",
            type: "post",
            data: { model: model },
            success: function (result) {
                _login.LoginSucess = true
                $('.btn-login').removeAttr('disabled');
                if (result != undefined && result.status == 0) {
                    _msgalert.success("Đăng nhập thành công", "thông báo");
                    sessionStorage.setItem("idClient", result.idClient);
                    sessionStorage.setItem("idAccount", result.idAccount);
                    sessionStorage.setItem("email", result.email);
                    sessionStorage.setItem("userName", result.userName);
                    setTimeout(function () {
                        window.location.href = result.returnUrl;
                    }, 1500);
                }
                else {
                    _msgalert.error("Đăng nhập thất bại","thông báo");
                    $('.img_loading_summit').remove()

                }

            }
        });
    },
}