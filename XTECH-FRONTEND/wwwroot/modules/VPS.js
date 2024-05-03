
let mem = 10;
let cpu = 10;
let ssd = 1;
let net = 1;
let nip = 1;
let nMonth = 2;
let quantity = 2;
let key = "vps_obj";
function priceCount_vps(idOption = null) {

    if (idOption == undefined)
        idOption = '';

    var mem = $('#select_vps_custom_mem' + idOption + ' option:selected').attr('value');
    var cpu = $('#select_vps_custom_cpu' + idOption + ' option:selected').attr('value');
    var ssd = $('#select_vps_custom_ssd' + idOption + ' option:selected').attr('value');
    var net = $('#select_vps_custom_net' + idOption + ' option:selected').attr('value');
    var nip = $('#select_vps_custom_nip' + idOption + ' option:selected').attr('value');
    var nMonth = $('#select_vps_time' + idOption + ' option:selected').attr('value');

    var quantity = $('#select_vps_quantity' + ' option:selected').attr('value');
    var affiliate_code = $('#affiliate_code').text();



    if (!(mem > 0 && cpu > 0 && ssd > 0 && net > 0 && nip > 0))
        return;

    $("#price_select_vps" + idOption).html('...');
    //var urlOK = "https://galaxycloud.vn/a_p_i/public-hosting/get-price/?type=vps&cpu=" + cpu + '&mem=' + mem + '&ssd=' + ssd + '&net=' + net + '&nip=' + nip + '&nMonth=' + nMonth + '&quantity=1';

    var requestObj = {
        CPU: cpu,
        Memory: mem,
        SSD: ssd,
        net: net,
        nip: nip,
        nMonth: nMonth,
        quantity: quantity == undefined ? 1 : quantity,
    }
    $.ajax({
        url: "/FAQ/GetPriceGalaxy",
        type: "Post",
        data: { data: requestObj },
        success: function (result) {
            if (result != undefined && result.errorNumber == 0) {

                console.log("RET = ", result.payload);

                $("#price_select_vps" + idOption).html(_global_function.Comma(result.payload+"000"));

                var totalPrice = requestObj.quantity * parseFloat(nMonth) * result.payload;

                console.log(" total price2 = " + totalPrice);
                $('#totalPricepriceVpsCustom' + idOption).html("Chi phí " + nMonth + " Tháng: " + _global_function.Comma(totalPrice+"000") + " đ ");
            }
        }
    });

}

