
//$(document).ready(function () {
//    getPriceCoBan();
//    getPriceCoBan2();
//    getPriceCoBan3();
//    getPriceCoBan4();


//}); 
//function getPriceCoBan() {

//    var requestObj = {
//        CPU: $('#select_vps_custom_cpuco-ban').val(),
//        Memory: $('#select_vps_custom_memco-ban').val(),
//        SSD: $('#select_vps_custom_ssdco-ban').val(),
//        net: $('#select_vps_custom_netco-ban').val(),
//        nip: $('#select_vps_custom_nipco-ban').val(),
//        nMonth: $('#select_vps_time0co-ban').val(),
//        quantity: 1,
//    }
//    //var requestObj = {
//    //    CPU: 1,
//    //    Memory: 1,
//    //    SSD: 20,
//    //    net:100,
//    //    nip: 1,
//    //    nMonth: 1,
//    //    quantity: 1,
//    //}

//    $('#price_co_ban').html(`.....`)
//    $.ajax({
//        url: "/FAQ/GetPriceGalaxy",
//        type: "Post",
//        data: {data:requestObj },
//        success: function (result) {
//            if (result != undefined && result.errorNumber == 0) {

//                $('#price_co_ban').html(result.payload)
//                $('#Chi_phi_price_co_ban').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
//            }
//        }
//    });


//}
//function getPriceCoBan2() {

//    var requestObj = {
//        CPU: $('#select_vps_custom_cpuco-ban2').val(),
//        Memory: $('#select_vps_custom_memco-ban2').val(),
//        SSD: $('#select_vps_custom_ssdco-ban2').val(),
//        net: $('#select_vps_custom_netco-ban2').val(),
//        nip: $('#select_vps_custom_nipco-ban2').val(),
//        nMonth: $('#select_vps_time0co-ban2').val(),
//        quantity: 1,
//    }
//    //var requestObj = {
//    //    CPU: 1,
//    //    Memory: 1,
//    //    SSD: 20,
//    //    net:100,
//    //    nip: 1,
//    //    nMonth: 1,
//    //    quantity: 1,
//    //}
//    $('#price_co_ban2').html(`.....`)

//    $.ajax({
//        url: "/FAQ/GetPriceGalaxy",
//        type: "Post",
//        data: { data: requestObj },
//        success: function (result) {
//            if (result != undefined && result.errorNumber == 0) {

//                $('#price_co_ban2').html(result.payload)
//                $('#Chi_phi_price_co_ban2').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
//            }
//        }
//    });


//}
//function getPriceCoBan3() {

//    var requestObj = {
//        CPU: $('#select_vps_custom_cpuco-ban3').val(),
//        Memory: $('#select_vps_custom_memco-ban3').val(),
//        SSD: $('#select_vps_custom_ssdco-ban3').val(),
//        net: $('#select_vps_custom_netco-ban3').val(),
//        nip: $('#select_vps_custom_nipco-ban3').val(),
//        nMonth: $('#select_vps_time0co-ban3').val(),
//        quantity: 1,
//    }
//    //var requestObj = {
//    //    CPU: 1,
//    //    Memory: 1,
//    //    SSD: 20,
//    //    net:100,
//    //    nip: 1,
//    //    nMonth: 1,
//    //    quantity: 1,
//    //}

//    $('#price_co_ban3').html(`.....`)
//    $.ajax({
//        url: "/FAQ/GetPriceGalaxy",
//        type: "Post",
//        data: { data: requestObj },
//        success: function (result) {
//            if (result != undefined && result.errorNumber == 0) {

//                $('#price_co_ban3').html(result.payload)
//                $('#Chi_phi_price_co_ban3').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
//            }
//        }
//    });


//}
//function getPriceCoBan4() {

//    var requestObj = {
//        CPU: $('#select_vps_custom_cpuco-ban4').val(),
//        Memory: $('#select_vps_custom_memco-ban4').val(),
//        SSD: $('#select_vps_custom_ssdco-ban4').val(),
//        net: $('#select_vps_custom_netco-ban4').val(),
//        nip: $('#select_vps_custom_nipco-ban4').val(),
//        nMonth: $('#select_vps_time0co-ban4').val(),
//        quantity: 1,
//    }
//    //var requestObj = {
//    //    CPU: 1,
//    //    Memory: 1,
//    //    SSD: 20,
//    //    net:100,
//    //    nip: 1,
//    //    nMonth: 1,
//    //    quantity: 1,
//    //}
//    $('#price_co_ban4').html(`.....`)

//    $.ajax({
//        url: "/FAQ/GetPriceGalaxy",
//        type: "Post",
//        data: { data: requestObj },
//        success: function (result) {
//            if (result != undefined && result.errorNumber == 0) {

