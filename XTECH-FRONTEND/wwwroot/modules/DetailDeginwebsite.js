$(document).ready(function () {
    getNewsCategory();
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
                    rows += `<div class="item">
                                <a class="thumb_img thumb_2x3" href="#">
                                    @htm.Raw(${item.body})
                                </a>
                                <div class="content">
                                    <h3 class="name"><a href="${item.directlink}">${item.title}</a></h3>
                                    <a class="read-more" href="${item.directlink}">Đọc thêm</a>
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