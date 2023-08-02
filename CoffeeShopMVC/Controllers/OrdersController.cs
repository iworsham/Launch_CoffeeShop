using CoffeeShopMVC.DataAccess;
using Microsoft.AspNetCore.Mvc;
using CoffeeShopMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopMVC.Controllers
{
	
	public class OrdersController : Controller
	{
		private readonly CoffeeShopMVCContext _context;

		public OrdersController(CoffeeShopMVCContext context)
		{
			_context = context;
		}
		[Route("/customers/{customerId:int}/orders")]
		public IActionResult Index(int customerId)
		{
			var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).ThenInclude(o => o.Items).FirstOrDefault();
			return View(customer);
		}

		[Route("/customers/{customerId:int}/orders/{orderId:int}")]
		public IActionResult Show(int orderId)
		{
			var order = _context.Orders
				.Where(o => o.Id == orderId)
				.Include(o => o.Items)
				.FirstOrDefault();

			return View(order);
		}
	}
}
