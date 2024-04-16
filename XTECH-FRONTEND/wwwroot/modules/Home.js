$(document).ready(function () {

    getRelatednews();

    openview();
    setTimeout(function () {
        openview();
    }, 500);
});
function getRelatednews() {
    var requestObj = {
        page: 1,
        size: 3,
        category_id: 2,
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
                                    <a class="thumb_img thumb_5x3" href="/News/Detail/${item.id}">
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
                                        <a href="/News/Detail/${item.id}">${item.title}</a>
                                    </h3>
                                    <p style=" width: 100%;" class="des">${item.lead}</p>
                                    <div><a class="read-more" href="/News/Detail/${item.id}">Đọc thêm</a></div>
                                </div>
                            </div>`
                }
                $('#Related_news').html(rows);
            }

            /* $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");*/

        }
    });
}
function openview() {
    
    $("body").on("click", ".link", function () {
        var title = "Hotline tư vấn";
        var url = "/contact"
        let param = {
            id: 0
        };
        _magnific.OpenSmallPopup(title, url, param);

    });
}
