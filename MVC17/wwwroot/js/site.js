
$(document).on("submit", ".add-to-cart-form", function (e) {
    e.preventDefault();

    let form = $(this);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function (res) {
            $("#cartModalMessage").text(res.message);

            let modal = new bootstrap.Modal(document.getElementById("cartModal"));
            modal.show();
        },
        error: function () {
            $("#cartModalMessage").text("Có lỗi xảy ra.");

            let modal = new bootstrap.Modal(document.getElementById("cartModal"));
            modal.show();
        }
    });
});
