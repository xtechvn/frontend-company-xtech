

$('body').on('click', '.menu-tab-button', function (event) {
    var element = $(this)
    $('.menu-tab-button').removeClass('active')
    switch (element.attr('data-tab-id')) {
        case '0': {
            $('.menu-tab-button-Home').addClass('active');

        } break;
        case '1': {
            $('.menu-tab-button-Pricing').addClass('active');

        } break;
        case '2': {
            $('.menu-tab-button-FAQ').addClass('active');

        } break;
        case '3': {
            $('.menu-tab-button-new').addClass('active');

        } break;
        case '4': {
            $('.menu-tab-button-Contact').addClass('active');

        } break;

    }

});

