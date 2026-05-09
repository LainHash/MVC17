namespace MVC17.Helpers.Constants.Products
{
    public static class CategoryConstants
    {
        public const int Laptop = 1;
        public const int CPU = 2;
        public const int GPU = 3;
        public const int Storage = 4;
        public const int RAM = 5;

        public static readonly Dictionary<string, string> CategoryTranslations = new Dictionary<string, string>()
        {
            {"Laptop", "Laptop" },
            {"Cpu", "CPU" },
            {"Gpu", "GPU" },
            {"Ram", "RAM" },
            {"Memory", "Bộ nhớ" }
        };
    }
}
