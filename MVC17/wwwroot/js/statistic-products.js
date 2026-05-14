$(document).ready(function () {
    const formatCurrency = (value) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    };

    let topSellingChart, topRevenueChart, lowSellingChart;

    // Load initial data
    loadTotalProducts();
    loadAvailableYears();
    loadLowStockProducts();
    
    // Initial charts (all time)
    updateProductCharts(null, null);

    function loadTotalProducts() {
        $.get('/Statistic/GetTotalProductsAllTime', function (res) {
            if (res.success) {
                $('#totalProducts').text(res.totalProducts);
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

            const yearSelect = $('#yearSelect');
            if (yearSelect.length) {
                yearSelect.empty();
                yearSelect.append('<option value="">Tất cả các năm</option>');
                years.forEach(year => {
                    yearSelect.append(`<option value="${year}" ${year === currentYear ? 'selected' : ''}>Năm ${year}</option>`);
                });
                
                // Set current month
                $('#monthSelect').val(new Date().getMonth() + 1);

                yearSelect.off('change').on('change', function() {
                    updateProductCharts($(this).val(), $('#monthSelect').val());
                });
                $('#monthSelect').off('change').on('change', function() {
                    updateProductCharts($('#yearSelect').val(), $(this).val());
                });
            }

            const initialYear = years.includes(currentYear) ? currentYear : (years.length > 0 ? years[0] : null);
            updateProductCharts(initialYear, null);
        }).fail(function() {
            const currentYear = new Date().getFullYear();
            const yearSelect = $('#yearSelect');
            if (yearSelect.length) {
                yearSelect.empty();
                yearSelect.append('<option value="">Tất cả các năm</option>');
                yearSelect.append(`<option value="${currentYear}" selected>Năm ${currentYear}</option>`);
            }
        });
    }

    function loadLowStockProducts() {
        $.get('/Statistic/GetLowStockProducts', function (res) {
            if (res.success) {
                const tbody = $('#lowStockTable');
                tbody.empty();
                if (res.data.length === 0) {
                    tbody.append('<tr><td colspan="3" class="text-center">Không có sản phẩm nào sắp hết hàng</td></tr>');
                    return;
                }
                res.data.forEach(item => {
                    const statusClass = item.unitsInStock <= 5 ? 'text-danger fw-bold' : 'text-warning';
                    tbody.append(`
                        <tr>
                            <td>${item.productName}</td>
                            <td class="text-center">${item.unitsInStock}</td>
                            <td class="text-center"><span class="${statusClass}">${item.unitsInStock <= 5 ? 'Cực thấp' : 'Sắp hết'}</span></td>
                        </tr>
                    `);
                });
            }
        });
    }

    function updateProductCharts(year, month) {
        // Top Selling (Quantity) - Note: Controller doesn't have year/month for GetTopSellingProducts, but has for GetTopRevenueProducts
        // I'll use GetTopRevenueProducts for both but show different metrics if possible, 
        // or just accept that TopSelling is all time as per controller current implementation.
        
        $.get('/Statistic/GetTopSellingProducts', function (res) {
            if (res.success) {
                if (topSellingChart) topSellingChart.destroy();
                topSellingChart = renderBarChart('topSellingChart', res.data.map(x => x.name), res.data.map(x => x.totalQuantity), 'Số lượng bán');
            }
        });

        const params = {};
        if (year) params.year = year;
        if (month) params.month = month;

        $.get('/Statistic/GetTopRevenueProducts', params, function (res) {
            if (res.success) {
                if (topRevenueChart) topRevenueChart.destroy();
                topRevenueChart = renderBarChart('topRevenueChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh thu (₫)', true);
            }
        });

        $.get('/Statistic/GetLowSellingProducts', params, function (res) {
            if (res.success) {
                if (lowSellingChart) lowSellingChart.destroy();
                lowSellingChart = renderBarChart('lowSellingChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh thu (₫)', true);
            }
        });
    }

    function renderBarChart(canvasId, labels, data, label, isCurrency = false) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: data,
                    backgroundColor: 'rgba(230, 57, 70, 0.7)',
                    borderColor: 'rgba(230, 57, 70, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                let val = context.raw;
                                if (isCurrency) return formatCurrency(val);
                                return val;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                if (isCurrency) {
                                    if (value >= 1000000) return (value / 1000000) + 'M';
                                    if (value >= 1000) return (value / 1000) + 'K';
                                }
                                return value;
                            }
                        }
                    }
                }
            }
        });
    }
});
