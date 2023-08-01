namespace CoffeeShopMVC.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();

        public double TotalSpent()
        {
            double total = 0;
            foreach(var item in AllItems())
            {
                total += item.PriceInDollars();
            }
            return Math.Round(total, 2);
        }

        public List<Item> AllItems()
        {
            var items = new List<Item>();
            foreach(var order in Orders)
            {
                items.AddRange(order.Items);
            }
            return items;
        }
    }
}
