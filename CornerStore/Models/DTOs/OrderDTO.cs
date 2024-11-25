using Microsoft.AspNetCore.SignalR;

namespace CornerStore.Models.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public CashierDTO Cashier {get; set;}
    public DateTime? PaidOnDate { get; set; }
    public List<OrderProductDTO> OrderProducts { get; set; }
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
