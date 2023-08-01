namespace CoffeeShopMVC.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PriceInCents { get; set; }
        public Order? Order { get; set; }
        public double PriceInDollars()
        {
            double priceInDollars = (double)PriceInCents / 100; 

            return Math.Round(priceInDollars,2);
        }
    
    
    
    }





}
