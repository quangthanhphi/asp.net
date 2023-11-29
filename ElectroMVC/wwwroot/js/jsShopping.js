$(document).ready(function () {
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
                    alert(rs.countc);
                    $('#checkout_items').html(rs.countc);
                    alert(rs.msg);         
                }
            }
        });
    });
});
