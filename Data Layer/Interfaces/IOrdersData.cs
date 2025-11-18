using Models;
namespace Data_Layer.Interfaces;

public interface IOrdersData
{
    Task<List<Order>> GetOrdersByUserId(int userId);
    Task<Order> GetOrderById(int orderId,int userId);
    Task<Order> CreateOrder(int userId);
    Task<List<OrderDetails>> GetOrderDetails(int orderId, int userId);
    Task<List<NewOrderRequest>> GetOrderItemQuantities(int orderId);
}