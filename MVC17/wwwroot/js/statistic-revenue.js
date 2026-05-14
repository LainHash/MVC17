$(document).ready(function () {
    // Utility for currency formatting
    const formatCurrency = (value) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    };

    // Initialize Charts
    let revenueByMonthChart, revenueByYearChart;
    let categoryAllTimeChart, categoryYearChart, categoryMonthChart;
    let supplierAllTimeChart, supplierYearChart, supplierMonthChart;

    // Load initial data
    loadTotalRevenueAllTime();
    loadAvailableYears();
    loadRevenueByYearChart();
    loadRevenueByCategoryCharts();
    loadRevenueBySupplierCharts();

    // 1. Total Revenue All Time
    function loadTotalRevenueAllTime() {
        $.get('/Statistic/GetTotalRevenueAllTime', function (res) {
            if (res.success) {
                $('#totalRevenueAllTime').text(formatCurrency(res.totalRevenue));
                $('#totalOrdersAllTime').text(res.totalOrders + ' đơn hàng');
            }
        });
    }

    // 2. Load Years for dropdowns
    function loadAvailableYears() {
        $.get('/Statistic/GetAvailableYears', function (res) {
            const currentYear = new Date().getFullYear();
            let years = [currentYear];

            if (res.success && res.years && res.years.length > 0) {
                years = [...new Set([currentYear, ...res.years])].sort((a, b) => b - a);
            }
            
            const yearSelects = ['#yearSelect1', '#yearSelect2', '#yearSelectChart1', '#yearSelectChart2', '#yearSelectChart3', '#yearSelectChart4', '#yearSelectChart5'];
            
            yearSelects.forEach(selector => {
                const select = $(selector);
                if (select.length) {
                    select.empty();
                    years.forEach(year => {
                        select.append(`<option value="${year}" ${year === currentYear ? 'selected' : ''}>Năm ${year}</option>`);
                    });
                }
            });

            // Load initial stats based on current year or first available year
            const initialYear = years.includes(currentYear) ? currentYear : years[0];
            loadRevenueByYear(initialYear);
            loadRevenueByMonth(initialYear, new Date().getMonth() + 1);
            
            // Set up event listeners for year selection
            $('#yearSelect1').off('change').on('change', function() { loadRevenueByYear($(this).val()); });
            $('#yearSelect2').off('change').on('change', function() { loadRevenueByMonth($(this).val(), $('#monthSelect').val()); });
            $('#monthSelect').off('change').on('change', function() { loadRevenueByMonth($('#yearSelect2').val(), $(this).val()); });
            
            // Chart specific listeners
            $('#yearSelectChart1').off('change').on('change', function() { updateRevenueByMonthChart($(this).val()); });
            $('#yearSelectChart2').off('change').on('change', function() { updateCategoryYearChart($(this).val()); });
            $('#yearSelectChart3').off('change').on('change', function() { updateCategoryMonthChart($(this).val(), $('#monthSelectChart').val()); });
            $('#monthSelectChart').off('change').on('change', function() { updateCategoryMonthChart($('#yearSelectChart3').val(), $(this).val()); });
            $('#yearSelectChart4').off('change').on('change', function() { updateSupplierYearChart($(this).val()); });
            $('#yearSelectChart5').off('change').on('change', function() { updateSupplierMonthChart($(this).val(), $('#monthSelectChart2').val()); });
            $('#monthSelectChart2').off('change').on('change', function() { updateSupplierMonthChart($('#yearSelectChart5').val(), $(this).val()); });

            updateRevenueByMonthChart(initialYear);
            updateCategoryYearChart(initialYear);
            updateSupplierYearChart(initialYear);
        }).fail(function() {
            console.error("Failed to load available years. Using current year as fallback.");
            const currentYear = new Date().getFullYear();
            const yearSelects = ['#yearSelect1', '#yearSelect2', '#yearSelectChart1', '#yearSelectChart2', '#yearSelectChart3', '#yearSelectChart4', '#yearSelectChart5'];
            yearSelects.forEach(selector => {
                const select = $(selector);
                if (select.length) {
                    select.empty();
                    select.append(`<option value="${currentYear}" selected>Năm ${currentYear}</option>`);
                }
            });
        });

        // Initialize months dropdowns
        const months = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
        const monthSelects = ['#monthSelect', '#monthSelectChart', '#monthSelectChart2'];
        const currentMonth = new Date().getMonth() + 1;

        monthSelects.forEach(selector => {
            const select = $(selector);
            if (select.length) {
                select.empty();
                months.forEach((m, i) => {
                    select.append(`<option value="${i + 1}" ${i + 1 === currentMonth ? 'selected' : ''}>${m}</option>`);
                });
            }
        });
    }

    function loadRevenueByYear(year) {
        $.get(`/Statistic/GetRevenueByYear?year=${year}`, function (res) {
            if (res.success) {
                let total = 0;
                let qty = 0;
                res.data.forEach(item => {
                    total += item.totalRevenue;
                    qty += item.totalQuantity;
                });
                $('#totalRevenueYear').text(formatCurrency(total));
                $('#totalOrdersYear').text(qty + ' sản phẩm');
            }
        });
    }

    function loadRevenueByMonth(year, month) {
        $.get(`/Statistic/GetRevenueByMonth?year=${year}&month=${month}`, function (res) {
            if (res.success) {
                let total = 0;
                let qty = 0;
                res.data.forEach(item => {
                    total += item.totalRevenue;
                    qty += item.totalQuantity;
                });
                $('#totalRevenueMonth').text(formatCurrency(total));
                $('#totalOrdersMonth').text(qty + ' sản phẩm');
            }
        });
    }

    // --- Charts Logic ---

    function loadRevenueByYearChart() {
        $.get('/Statistic/GetAvailableYears', function (res) {
            if (res.success) {
                const years = res.years.sort();
                const promises = years.map(year => $.get(`/Statistic/GetRevenueByYear?year=${year}`));
                
                Promise.all(promises).then(results => {
                    const data = results.map(r => {
                        let sum = 0;
                        r.data.forEach(item => sum += item.totalRevenue);
                        return sum;
                    });

                    const ctx = document.getElementById('revenueByYearChart').getContext('2d');
                    revenueByYearChart = new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: years.map(y => 'Năm ' + y),
                            datasets: [{
                                label: 'Doanh Thu',
                                data: data,
                                backgroundColor: 'rgba(230, 57, 70, 0.7)',
                                borderColor: 'rgba(230, 57, 70, 1)',
                                borderWidth: 1
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                title: { display: true, text: 'Doanh Thu Theo Năm' }
                            }
                        }
                    });

                    updateRevenueByMonthChart(new Date().getFullYear());
                });
            }
        });
    }

    function updateRevenueByMonthChart(year) {
        $.get(`/Statistic/GetRevenueByYear?year=${year}`, function (res) {
            if (res.success) {
                const labels = ["T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12"];
                const data = Array(12).fill(0);
                res.data.forEach(item => {
                    data[item.month - 1] = item.totalRevenue;
                });

                const ctx = document.getElementById('revenueByMonthChart').getContext('2d');
                if (revenueByMonthChart) revenueByMonthChart.destroy();
                
                revenueByMonthChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Doanh Thu',
                            data: data,
                            borderColor: '#e63946',
                            backgroundColor: 'rgba(230, 57, 70, 0.1)',
                            fill: true,
                            tension: 0.4
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            title: { display: true, text: 'Doanh Thu Theo Tháng (Năm ' + year + ')' }
                        }
                    }
                });
            }
        });
    }

    function loadRevenueByCategoryCharts() {
        // All Time
        $.get('/Statistic/GetRevenueByCategory', function(res) {
            if(res.success) {
                renderBarChart('categoryAllTimeChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
        // Initial Year
        updateCategoryYearChart(new Date().getFullYear());
        // Initial Month
        updateCategoryMonthChart(new Date().getFullYear(), new Date().getMonth() + 1);
    }

    function updateCategoryYearChart(year) {
        $.get(`/Statistic/GetRevenueByYearCategory?year=${year}`, function(res) {
            if(res.success) {
                if (categoryYearChart) categoryYearChart.destroy();
                categoryYearChart = renderBarChart('categoryYearChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
    }

    function updateCategoryMonthChart(year, month) {
        $.get(`/Statistic/GetRevenueByMonthCategory?year=${year}&month=${month}`, function(res) {
            if(res.success) {
                if (categoryMonthChart) categoryMonthChart.destroy();
                categoryMonthChart = renderBarChart('categoryMonthChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
    }

    function loadRevenueBySupplierCharts() {
        $.get('/Statistic/GetRevenueBySupplier', function(res) {
            if(res.success) {
                renderBarChart('supplierAllTimeChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
        updateSupplierYearChart(new Date().getFullYear());
        updateSupplierMonthChart(new Date().getFullYear(), new Date().getMonth() + 1);
    }

    function updateSupplierYearChart(year) {
        $.get(`/Statistic/GetRevenueByYearSupplier?year=${year}`, function(res) {
            if(res.success) {
                if (supplierYearChart) supplierYearChart.destroy();
                supplierYearChart = renderBarChart('supplierYearChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
    }

    function updateSupplierMonthChart(year, month) {
        $.get(`/Statistic/GetRevenueByMonthSupplier?year=${year}&month=${month}`, function(res) {
            if(res.success) {
                if (supplierMonthChart) supplierMonthChart.destroy();
                supplierMonthChart = renderBarChart('supplierMonthChart', res.data.map(x => x.name), res.data.map(x => x.totalRevenue), 'Doanh Thu');
            }
        });
    }

    function renderBarChart(canvasId, labels, data, label) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: data,
                    backgroundColor: [
                        'rgba(230, 57, 70, 0.7)', 'rgba(29, 53, 87, 0.7)', 'rgba(69, 123, 157, 0.7)', 
                        'rgba(168, 218, 220, 0.7)', 'rgba(241, 250, 238, 0.7)', 'rgba(255, 159, 28, 0.7)', 
                        'rgba(46, 196, 182, 0.7)', 'rgba(231, 29, 54, 0.7)', 'rgba(1, 22, 39, 0.7)'
                    ],
                    borderColor: [
                        '#e63946', '#1d3557', '#457b9d', '#a8dadc', '#f1faee',
                        '#ff9f1c', '#2ec4b6', '#e71d36', '#011627'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return formatCurrency(context.raw);
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                if (value >= 1000000) return (value / 1000000) + 'M';
                                if (value >= 1000) return (value / 1000) + 'K';
                                return value;
                            }
                        }
                    }
                }
            }
        });
    }
});
