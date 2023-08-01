using CoffeeShopMVC.DataAccess;
using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CoffeeShopMVCContext _context;

        public CustomersController(CoffeeShopMVCContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }
		public IActionResult New()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Customer customer)
		{

			_context.Customers.Add(customer);
			_context.SaveChanges();
		//	var newCustomerId = customer.Id;
			return RedirectToAction("index");
		}


		[Route("/customers/details/{customerId:int}")]
		public IActionResult Show(int customerId)
		{
			var customer = _context.Customers
				.Where(c => c.Id == customerId)
				.Include(c => c.Orders)
				.ThenInclude(o => o.Items)
				.First();

			return View(customer);

		[Route("/customers/edit/{customerId:int}")]
		public IActionResult Edit(int customerId)
		{
			var customer = _context.Customers.Find(customerId);
			return View(customer);
		}

		[HttpPost]
		public IActionResult Update(Customer customer, int id)
		{
			customer.Id = id;
			_context.Customers.Update(customer);
			_context.SaveChanges();

			return Redirect($"/customers/details/{customer.Id}");

		}
	}
}
