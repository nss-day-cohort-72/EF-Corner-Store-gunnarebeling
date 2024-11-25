
namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public Cashier Cashier {get; set;}
    public DateTime? PaidOnDate { get; set; }
    public List<OrderProduct> OrderProducts { get; set; }
    public decimal Total {
        get
        {
            return OrderProducts.Aggregate(0m, (total, op) => {
                decimal itemTotal = op.Quantity * op.Product.Price;
                total += itemTotal;
                return total;
            });
        }
    }

}