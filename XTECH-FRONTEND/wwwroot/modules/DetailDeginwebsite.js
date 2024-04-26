let parent_id = 1022;
$(document).ready(function () {
    getNewsCategory(1, 1022);
    GetListMenuKhoSp();

    $("body").on("click", ".send_email", function () {
        SendEmail();

    });
    $("body").on("click", "#btn_pricing1", function () {
        $('#btn_pricing1').addClass('active')
        $('#btn_pricing2').removeClass('active')
        $('#pricing2').hide()
        $('#pricing1').show()
    });
    $("body").on("click", "#btn_pricing2", function () {
        $('#btn_pricing2').addClass('active')
        $('#btn_pricing1').removeClass('active')
        $('#pricing1').hide()
        $('#pricing2').show()
    });
    $("body").on("click", "#btn_tk_khosp", function () {
        GetFindArticle(1,parent_id);

    });
    $('body').addClass('bg-white');

    const menu = document.querySelector(".toggle-menu");
    const menuLinks = menu.querySelectorAll("a");
    const menuLinkActive = menu.querySelector("li.active");
    const activeClass = "active";

    doCalculations(menuLinkActive);

    for (const menuLink of menuLinks) {
        menuLink.addEventListener("click", function (e) {
            e.preventDefault();
            menu.querySelector("li.active").classList.remove(activeClass);
            menuLink.parentElement.classList.add(activeClass);
            doCalculations(menuLink);
        });
    }

    function doCalculations(link) {
        menu.style.setProperty("--transformJS", `${link.offsetLeft}px`);
        menu.style.setProperty("--widthJS", `${link.offsetWidth}px`);
    }

    window.addEventListener("resize", function () {
        const menuLinkActive = menu.querySelector("li.active");
        doCalculations(menuLinkActive);
    });
});
function SendEmail() {
    var model = {
        Email: $('#Email').val(),
        type: 2,
    };
    $.ajax({
        url: "/Contact/SendEmail",
        type: "Post",
        data: { model },
        success: function (result) {
            if (result.status == 0) {
                _msgalert.success(result.msg);
            } else {
                _msgalert.error(result.msg);
            }
        }
    });
}
function getNewsCategory(page, category_id) {
    parent_id = category_id;
    var requestObj = {
        page: page == null ? 1 : page,
        size: size = 12,
        category_id: category_id,
    }
    pagenum = requestObj.page;
    var rows = "";
    var pagination = "";

    $.ajax({
        url: "/News/GetlistNews",
        type: "Post",
        data: { requestObj },
        success: function (result) {
            if (result != undefined && result.data != null && result.total_item > 0) {
                for (var i in result.data) {
                    var item = result.data[i];
                    rows += `<div class="item">
                                <a class="img-scroll" href="${item.directlink == "" ? "/tin-tuc/" + item.id : item.directlink}">
                                    ${item.body}

                                </a>
                                <div class="content">
                                    <h3 class="name"><a href="${item.directlink == "" ? "/tin-tuc/" + item.id : item.directlink}">${item.title}</a></h3>
                                    <a class="read-more" href="${item.directlink == "" ? "/tin-tuc/" + item.id : item.directlink}">Đọc thêm</a>
                                </div>
                            </div>`
                }
                var total_page = Math.ceil(result.total_item / requestObj.size);
                for (var i = 1; i <= total_page; i++) {
                    pagination += `<li data-page="${i}" class="page-${i} page-item ${i == pagenum ? 'active' : ''}"><a onclick="getNewsCategory(${i,parent_id})" class="page-link" href="javascripts:;">${i}</a></li>`;
                }
                var paginationHtml = `<ul class="pagination mb40">
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;"onclick="getNewsCategory(${1,parent_id})" aria-label="Previous">
                                                <span aria-hidden="true"><i class="fa fa-angle-left"></i></span>
                                                <span class="sr-only">Previous</span>
                                            </a>
                                        </li>
                                       ${pagination}
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;" onclick="getNewsCategory(${total_page,parent_id})" aria-label="Next">
                                                <span aria-hidden="true"><i class="fa fa-angle-right"></i></span>
                                                <span class="sr-only">Next</span>
                                            </a>
                                        </li>
                                    </ul>`;

                if (pagination != "")
                    $('#grid_data_pagination').html(paginationHtml);
                $('#grid_data_New').html(rows);
                $('#err_seach').html('');
            }
            else {
                rows =`<div class="min-content center mb40">
                        <h2 class="title-cate mb10">Không tìm thấy kết quả</h2>
                        <div class="mb-3">Chúng tôi không tìm thấy thông tin mà bạn cần, vui lòng thử lại
                      </div>`
                $('#err_seach').html(rows);
                $('#grid_data_New').html('');
                $('#grid_data_pagination').html('');
            }
        }
    });
}
function GetListMenuKhoSp() {
    var rows = "";
    var id = 1022;
    $.ajax({
        url: "/News/GetNewsCategory",
        type: "Post",

        data: { id: id },
        success: function (result) {
            if (result != undefined && result.categories != null ) {
                for (var i in result.categories) {
                    var item = result.categories[i];
                    rows += `<a class="swiper-slide item" href="javascript:;"  onclick="getNewsCategory(1,${item.id});" style="width: 180px !important; margin-right: 24px !important;">
                        <svg class="icon-svg">
                            <use xlink:href="${item.image_path}"></use>
                        </svg>
                        <h3>${item.name}</h3>
                    </a>`
                }
                var html = `<a class="swiper-slide item" href="javascript:;" onclick="getNewsCategory(1,1022);"   style="width: 180px !important; margin-right: 24px !important;">
                        <svg class="icon-svg">
                            <use xlink:href="/images/icons/icon.svg#all"></use>
                        </svg>
                        <h3>Tất cả</h3>
                    </a>
                    ${rows}`
                $('#menu_khosp').html(html);
                var swiper = new Swiper('#menu_khosp_slide .swiper-container ', {
                    slidesPerView: 5,
                    spaceBetween: 40,
                    pagination: {
                        el: ".swiper-pagination",
                        clickable: true,
                    },
                    navigation: {
                        nextEl: ".swiper-button-next",
                        prevEl: ".swiper-button-prev",
                    },
                });
            }
        }
    });

}
function GetFindArticle(page, category_id) {

    var request = {
        page: page == null ? 1 : page,
        size: size = 12,
        title: $('#input_khosp').val(),
        parent_id: category_id,
    }
    
    var rows = "";
    var pagination = "";

    $.ajax({
        url: "/News/GetFindArticle",
        type: "Post",
        data: { request },
        success: function (result) {
            if (result != undefined && result.data != null && result.total_item > 0) {
                for (var i in result.data) {
                    var item = result.data[i];
                    rows += `<div class="item">
                                <a class="img-scroll" href="${item.directlink == "" ? "/tin-tuc/" + item.id : item.directlink}">
                                      ${item.body}
                                </a>
                                <div class="content">
                                    <h3 class="name"><a href="${item.directlink == "" ?"/tin-tuc/"+item.id:item.directlink}">${item.title}</a></h3>
                                    <a class="read-more" href="${item.directlink == "" ? "/tin-tuc/" + item.id : item.directlink}">Đọc thêm</a>
                                </div>
                            </div>`
                }
                var total_page = Math.ceil(result.total_item / request.size);
                for (var i = 1; i <= total_page; i++) {
                    pagination += `<li data-page="${i}" class="page-${i} page-item ${i == pagenum ? 'active' : ''}"><a onclick="getNewsCategory(${i,parent_id})" class="page-link" href="javascripts:;">${i}</a></li>`;
                }
                var paginationHtml = `<ul class="pagination mb40">
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;"onclick="getNewsCategory(${1,parent_id})" aria-label="Previous">
                                                <span aria-hidden="true"><i class="fa fa-angle-left"></i></span>
                                                <span class="sr-only">Previous</span>
                                            </a>
                                        </li>
                                       ${pagination}
                                        <li class="page-item">
                                            <a class="page-link" href="javascripts:;" onclick="getNewsCategory(${total_page,parent_id})" aria-label="Next">
                                                <span aria-hidden="true"><i class="fa fa-angle-right"></i></span>
                                                <span class="sr-only">Next</span>
                                            </a>
                                        </li>
                                    </ul>`;

                if (pagination != "")
                    $('#grid_data_pagination').html(paginationHtml);
                $('#grid_data_New').html(rows);
                $('#err_seach').html('');
            }
            else {
                rows = `<div class="min-content center mb40">
                        <h2 class="title-cate mb10">Không tìm thấy kết quả</h2>
                        <div class="mb-3">Hiện tại chúng tôi không tìm thấy kết quả theo từ khóa nàyupdate
                      </div>`
                $('#err_seach').html(rows);
                $('#grid_data_New').html('');
                $('#grid_data_pagination').html('');
            }
        }
    });
}