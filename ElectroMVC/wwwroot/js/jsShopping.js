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
    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);

        
    });
    $('.soluong').change(function () {
        var id = $(this).data('id');
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);

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
                        LoadCart();
                        location.reload();
                    }
                }
            });
        } 
    });

    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm("Bạn có chắc muốn xóa hết sản phẩm trong giỏ hàng?");
        if (conf == true) {
            DeleteAll();
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

function DeleteAll() {
    $.ajax({
        url: '/ShoppingCart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            //code để debug
            //console.log(rs);
            if (rs.success) {
                console.log("Xóa Thành công");
                LoadCart();
                $('#checkout_items').html(rs.countc);
                location.reload();

            }
        }
    });
}

function Update(id,quantity) {
    $.ajax({
        url: '/ShoppingCart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            //code để debug
            //console.log(rs);
            if (rs.success) {
                console.log("Update Thành công");
                LoadCart();
                location.reload();

            }
        }
    });
}

function LoadCart() {
    $.ajax({
        url: '/ShoppingCart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            //code để debug
            console.log("Load Thành công");
            $('#load_data').html(rs);
            console.log(rs);
        }
    });
}