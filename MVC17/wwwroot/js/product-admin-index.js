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
            error: function () {
                alert('Có lỗi xảy ra khi tải dữ liệu.');
            }
        });
    }

    var debounceTimer;
    $('#filter-form input, #filter-form select').on('change keyup', function (e) {
        var isCategoryChange = e.target.id === 'categoryId';
        
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
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
        }, 500);
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

    $(document).on('click', '.pagination a', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        if (url) {
            loadProducts(url);
        }
    });

    // Modal handling
    $(document).on('click', '.btn-action-modal', function(e) {
        e.preventDefault();
        var url = $(this).attr('href');
        var title = $(this).data('title');
        
        $('#productModalLabel').text(title);
        $('#productModalContent').html('<div class="text-center py-4"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        
        var modalEl = document.getElementById('productModal');
        var myModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
        myModal.show();

        $.ajax({
            url: url,
            type: 'GET',
            headers: { "X-Requested-With": "XMLHttpRequest" },
            success: function(res) {
                $('#productModalContent').html(res);
                
                // Parse validation scripts for newly loaded form
                if ($.validator && $.validator.unobtrusive) {
                    var form = $('#productModalContent').find('form');
                    if(form.length > 0) {
                        $.validator.unobtrusive.parse(form);
                    }
                }
            },
            error: function() {
                $('#productModalContent').html('<div class="alert alert-danger">Có lỗi xảy ra khi tải dữ liệu.</div>');
            }
        });
    });

    // Handle form submission inside modal
    $(document).on('submit', '#productModalContent form', function(e) {
        // If it's the category selector form in Create, let it submit normally or handle via AJAX
        if($(this).find('.category-selector').length > 0 && e.originalEvent && e.originalEvent.submitter == null) {
            // It's probably the onchange event of the category selector
            e.preventDefault();
            var url = $(this).attr('action') + '?' + $(this).serialize();
            $.ajax({
                url: url,
                type: 'GET',
                headers: { "X-Requested-With": "XMLHttpRequest" },
                success: function(res) {
                    $('#productModalContent').html(res);
                }
            });
            return false;
        }

        e.preventDefault();
        var form = $(this);
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method') || 'POST',
            data: form.serialize(),
            headers: { "X-Requested-With": "XMLHttpRequest" },
            success: function(res) {
                if (res.success) {
                    var modalEl = document.getElementById('productModal');
                    var myModal = bootstrap.Modal.getInstance(modalEl);
                    if (myModal) {
                        myModal.hide();
                    } else {
                        $('#productModal').modal('hide');
                    }
                    
                    // Fallback to clear backdrop if stuck
                    setTimeout(function() {
                        $('.modal-backdrop').remove();
                        $('body').removeClass('modal-open').css('padding-right', '');
                    }, 300);
                    
                    // Reload list with current filters
                    var currentUrl = $('#filter-form').attr('action') + '?' + $('#filter-form').serialize();
                    var activePageUrl = $('.pagination .active a').attr('href');
                    loadProducts(activePageUrl || currentUrl);
                } else {
                    // Replace content with validation errors (the partial view HTML)
                    $('#productModalContent').html(res);
                    if ($.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse('#productModalContent form');
                    }
                }
            },
            error: function() {
                alert('Có lỗi xảy ra khi lưu dữ liệu.');
            }
        });
    });
});
