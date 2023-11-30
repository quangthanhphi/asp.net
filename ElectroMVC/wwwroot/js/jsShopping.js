$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();

        var id = $(this).data('id');
        var quantity = 1;
        var tQuantity = $('#quantity_value').text();
        if (tQuantity != '') {
            quantity = parseInt(tQuantity);
        }

        $.ajax({
            url: '/ShoppingCart/AddToCart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                //code để debug
                //console.log(rs);
                if (rs.success) {
                    //alert(rs.countc);
                    $('#checkout_items').html(rs.countc);
                    alert(rs.msg);         
                }
            }
        });
    });
    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var conf = confirm("Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?");
        if (conf == true) {
            $.ajax({
                url: '/ShoppingCart/Delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    //code để debug
                    //console.log(rs);
                    if (rs.success) {
                        console.log(rs);
                        $('#checkout_items').html(rs.countc);
                        //alert(rs.msg);
                        $('#trow_' + id).remove();
                        location.reload();
                    }
                }
            });
        }
        
    });
});

function ShowCount() {
    $.ajax({
        url: '/ShoppingCart/ShowCount',
        type: 'GET',
        success: function (rs) {
            //code để debug
            console.log(rs);
            $('#checkout_items').html(rs.countc);
        }
    });
}