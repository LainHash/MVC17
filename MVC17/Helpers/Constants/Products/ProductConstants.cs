namespace MVC17.Helpers.Constants.Products
{
    public static class ProductConstants
    {
        public const int ProductsPerPage = 15; 
        public static readonly Dictionary<int, string> sortDict = new Dictionary<int, string>()
        {
                { 0, "Mới nhất" },
                { 1, "Cũ nhất" },
                { 2, "Giá: Thấp đến Cao" },
                { 3, "Giá: Cao đến Thấp" }
        };
    }
}
