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
	[Collection("Customer Controller Tests")]
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
			context.Customers.Add(new Customer { Name = "Isiah", Email = "Isiah@email.com"});
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

	}
}