$(document).ready(function () {
    priceCount_vps("co-ban");
    priceCount_vps("pho-bien");
    priceCount_vps("cao-cap");
    priceCount_vps("");
    $("[id^=dk_vps]").click(function () {
        var id = $(this).attr('id');
        var id = $(this).attr('data-opt');

        var idItem = $(this).attr('data-opt');

        if (idItem == undefined)
            idItem = '';

        var mem = $('#select_vps_custom_mem' + idItem + ' option:selected').attr('value');
        var cpu = $('#select_vps_custom_cpu' + idItem + ' option:selected').attr('value');
        var ssd = $('#select_vps_custom_ssd' + idItem + ' option:selected').attr('value');
        var net = $('#select_vps_custom_net' + idItem + ' option:selected').attr('value');
        var nip = $('#select_vps_custom_nip' + idItem + ' option:selected').attr('value');
        var nMonth = $('#select_vps_time' + idItem + ' option:selected').attr('value');
        var quantity = $('#select_vps_quantity' + idItem + ' option:selected').attr('value');

        var obj = {
            mem: mem,
            cpu: cpu,
            ssd: ssd,
            net: net,
            nip: nip,
            nMonth: nMonth,
            quantity: quantity
        }

        localStorage.setItem(key, JSON.stringify(obj))
        window.location.href = '/dich-vu/dang-ky-vps'
    });
    $("[id^=select_vps_custom_]").change(function () {

        var idOptionVps = $(this).attr('data-opt');
        priceCount_vps(idOptionVps);
    });

    $("[id^=select_vps_quantity]").change(function () {

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

        $('#pricePack_vps' + idItem).html($('#select_vps_time' + idItem).find('option:selected').attr("name") + "K");

        getPriceFixVps(idItem);

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
        var priceMonth = $('#price_select_vps' + idItem).text();
        var quantity = $('#select_vps_quantity' + idItem).find('option:selected').attr("name");
        var totalPrice = nMonth * parseFloat(priceMonth.replaceAll(',', '')) * quantity;
        var totalPricequantity = parseFloat(priceMonth.replaceAll(',', '')) * quantity;

        $("#prePayPackage" + idItem).attr("href", "/buy/hosting/item/id/" + idItem + "/month/" + nMonth + '/quantity/' + quantity);
        $("#payPackage" + idItem).attr("href", "/buy/hosting/pre-pay/id/" + idItem + "/month/" + nMonth + '/quantity/' + quantity);

        $('#totalPricepriceVpsCustom' + idItem).html("Chi phí " + nMonth + " Tháng: " + _global_function.Comma(totalPrice+"000") + "đ ");

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
        priceCount_vps(idOptionVps);

    });
});
function LaodDetailVPS() {
    $('body').on('click', '#booking_vps', function (event) {
        LoadThongso(1)
    });
    $('body').on('click', '#btn_thanh_toan', function (event) {
        dangkyvps()
    });
    $('body').on('click', '#btn_huy_thanh_toan', function (event) {
        LoadThongso(2)
    });
    const itemStr = localStorage.getItem(key)
    var data = JSON.parse(itemStr);

    $('#select_vps_custom_mem').val(parseFloat(data.mem)).attr("selected", "selected");
    $('#select_vps_custom_cpu').val(parseFloat(data.cpu)).attr("selected", "selected");
    $('#select_vps_custom_ssd').val(parseFloat(data.ssd)).attr("selected", "selected");
    $('#select_vps_custom_net').val(parseFloat(data.net)).attr("selected", "selected");
    $('#select_vps_custom_nip').val(parseFloat(data.nip)).attr("selected", "selected");
    $('#select_vps_time').val(parseFloat(data.nMonth)).attr("selected", "selected");
    $('#select_vps_quantity').val(parseFloat(data.quantity)).attr("selected", "selected");
    priceCount_vps('');
}
function dangkyvps() {
    let FromCreate = $('#dang_ky_vps');
    FromCreate.validate({
        rules: {
            "name": "required",
            "sdt": "required",
            "email": "required",
        },
        messages: {
            "name": "Họ tên không được bỏ trống",
            "sdt": "Số điện thoại không được bỏ trống",
            "email": "Email không được bỏ trống",
         
        }
    });
    if (FromCreate.valid()) {
        var requestObj = {
            CPU: $('#select_vps_custom_cpu').val(),
            Memory: $('#select_vps_custom_mem').val(),
            SSD: $('#select_vps_custom_ssd').val(),
            net: $('#select_vps_custom_net').val(),
            nip: $('#select_vps_custom_nip').val(),
            nMonth: $('#select_vps_time').val(),
            quantity: $('#select_vps_quantity').val(),
            Clientid: 1,
            Amount: $('#price_select_vps').text().replaceAll(',', ''),
            Name: $('#name').val(),
            Sdt: $('#sdt').val(),
            Email: $('#email').val(),
        }

        $.ajax({
            url: "/FAQ/BookingVPS",
            type: "Post",
            data: { data: requestObj },
            success: function (result) {
                if (result != undefined && result.status == 0) {
                    _msgalert.success(result.msg);
                } else {
                    _msgalert.error(result.msg);
                }
            }
        });
    }

}
function LoadThongso(type) {
    if (type == 1) {
        $('#Load_vps_thong_so').hide();
        $('#cpu').html($('#select_vps_custom_cpu').val() + ' VCore')
        $('#mem').html($('#select_vps_custom_mem').val() + ' GB')
        $('#ssd').html($('#select_vps_custom_ssd').val() + ' GB')
        $('#net').html($('#select_vps_custom_net' + ' Mbit').val())
        $('#nip').html($('#select_vps_custom_nip').val())
        $('#nMonth').html($('#select_vps_time').val() + ' tháng')
        $('#amount').html(_global_function.Comma(Math.round(parseFloat($('#price_select_vps').text().replaceAll(',', '')) / parseFloat($('#select_vps_time').val()))) + ',000 đ (VND)')
        $('#quantity').html($('#select_vps_quantity').val())
        $('#totalPric').html($('#price_select_vps').text() + ',000 VND (' + $('#select_vps_time').val() + ' x ' + _global_function.Comma(Math.round(parseFloat($('#price_select_vps').text().replaceAll(',', '')) / parseFloat($('#select_vps_time').val()))) + ' x ' + $('#select_vps_quantity').val() + ')' )
        var a = _global_function.Comma(Math.round(parseFloat($('#price_select_vps').text().replaceAll(',', '')) / parseFloat($('#select_vps_time').val())));
        $('#thong_tin_dang_ky').show();
    } else {
        $('#Load_vps_thong_so').show();
        $('#thong_tin_dang_ky').hide()
    }

}