using Presentation_Layer.Authorization;
using Enums;
using Business_Layer.Interfaces;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    public ILogger<OrdersController> logger { get; }
    public IOrdersBusiness OrdersBusiness { get; }
    public IUsersBusiness UsersBusiness { get; }

    public OrdersController
        (
        ILogger<OrdersController> _logger,
        IOrdersBusiness ordersBusiness,
        IUsersBusiness usersBusiness
        )
    {
        logger = _logger;
        OrdersBusiness = ordersBusiness;
        UsersBusiness = usersBusiness;
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetMyOrders()
    {
        var orders = await OrdersBusiness.GetOrdersByUserId(UsersBusiness.GetUserId());
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<List<OrderDetails>>> GetMyOrderDetails(int orderId)
    {
        var orders = await OrdersBusiness.GetOrderDetails(orderId);
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewUserOrders)]
    [HttpGet("UserOrder/{id}")]
    public async Task<ActionResult<List<Order>>> GetOrdersByUserId(int id)
    {
        var orders = await OrdersBusiness.GetOrdersByUserId(id);
        return orders != null ?
            Ok(orders) : NotFound();
    }


}
