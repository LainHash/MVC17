$(document).ready(function () {
    const formatCurrency = (value) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    };

    // Load initial data
    loadTotalOrdersAllTime();
    loadAvailableYears();
    loadStatusStats();
    loadOrdersToday();

    function loadTotalOrdersAllTime() {
        $.get('/Statistic/GetTotalOrdersAllTime', function (res) {
            if (res.success) {
                $('#totalOrdersAllTime').text(res.totalOrders);
                $('#totalQtyAllTime').text(res.totalQuantity + ' sản phẩm');
                $('#totalRevenuOrders').text(formatCurrency(res.totalRevenue));
            }
        });
    }

    function loadAvailableYears() {
        $.get('/Statistic/GetAvailableYears', function (res) {
            const currentYear = new Date().getFullYear();
            let years = [currentYear];

            if (res.success && res.years && res.years.length > 0) {
                years = [...new Set([currentYear, ...res.years])].sort((a, b) => b - a);
            }

            const yearSelects = ['#yearSelect1', '#yearSelect2'];
            
            yearSelects.forEach(selector => {
                const select = $(selector);
                if (select.length) {
                    select.empty();
                    years.forEach(year => {
                        select.append(`<option value="${year}" ${year === currentYear ? 'selected' : ''}>Năm ${year}</option>`);
                    });
                }
            });

            const initialYear = years.includes(currentYear) ? currentYear : years[0];
            loadOrdersByYear(initialYear);
            loadOrdersByMonth(initialYear, new Date().getMonth() + 1);

            $('#yearSelect1').off('change').on('change', function() { loadOrdersByYear($(this).val()); });
            $('#yearSelect2').off('change').on('change', function() { loadOrdersByMonth($(this).val(), $('#monthSelect').val()); });
            $('#monthSelect').off('change').on('change', function() { loadOrdersByMonth($('#yearSelect2').val(), $(this).val()); });
        }).fail(function() {
            const currentYear = new Date().getFullYear();
            const yearSelects = ['#yearSelect1', '#yearSelect2'];
            yearSelects.forEach(selector => {
                const select = $(selector);
                if (select.length) {
                    select.empty();
                    select.append(`<option value="${currentYear}" selected>Năm ${currentYear}</option>`);
                }
            });
        });

        const months = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
        const currentMonth = new Date().getMonth() + 1;
        const monthSelect = $('#monthSelect');
        if (monthSelect.length) {
            monthSelect.empty();
            months.forEach((m, i) => {
                monthSelect.append(`<option value="${i + 1}" ${i + 1 === currentMonth ? 'selected' : ''}>${m}</option>`);
            });
        }
    }

    function loadOrdersByYear(year) {
        $.get(`/Statistic/GetOrdersByYear?year=${year}`, function (res) {
            if (res.success) {
                $('#totalOrdersYear').text(res.totalOrders);
                $('#totalQtyYear').text(res.totalQuantity + ' sản phẩm');
            }
        });
    }

    function loadOrdersByMonth(year, month) {
        $.get(`/Statistic/GetOrdersByMonth?year=${year}&month=${month}`, function (res) {
            if (res.success) {
                $('#totalOrdersMonth').text(res.totalOrders);
                $('#totalQtyMonth').text(res.totalQuantity + ' sản phẩm');
            }
        });
    }

    function loadOrdersToday() {
        $.get('/Statistic/GetOrdersToday', function (res) {
            if (res.success) {
                $('#totalOrdersToday').text(res.totalOrders);
                $('#totalQtyToday').text(res.totalQuantity + ' sản phẩm');
            }
        });
    }

    function loadStatusStats() {
        $.get('/Statistic/GetCancelledOrders', function (res) {
            if (res.success) {
                $('#cancelledOrders').text(res.totalOrders);
                $('#cancelledQty').text(res.totalQuantity + ' sản phẩm');
            }
        });
        $.get('/Statistic/GetCompletedOrders', function (res) {
            if (res.success) {
                $('#completedOrders').text(res.totalOrders);
                $('#completedQty').text(res.totalQuantity + ' sản phẩm');
            }
        });
        $.get('/Statistic/GetRefundedOrders', function (res) {
            if (res.success) {
                $('#refundedOrders').text(res.totalOrders);
                $('#refundedQty').text(res.totalQuantity + ' sản phẩm');
            }
        });
    }
});
