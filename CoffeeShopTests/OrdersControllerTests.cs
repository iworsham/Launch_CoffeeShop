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

	}
}
