using CoffeeShopMVC.DataAccess;
using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoffeeShopMVC.FeatureTests;


using System.Net;


namespace CoffeeShopTests
{
	[Collection("Controller Tests")]
	public class CustomersControllerTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public CustomersControllerTests(WebApplicationFactory<Program> factory)
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
		public async void Returns_ListOfCustomers_Index()
		{
			var context = GetDbContext();
			context.Customers.Add(new Customer { Name = "Isiah", Email = "Isiah@email.com" });
			context.SaveChanges();

			var client = _factory.CreateClient();
			var response = await client.GetAsync("/customers");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Isiah", html);
		}
		[Fact]
		public async Task New_ReturnsViewWithForm()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			var response = await client.GetAsync("/customers/new");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Name:", html);
			Assert.Contains("Email:", html);
			Assert.Contains($"<form method=\"post\" action=\"/customers/create\">", html);
		}
		[Fact]
		public async Task EditView_DisplaysForm()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			var customer = new Customer { Name = "Zay", Email = "zay@gmail.com" };
			context.Customers.Add(customer);
			context.SaveChanges();

			var response = await client.GetAsync($"/customers/edit/{customer.Id}");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Zay", html);
			Assert.Contains("zay@gmail.com", html);
			Assert.Contains($"<form method=\"post\" action=\"/customers/update/{customer.Id}\">", html);
		}

		[Fact]
		public async Task Show_ReturnsViewWithCustomerInformation()
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

            var response = await client.GetAsync($"/customers/details/{customer.Id}");
            var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Seth", html);
			Assert.Contains("Chai Latte", html);
			Assert.Contains("Coffee", html);
			Assert.Contains("$10.75", html);
        }

		[Fact]
		public async Task DeleteAction_SuccessfullyWorksWithButton()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			Customer customer = new Customer { Name = "Seth", Email = "seth@aol.com" };
			context.Customers.Add(customer);
			context.SaveChanges();

			var response = await client.PostAsync($"/customers/delete/{customer.Id}", null);
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.DoesNotContain("Seth", html);
			Assert.DoesNotContain("seth@aol.com", html);
			Assert.Contains("<h1>Customers</h1>", html);
		}
	}
}