//                $('#price_co_ban4').html(result.payload)
//                $('#Chi_phi_price_co_ban4').html(`chi phí ${requestObj.nMonth} tháng:${result.payload * requestObj.nMonth} K`)
//            }
//        }
//    });
//}
//function PrevCoBan(id) {
//    switch (id) {
//        case 1: {
//            var selec_cpu = parseFloat($('#select_vps_custom_cpuco-ban').val());
//            if (selec_cpu <= 2) {
//                $('#btn_prev_cpu_co_ban').removeClass('plus')
//                $('#btn_prev_cpu_co_ban').addClass('minus')
//                $('#select_vps_custom_cpuco-ban').val(1).attr("selected", "selected")

//            } else {
//                $('#btn_prev_cpu_co_ban').removeClass('minus')
//                $('#btn_prev_cpu_co_ban').addClass('plus')
//                $('#select_vps_custom_cpuco-ban').val(selec_cpu - 1).attr("selected", "selected")
//            }

//            getPriceCoBan()
//        } break;
//        case 2: {
//            var selec_cpu = parseFloat($('#select_vps_custom_memco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_prev_mem_co_ban').removeClass('minus')
//                $('#btn_prev_mem_co_ban').addClass('plus')
//                $('#select_vps_custom_cpuco-ban').val( selec_cpu - 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_prev_mem_co_ban').removeClass('plus')
//                $('#btn_prev_mem_co_ban').addClass('minus')
//            }
//        } break;
//        case 3: {
//            var selec_cpu = parseFloat($('#select_vps_custom_ssdco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_prev_ssd_co_ban').removeClass('minus')
//                $('#btn_prev_ssd_co_ban').addClass('plus')
//                $('#select_vps_custom_ssdco-ban').val(selec_cpu - 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_prev_ssd_co_ban').removeClass('plus')
//                $('#btn_prev_ssd_co_ban').addClass('minus')
//            }
//        } break;
//        case 4: {
//            var selec_cpu = parseFloat($('#select_vps_custom_nipco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_prev_nip_co_ban').removeClass('minus')
//                $('#btn_prev_nip_co_ban').addClass('plus')
//                $('#select_vps_custom_nipco-ban').val(selec_cpu - 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_prev_nip_co_ban').removeClass('plus')
//                $('#btn_prev_nip_co_ban').addClass('minus')
//            }
//        } break;
//        case 5: {
//            var selec_cpu = parseFloat($('#select_vps_time0co-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_prev_time_co_ban').removeClass('minus')
//                $('#btn_prev_time_co_ban').addClass('plus')
//                $('#select_vps_time0co-ban').val(selec_cpu - 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_prev_time_co_ban').removeClass('plus')
//                $('#btn_prev_time_co_ban').addClass('minus')
//            }
//        } break;
//    }

//}
//function NextCoBan(id){
//    switch (id) {
//        case 1: {
//            var selec_cpu = parseFloat($('#select_vps_custom_cpuco-ban').val());
//            if (selec_cpu != 2000 ) {
//                $('#btn_prev_cpu_co_ban').removeClass('minus')
//                $('#btn_prev_cpu_co_ban').addClass('plus')
//                $('#select_vps_custom_cpuco-ban').val(selec_cpu + 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#select_vps_custom_cpuco-ban').val(2000).attr("selected", "selected")
//                getPriceCoBan()
//                $('#btn_next_cpu_co_ban').removeClass('plus')
//                $('#btn_next_cpu_co_ban').addClass('minus')
//            }
//        } break;
//        case 2: {
//            var selec_cpu = parseFloat($('#select_vps_custom_memco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_next_mem_co_ban').removeClass('minus')
//                $('#btn_next_mem_co_ban').addClass('plus')
//                $('#select_vps_custom_memco-ban').val(selec_cpu + 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_next_mem_co_ban').removeClass('plus')
//                $('#btn_next_mem_co_ban').addClass('minus')
//            }
//        } break;
//        case 3: {
//            var selec_cpu = parseFloat( $('#select_vps_custom_ssdco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_next_ssd_co_ban').removeClass('minus')
//                $('#btn_next_ssd_co_ban').addClass('plus')
//                $('#select_vps_custom_ssdco-ban').val(selec_cpu + 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_next_ssd_co_ban').removeClass('plus')
//                $('#btn_next_ssd_co_ban').addClass('minus')
//            }
//        } break;
//        case 4: {
//            var selec_cpu = parseFloat( $('#select_vps_custom_nipco-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_next_nip_co_ban').removeClass('minus')
//                $('#btn_next_nip_co_ban').addClass('plus')
//                $('#select_vps_custom_nipco-ban').val(selec_cpu + 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_next_nip_co_ban').removeClass('plus')
//                $('#btn_next_nip_co_ban').addClass('minus')
//            }
//        } break;
//        case 5: {
//            var selec_cpu = parseFloat($('#select_vps_time0co-ban').val());
//            if (selec_cpu > 1) {
//                $('#btn_next_time_co_ban').removeClass('minus')
//                $('#btn_next_time_co_ban').addClass('plus')
//                $('#select_vps_time0co-ban').val(selec_cpu + 1).attr("selected", "selected")
//                getPriceCoBan()
//            } else {
//                $('#btn_next_time_co_ban').removeClass('plus')
//                $('#btn_next_time_co_ban').addClass('minus')
//            }
//        } break;
//    }
//}


