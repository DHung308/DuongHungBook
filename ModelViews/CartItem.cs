using DHungBooks.Models;

namespace DHungBooks.ModelViews
{
    public class CartItem
    {
        public Book product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * product.Price.Value;
    }
}
