﻿$("body").on("click", ".btn_sendemail", function () {
    let FromCreate = $('#form_send_EmailCT');
    FromCreate.validate({
        rules: {
            "Name": "required",
            "Email": "required",
            "Title": "required",
            "Note": "required",
            "checkboxemail": "required",
        },
        messages: {
            "Name": "Họ tên không được bỏ trống",
            "Email": "Email không được bỏ trống",
            "Title": "Tiêu đề không được bỏ trống",
            "Note": "Nội dung không được bỏ trống",
            "checkboxemail": "Chưa chọn chấp nhận điều khoản và chính sách",
        }
    });
    if (FromCreate.valid()) {
        var model = {
            Email: $('#Email').val(),
            Name: $('#Name').val(),
            Title: $('#Title').val(),
            Note: $('#Note').val(),
        };
        $.ajax({
            url: "/Contact/SendEmail",
            type: "Post",
            data: { model },
            success: function (result) {
                if (result.status == 0) {
                    _msgalert.success(result.smg);

                } else {
                    _msgalert.error(result.smg);
                }
            }
        });
    }
   
});