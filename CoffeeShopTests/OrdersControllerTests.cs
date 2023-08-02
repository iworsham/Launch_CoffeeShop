using CoffeeShopMVC.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using CoffeeShopMVC.FeatureTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeShopMVC.Models;

namespace CoffeeShopTests
{
	[Collection("Controller Tests")]
	public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public OrdersControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}
		private CoffeeShopMVCContext GetDbContext()
		{
			var optionsBuilder = new DbContextOptionsBuilder<CoffeeShopMVCContext>();
			optionsBuilder.UseInMemoryDatabase("TestDatabase");

			var context = new CoffeeShopMVCContext(optionsBuilder.Options);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();

			return context;
		}
		[Fact]
		public async Task Index_ReturnsViewOfAll_Orders()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();


			Customer customer = new Customer { Name = "Seth", Email = "seth@aol.com" };
			Order order1 = new Order { DateCreated = DateTime.Now.ToUniversalTime() };
			Order order2 = new Order { DateCreated = DateTime.Now.ToUniversalTime() };
			customer.Orders.Add(order1);
			customer.Orders.Add(order2);

			context.Customers.Add(customer);
			context.SaveChanges();
			var response = await client.GetAsync($"/customers/{customer.Id}/orders");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains($"Order {order1.Id}", html);
			Assert.Contains($"Order {order2.Id}", html);
		}

		[Fact]
		public async Task Show_DisplaysViewOfSingleCustomerOrder()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			Customer customer = new Customer { Name = "Seth", Email = "seth@aol.com" };
			Order order1 = new Order { DateCreated = DateTime.Now.ToUniversalTime() };
			Order order2 = new Order { DateCreated = DateTime.Now.ToUniversalTime() };
			Item item1 = new Item { Name = "Chai Latte", PriceInCents = 450 };
			Item item2 = new Item { Name = "Scone", PriceInCents = 300 };
			Item item3 = new Item { Name = "Coffee", PriceInCents = 325 };

			order1.Items.Add(item1);
			order1.Items.Add(item2);
			order2.Items.Add(item3);
			customer.Orders.Add(order1);
			customer.Orders.Add(order2);

			context.Customers.Add(customer);
			context.SaveChanges();

			var response = await client.GetAsync($"/customers/{customer.Id}/orders/{order1.Id}");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Chai Latte", html);
			Assert.Contains("Scone", html);
			Assert.Contains("1 Chai", html);
			Assert.Contains("2 Scone", html);
			Assert.Contains($"Order {order1.Id}", html);
		}
	}
}
