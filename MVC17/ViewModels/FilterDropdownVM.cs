namespace MVC17.ViewModels
{
    public class DropdownItemVM
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public int? Count { get; set; }
    }

    public class FilterDropdownVM
    {
        public string Name { get; set; }
        public string IconClass { get; set; }
        public string DefaultText { get; set; }
        public string CurrentValue { get; set; }
        public List<DropdownItemVM> Items { get; set; }
        public bool ShowBadge { get; set; }
    }
}
