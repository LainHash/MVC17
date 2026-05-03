namespace MVC17.Helpers.Constants.Orders
{
    public static class InvoiceStatuses
    {
        public const string pending = "Pending";
        public const string shipping = "Shipping";
        public const string delivered = "Delivered";
        public const string cancelled = "Cancelled";
        public const string completed = "Completed";
        public const string refunded = "Refunded";
        public const string failed = "Failed";
        public const string overdue = "Overdue";
        public const string paid = "Paid";

        public static float RandomShippingDiscount()
        {
            var rd = new Random();
            var values = new float[] { 0.5f, 0.1f, 0.8f};
            return values[rd.Next(values.Length)];
        }
    }
}
