$(document).ready(function () {

    $('.menu-tab-button-trang-chu').addClass('active');
    getRelatednews();

    $("body").on("click", ".send_email", function () {
        SendEmail();

    });
});
function getRelatednews() {
    var requestObj = {
        page: 1,
        size: 3,
        category_id: 1004,
    }
    var rows = "";
    $.ajax({
        url: "/News/GetlistNews",
        type: "Post",
        data: { requestObj },
        success: function (result) {
            if (result.data != null) {
                for (var i in result.data) {
                    var item = result.data[i];
                    /* var date = TIME_UTILS.getDateStringRequest(item.publish_date)*/
                    rows += `<div class="article-itemt full">
                                <div class="article-thumb">
                                    <a class="thumb_img thumb_5x3" href="/tin-tuc/${item.id}">
                                        <img src="${item.image_169}" alt="">
                                    </a>
                                </div>
                                <div class="article-content">
                                    <div class="date">
                                        <svg class="icon-svg">
                                            <use xlink:href="/images/icons/icon.svg#date"></use>
                                        </svg>
                                        <span>${item.publish_date}</span>
                                    </div>

                                    <h3 class="title_new" style=" width: 100%;">
                                        <a href="/tin-tuc/${item.id}">${item.title}</a>
                                    </h3>
                                    <p style=" width: 100%;" class="des">${item.lead}</p>
                                    <div><a class="read-more" href="/tin-tuc/${item.id}">Đọc thêm</a></div>
                                </div>
                            </div>`
                }
                $('#Related_news').html(rows);
            }

            /* $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");*/

        }
    });
}

function SendEmail() {
    var model = {
        Email: $('#Email').val(),
        Name: null,
        Title: null,
        Note: null,
        type: 2,
    };
    $.ajax({
        url: "/Contact/SendEmail",
        type: "Post",
        data: { model},
        success: function (result) {
            if (result.status == 0) {
                _msgalert.success(result.msg);
            } else {
                _msgalert.error(result.msg);
            }
        }
    });
}