namespace MVC17.Helpers.Constants.Orders
{
    public static class InvoiceStatuses
    {
        public const string Pending = "Pending";
        public const string Shipping = "Shipping";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";
        public const string Refunded = "Refunded";
        public const string Failed = "Failed";
        public const string Overdue = "Overdue";
        public const string Paid = "Paid";

        /// <summary>
        /// Key   : Mã trạng thái tiếng Anh
        /// Value :
        ///   Item1 = Tên hiển thị tiếng Việt
        ///   Item2 = Bootstrap background class (bg-*)
        /// </summary>
        public static readonly Dictionary<string, (string ViName, string BgClass)> StatusInfo
            = new()
            {
            { Pending,   ("Đang xử lý",       "bg-warning") },
            { Shipping,  ("Đang giao hàng",   "bg-primary") },
            { Delivered, ("Đã giao hàng",     "bg-info") },
            { Completed, ("Hoàn thành",       "bg-success") },
            { Cancelled, ("Đã hủy",           "bg-secondary") },
            { Refunded,  ("Đã hoàn tiền",     "bg-dark") },
            { Failed,    ("Thất bại",         "bg-danger") },
            { Overdue,   ("Quá hạn",          "bg-danger") },
            { Paid,      ("Đã thanh toán",    "bg-success") }
            };

        /// <summary>
        /// Chuẩn hóa chuỗi:
        /// - trim khoảng trắng
        /// - lowercase toàn bộ
        /// - viết hoa chữ cái đầu
        /// Ví dụ:
        /// "pending" => "Pending"
        /// " SHIPPING " => "Shipping"
        /// </summary>
        private static string Normalize(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return string.Empty;

            status = status.Trim().ToLowerInvariant();
            return char.ToUpperInvariant(status[0]) + status.Substring(1);
        }

        /// <summary>
        /// Lấy tên trạng thái tiếng Việt.
        /// </summary>
        public static string GetStatusVi(string? status)
        {
            status = Normalize(status);

            if (StatusInfo.TryGetValue(status, out var info))
                return info.ViName;

            return status;
        }

        /// <summary>
        /// Lấy Bootstrap background class.
        /// </summary>
        public static string GetStatusStyle(string? status)
        {
            status = Normalize(status);

            if (StatusInfo.TryGetValue(status, out var info))
                return info.BgClass;

            // Mặc định nếu không tìm thấy
            return "bg-secondary";
        }

        /// <summary>
        /// Lấy đồng thời cả tên tiếng Việt và CSS class.
        /// </summary>
        public static (string ViName, string BgClass) GetStatusInfo(string? status)
        {
            status = Normalize(status);

            if (StatusInfo.TryGetValue(status, out var info))
                return info;

            return (status, "bg-secondary");
        }
    }
}