function priceCount_vps(idOption = null) {

    if (idOption == undefined)
        idOption = '';

    var mem = $('#select_vps_custom_mem' + idOption + ' option:selected').attr('value');
    var cpu = $('#select_vps_custom_cpu' + idOption + ' option:selected').attr('value');
    var ssd = $('#select_vps_custom_ssd' + idOption + ' option:selected').attr('value');
    var net = $('#select_vps_custom_net' + idOption + ' option:selected').attr('value');
    var nip = $('#select_vps_custom_nip' + idOption + ' option:selected').attr('value');
    var nMonth = $('#select_vps_time0' + idOption + ' option:selected').attr('value');

    var quantity = $('#select_vps_quantity0' + ' option:selected').attr('value');
    var affiliate_code = $('#affiliate_code').text();

    //var quantity = 1;

    console.log("ID Option: " + idOption);
    //alert("quantity = " + quantity);

    if (!(mem > 0 && cpu > 0 && ssd > 0 && net > 0 && nip > 0))
        return;
    $("#waitting_icon").show();
    $("#price_select_vps" + idOption).html('...');
    var urlOK = "http://galaxycloud.vn/a_p_i/public-hosting/get-price/?type=vps&cpu=" + cpu + '&mem=' + mem + '&ssd=' + ssd + '&net=' + net + '&nip=' + nip + '&nMonth=' + nMonth + '&quantity=1';
    console.log("URL : " + urlOK);

    $.get(urlOK, function (data, status) {
        $("#waitting_icon").hide();
        if (!ClassApi.checkReturnApi(data))
            return;

        console.log("RET = ", data.payload);

        $("#price_select_vps" + idOption).html(data.payload + "K /Tháng");

        var totalPrice = quantity * nMonth * data.payload;

        console.log(" total price2 = " + totalPrice);
        $('#totalPricepriceVpsCustom' + idOption).html("Chi phí " + nMonth + " Tháng: " + totalPrice + "K ");
        //alert("Data: " + data + "\nStatus: " + status);
    });
}

