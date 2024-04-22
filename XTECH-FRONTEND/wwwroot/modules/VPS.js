
$(document).ready(function () {
    getPriceCoBan();
    getPriceCoBan2();
    getPriceCoBan3();
    getPriceCoBan4();


}); 
function getPriceCoBan() {

    var requestObj = {
        CPU: $('#select_vps_custom_cpuco-ban').val(),
        Memory: $('#select_vps_custom_memco-ban').val(),
        SSD: $('#select_vps_custom_ssdco-ban').val(),
        net: $('#select_vps_custom_netco-ban').val(),
        nip: $('#select_vps_custom_nipco-ban').val(),
        nMonth: $('#select_vps_time0co-ban').val(),
        quantity: 1,
    }
    //var requestObj = {
    //    CPU: 1,
    //    Memory: 1,
    //    SSD: 20,
    //    net:100,
    //    nip: 1,
    //    nMonth: 1,
    //    quantity: 1,
    //}
   
    $('#price_co_ban').html(`.....`)
    $.ajax({
        url: "/FAQ/GetPriceGalaxy",
        type: "Post",
        data: {data:requestObj },
        success: function (result) {
            if (result != undefined && result.errorNumber == 0) {

                $('#price_co_ban').html(result.payload)
                $('#Chi_phi_price_co_ban').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
            }
        }
    });


}
function getPriceCoBan2() {

    var requestObj = {
        CPU: $('#select_vps_custom_cpuco-ban2').val(),
        Memory: $('#select_vps_custom_memco-ban2').val(),
        SSD: $('#select_vps_custom_ssdco-ban2').val(),
        net: $('#select_vps_custom_netco-ban2').val(),
        nip: $('#select_vps_custom_nipco-ban2').val(),
        nMonth: $('#select_vps_time0co-ban2').val(),
        quantity: 1,
    }
    //var requestObj = {
    //    CPU: 1,
    //    Memory: 1,
    //    SSD: 20,
    //    net:100,
    //    nip: 1,
    //    nMonth: 1,
    //    quantity: 1,
    //}
    $('#price_co_ban2').html(`.....`)

    $.ajax({
        url: "/FAQ/GetPriceGalaxy",
        type: "Post",
        data: { data: requestObj },
        success: function (result) {
            if (result != undefined && result.errorNumber == 0) {

                $('#price_co_ban2').html(result.payload)
                $('#Chi_phi_price_co_ban2').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
            }
        }
    });


}
function getPriceCoBan3() {

    var requestObj = {
        CPU: $('#select_vps_custom_cpuco-ban3').val(),
        Memory: $('#select_vps_custom_memco-ban3').val(),
        SSD: $('#select_vps_custom_ssdco-ban3').val(),
        net: $('#select_vps_custom_netco-ban3').val(),
        nip: $('#select_vps_custom_nipco-ban3').val(),
        nMonth: $('#select_vps_time0co-ban3').val(),
        quantity: 1,
    }
    //var requestObj = {
    //    CPU: 1,
    //    Memory: 1,
    //    SSD: 20,
    //    net:100,
    //    nip: 1,
    //    nMonth: 1,
    //    quantity: 1,
    //}

    $('#price_co_ban3').html(`.....`)
    $.ajax({
        url: "/FAQ/GetPriceGalaxy",
        type: "Post",
        data: { data: requestObj },
        success: function (result) {
            if (result != undefined && result.errorNumber == 0) {

                $('#price_co_ban3').html(result.payload)
                $('#Chi_phi_price_co_ban3').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
            }
        }
    });


}
function getPriceCoBan4() {

    var requestObj = {
        CPU: $('#select_vps_custom_cpuco-ban4').val(),
        Memory: $('#select_vps_custom_memco-ban4').val(),
        SSD: $('#select_vps_custom_ssdco-ban4').val(),
        net: $('#select_vps_custom_netco-ban4').val(),
        nip: $('#select_vps_custom_nipco-ban4').val(),
        nMonth: $('#select_vps_time0co-ban4').val(),
        quantity: 1,
    }
    //var requestObj = {
    //    CPU: 1,
    //    Memory: 1,
    //    SSD: 20,
    //    net:100,
    //    nip: 1,
    //    nMonth: 1,
    //    quantity: 1,
    //}
    $('#price_co_ban4').html(`.....`)

    $.ajax({
        url: "/FAQ/GetPriceGalaxy",
        type: "Post",
        data: { data: requestObj },
        success: function (result) {
            if (result != undefined && result.errorNumber == 0) {

                $('#price_co_ban4').html(result.payload)
                $('#Chi_phi_price_co_ban4').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
            }
        }
    });


}