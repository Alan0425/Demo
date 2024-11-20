var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/customer/home/getall'
        },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            { data: 'size', "width": "10%" },
            { data: 'price', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<input type="radio" id="ice_${data}" name="ice_${data}" value="正常冰"/><label>正常冰</label>
                    <input type="radio" id="ice_${data}" name="ice_${data}" value="少冰"/><label>少冰</label>
                    <input type="radio" id="ice_${data}" name="ice_${data}" value="微冰"/><label>微冰</label>
                    <input type="radio" id="ice_${data}" name="ice_${data}" value="去冰"/><label>去冰</label>
                    <input type="radio" id="ice_${data}" name="ice_${data}" value="溫"/><label>溫</label>
                    <input type="radio" id="ice_${data}" name="ice_${data}" value="熱"/><label>熱</label><br>
                    <input type="radio" id="sweet_${data}" name="sweet_${data}" value="正常甜"/><label>正常甜</label>
                    <input type="radio" id="sweet_${data}" name="sweet_${data}" value="少糖"/><label>少糖</label>
                    <input type="radio" id="sweet_${data}" name="sweet_${data}" value="半糖"/><label>半糖</label>
                    <input type="radio" id="sweet_${data}" name="sweet_${data}" value="微糖"/><label>微糖</label>
                    <input type="radio" id="sweet_${data}" name="sweet_${data}" value="無糖"/><label>無糖</label>`
                },
                "width": "40%"
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<input id="count_${data}" type="number" value="0" min="0">`
                },
                "width": "5%"
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a onClick=Check('${data}') class="btn btn-primary mx-2"> <i class="bi bi-cart"></i> </a>
                    </div>`
                },
                "width": "10%"
            }
        ]
    });
}

function Check(id) {
    $("#productId").val(id);
    $("#ice").val($('input:radio:checked[name="ice_' + id + '"]').val());
    $("#sweet").val($('input:radio:checked[name="sweet_' + id + '"]').val());
    $("#count").val($("#count_" + id).val());

    var error = "";
    if ($("#ice").val() == "") error += "請選擇冰量 ";
    if ($("#sweet").val() == "") error += " 請選擇甜度 ";
    if ($("#count").val() == 0) error += " 請選擇數量 ";

    if (error == "") Cart(id);
    else alert(error);

}

function Cart(id) {
    $.ajax({
        url: '/customer/home/AddDetail',
        type: 'POST',
        data: jQuery.param({ ProductId: id, ice: $("#ice").val(), sweet: $("#sweet").val(), count: $("#count").val() }),
        success: function (data) {
            //alert(data.message);
            //toastr.success(data.message);
            $("#cartSpan").fadeIn(1000);
            $("#cartSpan").fadeOut(1000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("請先登入");
        }   
    })
}

