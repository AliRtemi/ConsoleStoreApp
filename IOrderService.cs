using System.Collections.Generic;

namespace ConsoleStoreApp
{
    public interface IOrderService
    {
        Order CreateOrder(int customerId, List<(int productId, int quantity)> itemsToBuy, string? promoCode);
        IEnumerable<Order> GetAllOrders();
    }
}