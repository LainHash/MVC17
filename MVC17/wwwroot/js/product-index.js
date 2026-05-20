$(document).ready(function () {
    function loadProducts(url) {
        $.ajax({
            url: url,
            type: 'GET',
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            success: function (result) {
                $('#product-list-container').html(result);
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi tải sản phẩm: ", error);
            }
        });
    }

    var debounceTimer;
    $('#filter-form select, #filter-form input').on('change input', function (e) {
        var isCategoryChange = e.target.id === 'categoryId';
        
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function() {
            var form = $('#filter-form');
            
            if (isCategoryChange) {
                var catId = $('#categoryId').val();
                $.ajax({
                    url: '/Product/GetSuppliersJson',
                    type: 'GET',
                    data: { categoryId: catId },
                    success: function(data) {
                        var dropdown = $('#btnSupplier').siblings('.dropdown-menu');
                        dropdown.empty();
                        
                        var allHtml = '<li><a class="dropdown-item d-flex justify-content-between align-items-center custom-dropdown-item py-2" href="#" data-val="0" data-text="-- Tất cả nhà cung cấp --" data-target="supplierId" data-display="btnSupplier"><span class="fw-bold text-primary">-- Tất cả nhà cung cấp --</span></a></li>';
                        dropdown.append(allHtml);

                        data.forEach(function(sup) {
                            var itemHtml = '<li><a class="dropdown-item d-flex justify-content-between align-items-center custom-dropdown-item py-2" href="#" data-val="' + sup.supplierId + '" data-text="' + sup.companyName + '" data-target="supplierId" data-display="btnSupplier"><span class="">' + sup.companyName + '</span><span class="badge bg-light text-dark rounded-pill">' + sup.productCount + '</span></a></li>';
                            dropdown.append(itemHtml);
                        });

                        $('#supplierId').val(0);
                        $('#btnSupplier').text('-- Tất cả nhà cung cấp --');
                        
                        var url = form.attr('action') + '?' + form.serialize();
                        loadProducts(url);
                    }
                });
            } else {
                var url = form.attr('action') + '?' + form.serialize();
                loadProducts(url);
            }
        }, 400); // 400ms delay cho AJAX
    });

    $(document).on('click', '.custom-dropdown-item', function (e) {
        e.preventDefault();
        var val = $(this).data('val');
        var text = $(this).data('text');
        var targetId = $(this).data('target');
        var displayId = $(this).data('display');
        
        $('#' + targetId).val(val).trigger('change');
        $('#' + displayId).text(text);

        // Update active state
        $(this).closest('.dropdown-menu').find('.custom-dropdown-item span.fw-bold').removeClass('fw-bold text-primary');
        $(this).closest('.dropdown-menu').find('.custom-dropdown-item span.badge.bg-primary').removeClass('bg-primary').addClass('bg-light text-dark');
        
        $(this).find('span').first().addClass('fw-bold text-primary');
        $(this).find('span.badge').removeClass('bg-light text-dark').addClass('bg-primary');
    });

    $(document).on('click', '#product-list-container .pagination a.page-link', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        
        // Trích xuất filterString từ form và nối vào url phân trang
        // bởi vì Component Pagination có thể không chứa chuỗi tìm kiếm này
        var filterString = $('input[name="filterString"]').val();
        if (filterString) {
            url += (url.indexOf('?') > -1 ? '&' : '?') + 'filterString=' + encodeURIComponent(filterString);
        }

        loadProducts(url);
    });
});