$(document).ready(function () {
    priceCount_vps("co-ban");
    priceCount_vps("pho-bien");
    priceCount_vps("cao-cap");
    priceCount_vps("");

    $("[id^=select_vps_custom_]").change(function () {
        //alert("Change...delll");
        var idOptionVps = $(this).attr('data-opt');
        priceCount_vps(idOptionVps);
    });

    $("[id^=select_vps_quantity]").change(function () {
        //alert("Change...delll");
        //alert("Change t...");
        var id = $(this).attr('id');
        var arrId = id.split('_quantity');
        var idItem = arrId[1];

        if (idItem == 0) {
            priceCount_vps('');
            return;
        }
        getPriceFixVps(idItem);
    });

    $("[id^=select_vps_time]").change(function () {
        //alert("Change t...");
        var id = $(this).attr('id');
        var arrId = id.split('_time');
        var idItem = arrId[1];

        console.log(" idItem1 = " + idItem);

        console.log(" select_vps_time value = " + $('#select_vps_time' + idItem).find('option:selected').attr("value"));

        //Price 1 month is set in name of option:
        $('#pricePack_vps' + idItem).html($('#select_vps_time' + idItem).find('option:selected').attr("name") + "K");

        getPriceFixVps(idItem);
        /*
        var nMonth = $('#select_vps_time' + idItem).find('option:selected').attr("value");
        var priceMonth = $('#select_vps_time' + idItem).find('option:selected').attr("name");
        var quantity = $('#select_vps_quantity' + idItem).find('option:selected').attr("name");
        var totalPrice = nMonth * priceMonth * quantity;


        $('#totalPricepriceVps' + idItem).html("Chi phí " + nMonth + " Tháng: " + totalPrice + "K");
        $('#pricePack_vps' + idItem).html(priceMonth + "K /Tháng");

        $("#payPackage" + idItem).attr("href", "/buy/hosting/item/id/" + idItem + "/month/" + nMonth);

        if(idItem == 0)
            priceCount_vps();
            */
    });


    $("[id^=prev_select_vps_month],[id^=next_select_vps_month]").click(function () {
        var id = $(this).attr('id');
        var arrId = id.split('_month');
        var idItem = arrId[1];

        console.log(" idItem2 = " + idItem);

        var nMonth0 = $('#select_vps_time' + idItem).find('option:selected').attr("value");

        if (id.indexOf("next_select_vps_month") >= 0)
            $('#select_vps_time' + idItem + ' option:selected').next().prop('selected', 'selected');
        if (id.indexOf("prev_select_vps_month") >= 0)
            $('#select_vps_time' + idItem + ' option:selected').prev().prop('selected', 'selected');

        var nMonth = $('#select_vps_time' + idItem).find('option:selected').attr("value");

        if (nMonth == nMonth0)
            return;


        getPriceFixVps(idItem);

        /*
        var priceMonth = $('#select_vps_time' + idItem).find('option:selected').attr("name");
        var quantity = $('#select_vps_quantity' + idItem).find('option:selected').attr("name");
        var totalPrice = nMonth * priceMonth * quantity;

        console.log(" select_vps_time value = " + $('#select_vps_time' + idItem).find('option:selected').attr("value"));
        console.log(" select_vps_time name = " + $('#select_vps_time' + idItem).find('option:selected').attr("name"));

        console.log(" total price = " + totalPrice);

        $("#payPackage" + idItem).attr("href", "/buy/hosting/item/id/" + idItem + "/month/" + nMonth);
        


        $('#totalPriceprice' + idItem).html("Chi phí " + nMonth + " Tháng: " + totalPrice + "K");
        $('#pricePack_vps' + idItem).html(priceMonth + "K /Tháng");

        if (idItem == 0)
            priceCount_vps();
            */
    });


    $("[id^=prev_select_vps_quantity],[id^=next_select_vps_quantity]").click(function () {
        var id = $(this).attr('id');
        var arrId = id.split('_quantity');
        var idItem = arrId[1];


        console.log(" idItem2 = " + idItem);

        if (id.indexOf("next_select_vps_quantity") >= 0)
            $('#select_vps_quantity' + idItem + ' option:selected').next().prop('selected', 'selected');
        if (id.indexOf("prev_select_vps_quantity") >= 0)
            $('#select_vps_quantity' + idItem + ' option:selected').prev().prop('selected', 'selected');

        getPriceFixVps(idItem);

    });

    function getPriceFixVps(idItem) {

        var nMonth = $('#select_vps_time' + idItem).find('option:selected').attr("value");
        var priceMonth = $('#select_vps_time' + idItem).find('option:selected').attr("name");
        var quantity = $('#select_vps_quantity' + idItem).find('option:selected').attr("name");
        var totalPrice = nMonth * priceMonth * quantity;

        console.log(" select_vps_time value = " + $('#select_vps_time' + idItem).find('option:selected').attr("value"));
        console.log(" select_vps_time name = " + $('#select_vps_time' + idItem).find('option:selected').attr("name"));

        console.log(" total price1 = " + totalPrice);

        $("#prePayPackage" + idItem).attr("href", "/buy/hosting/item/id/" + idItem + "/month/" + nMonth + '/quantity/' + quantity);
        $("#payPackage" + idItem).attr("href", "/buy/hosting/pre-pay/id/" + idItem + "/month/" + nMonth + '/quantity/' + quantity);

        $('#totalPricepriceVps' + idItem).html("Chi phí " + nMonth + " Tháng: " + totalPrice + "K ");
        $('#pricePack_vps' + idItem).html(priceMonth + "K /Tháng");

        if (idItem == 0)
            priceCount_vps('');
    }


    $("[id^=prev_select_vps_custom],[id^=next_select_vps_custom]").click(function () {
        var id = $(this).attr('id');

        var idOptionVps = $(this).attr('data-opt');

        var arrId = id.split('_select_vps_custom_');
        var item = arrId[1];
        console.log(" nitemx1 " + item);


        console.log(" idOptionVps1 = " + idOptionVps);

        if (id.indexOf("next_select_vps_") >= 0) {

            $('#select_vps_custom_' + item + ' option:selected').next().attr('selected', 'selected');
            $('#select_vps_custom_' + item + ' option:selected').prev().attr('selected', null);
        }
        if (id.indexOf("prev_select_vps_") >= 0) {

            $('#select_vps_custom_' + item + ' option:selected').prev().attr('selected', 'selected');
            $('#select_vps_custom_' + item + ' option:selected').next().attr('selected', null);
        }

        //$('#select_vps_custom_' + item + ' option:selected').trigger('change');

        // if(typeof idOptionVps !== 'undefined')
        priceCount_vps(idOptionVps);
        // else
        //     priceCount_vps('');
    });
});
