// default when load page
var articles = [];
var pinnedArticles = [];
var pageAllFareCurrent = 1;
var paginateArticles = [];
var selectedCategory = "";
var selectedCategoryId = 0;
$(document).ready(function () {

    var path = window.location.pathname;
    getNewsCategory();
    //getNewsCategory2();
    //getNewsCategory3();
});


function getNewsCategory(page) {
    var requestObj = {
        page: page == null ? 1 : page,
        size: size =  20 ,
        category_id: 40,
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
                  
                    rows += `<div class="article-itemt full">
                                <div class="article-thumb">
                                    <a class="thumb_img thumb_5x3" href="Detail/${item.id}">
                                        <img src="${item.image_11}" alt="">
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
                                        <a href="Detail/${item.id}">${item.title}</a>
                                    </h3>
                                    <p style=" width: 100%;" class="des">${item.lead}</p>
                                    <div><a class="read-more" href="Detail/${item.id}">Đọc thêm</a></div>
                                </div>
                            </div>`
                }
                $('#grid_data_New').html(rows);
            }
        }
    });


}


function getNewsCategory2() {
    var requestObj = {
        page: 1,
        size: 10,
        category_id: 40,
    }
    var rows = "";
    var swiper = "";
    $.ajax({
        url: "/News/GetlistNews",
        type: "Post",
        data: { requestObj },
        success: function (result) {
            if (result.data != null) {
                for (var i in result.data) {
                    var item = result.data[i];
                    rows += `<div class="swiper-slide ">
                                <div class="article-itemt full">
                                    <div class="article-thumb">
                                        <a class="thumb_img thumb_5x3" href="Detail/${item.id}">
                                            <img src="${item.image_11}" alt="">
                                        </a>
                                    </div>
                                    <div class="article-content">
                                        <h3 class="title_new">
                                              <a href="Detail/${item.id}">${item.title}</a>
                                        </h3>
                                        <div class="date">
                                            <svg class="icon-svg">
                                                <use xlink:href="/images/icons/icon.svg#date"></use>
                                            </svg>
                                            <span>${item.publish_date}</span>
                                        </div>
                                    </div>
                                </div>
                            </div>`
                }
                $('#grid_data_slide').html(rows);

            }
        }
    });


}
function getNewsCategory3() {
    var requestObj = {
        page: 1,
        size: 10,
        category_id: 40,
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
                    rows += `<div class="article-itemt">
                                <div class="article-thumb">
                                    <a class="thumb_img thumb_5x5" href="Detail/${item.id}">
                                        <img src="${item.image_11}" alt="">
                                    </a>
                                </div>
                                <div class="article-content">
                                    <h3 class="title_new">
                                         <a href="Detail/${item.id}">${item.title}</a>
                                    </h3>
                                    <div class="date">
                                        <svg class="icon-svg">
                                            <use xlink:href="/images/icons/icon.svg#date"></use>
                                        </svg>
                                        <span>${item.publish_date}</span>
                                    </div>
                                </div>
                            </div>`
                }
                $('#grid_data2_new').html(rows);
            }


        }
    });


}
// Click button pafination to go to page
$("body").on("click", ".page-item", function () {
    if ($(this).hasClass('prev-page') && pageAllFareCurrent > 1) {
        pageAllFareCurrent = pageAllFareCurrent - 1;
    }
    if ($(this).hasClass('next-page') && pageAllFareCurrent < arrPagination.length) {
        pageAllFareCurrent = pageAllFareCurrent + 1;
    }
    if ($(this).hasClass('page-number')) {
        pageAllFareCurrent = $(this).data('page');
    }
    $('.page-item').removeClass('active');
    $('.page-item').eq(pageAllFareCurrent).addClass('active');

    getNewsCategory( pageAllFareCurrent)
    // scroll top
    $('html, body').animate({
        scrollTop: $("#section-article-paginate").offset().top - 100
    });
});

