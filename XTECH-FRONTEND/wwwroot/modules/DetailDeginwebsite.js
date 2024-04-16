$(document).ready(function () {

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
                _msgalert.success(result.smg);
            } else {
                _msgalert.error(result.smg);
            }
        }
    });
}