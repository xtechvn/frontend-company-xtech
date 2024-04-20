// default when load page
var articles = [];
var pinnedArticles = [];
var pageAllFareCurrent = 1;
var paginateArticles = [];
var selectedCategory = "";
var selectedCategoryId = 0;
let pagenum = 1;
$(document).ready(function () {

    var path = window.location.pathname;
    getNewsCategory();
   
});


function getNewsCategory(page) {

    var requestObj = {
        page: page == null ? 1 : page,
        size: size = 12,
        category_id: 1004,
    }
    pagenum = requestObj.page;
    var rows = "";
    var pagination = "";

    $.ajax({
        url: "/News/GetlistNews",
        type: "Post",
        data: { requestObj },
        success: function (result) {
            if (result != undefined && result.data != null) {
                for (var i in result.data) {
                    var item = result.data[i];
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
                var total_page = Math.ceil(result.total_item / requestObj.size);
                for (var i = 1; i <= total_page; i++) {
                    pagination += `<li data-page="${i}" class="page-${i} page-item ${i == pagenum ? 'active' : ''}"><a onclick="getNewsCategory(${i})" class="page-link" href="javascripts:;">${i}</a></li>`;
                }
                var paginationHtml = `<ul class="pagination mb40">
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;"onclick="getNewsCategory(1)" aria-label="Previous">
                                                <span aria-hidden="true"><i class="fa fa-angle-left"></i></span>
                                                <span class="sr-only">Previous</span>
                                            </a>
                                        </li>
                                       ${pagination}
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;" onclick="getNewsCategory(${total_page})" aria-label="Next">
                                                <span aria-hidden="true"><i class="fa fa-angle-right"></i></span>
                                                <span class="sr-only">Next</span>
                                            </a>
                                        </li>
                                    </ul>`;

                if (pagination != "")
                    $('#grid_data_pagination').html(paginationHtml);
                $('#grid_data_New').html(rows);
            }
        }
    });

    
}


