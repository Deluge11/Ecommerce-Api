using Models;


namespace Business_Layer.Interfaces;

public interface IOrdersBusiness
{
    Task<List<Order>> GetMyOrders();
    Task<List<Order>> GetOrdersByUserId(int userId);
    Task<Order> GetOrderById(int orderId);
    Task<OperationResult<Order>> CreateOrder();
    Task<List<OrderDetails>> GetOrderDetails(int orderId);
    Task<bool> CreateStoreOrder(int orderId);
    Task<bool> ConfrimOrderInStore(int orderId);
}