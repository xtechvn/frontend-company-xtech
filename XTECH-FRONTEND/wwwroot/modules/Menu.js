﻿$(document).ready(function () {
    GetListCategory();
    
    $('body').on('click', '.menu-tab-button', function (event) {
        var element = $(this)
        $('.menu-tab-button').removeClass('active')
        element.addClass('active')
    });
    
});
function GetListCategory() {
    var rows = "";
    var rows2 = "";
    $.ajax({
        url: "/News/GetNewsCategory",
        type: "Post",

        data: { id: 1 },
        success: function (result) {
            if (result != undefined && result.categories != null) {
                for (var i in result.categories) {
                    var item = result.categories[i];
                    if (item.listCategoryResponse != null) {
                        for (var j in item.listCategoryResponse) {
                            var item2 = item.listCategoryResponse[j];
                            rows2 += `<div class="level2">
                                                <a href="/${item2.url_path}">${item2.name}</a>
                                            </div>`
                        }
                        rows += `<li class="menu-tab-button menu-tab-button-${item.url_path} default">
                                         <a href="/${item.url_path}">${item.name}</a>
                                         <span class="sub_menu">+</span>
                                         ${rows2}
                                    </li>`
                    } else {
                        rows += `<li class="menu-tab-button menu-tab-button-${item.url_path} ">
                                        <a href="/${item.url_path}">${item.name}</a>
                                    </li>`
                    }

                }
                var html = `<div class="container">
                                        <div class="flex flex_row">
                                            <h1 class="logo" title="new-ca">
                                                <a href="/"><img src="/images/graphics/Logo_XTech_White.svg" alt=""></a>
                                            </h1>
                                            <div class="right_menu">
                                                <a href="javascript:;" id="vibeji-ham" class="item"><span></span> <span></span> <span></span></a>
                                                <div class="flex flex_right">
                                                    <div class="main-menu">
                                                        <ul class="nav">
                                                            ${rows}
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`
                $('#grid_data_ListCategory').html(html);
            }
        }
    });

}