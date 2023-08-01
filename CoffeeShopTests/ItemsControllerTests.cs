using CoffeeShopMVC.FeatureTests;
using CoffeeShopMVC.DataAccess;
using CoffeeShopMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace CoffeeShopTests
{
    [Collection("Item Controller Tests")]
    public class ItemsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;

        public ItemsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void Index_ReturnsViewOfListOfItems()
        {
            var context = GetDbContext();
            context.Items.Add(new Item { Name = "Latte", PriceInCents = 600 });
            context.SaveChanges();

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/items");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain("Latte", html);
            
        }
        [Fact]
        public async Task Show_ReturnsView_WithNameAndPrice()
        {
            var context = GetDbContext();
            context.Items.Add(new Item { Name = "Latte", PriceInCents = 600 });
            var coffee = new Item { Name = "Iced Coffee", PriceInCents = 825 };
            context.Items.Add(coffee);
            context.SaveChanges();

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/items/details/{coffee.Id}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Iced Coffee", html);
            Assert.Contains("$8.25", html);
            Assert.DoesNotContain("Latte", html);
        }

        [Fact]
        public async Task DeleteAction_DeletesFromDatabase()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var item = new Item { Name = "Espresso", PriceInCents = 225 };
            context.Items.Add(item);
            context.SaveChanges();

            var response = await client.PostAsync($"/items/delete/{item.Id}", null);
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain("Espresso", html);

        }
        [Fact]
        public async Task New_ReturnsViewWithForm()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/items/new");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Name:", html);
            Assert.Contains("Price:", html);
            Assert.Contains($"<form method=\"post\" action=\"/items/create\">", html);
        }

        [Fact]
        public async Task Edit_ReturnsPopulatedForm()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var item = new Item { Name = "Espresso", PriceInCents = 225 };
            context.Items.Add(item);
            context.SaveChanges();

            var response = await client.GetAsync($"/items/edit/{item.Id}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Espresso", html);
            Assert.Contains("225", html);
            Assert.Contains($"<form method=\"post\" action=\"/items/update/{item.Id}\">", html);
        }

        [Fact]
        public async Task Update_UpdatesItemInDatabase()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var coffee = new Item { Name = "Iced Coffee", PriceInCents = 825 };
            context.Items.Add(coffee);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                { "Name", "Coffee on the Rocks" },
                { "PriceInCents", "799" }
            };

            var response = await client.PostAsync($"/items/update/{coffee.Id}", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Coffee on the Rocks", html);
            Assert.Contains("$7.99", html);
            Assert.DoesNotContain("Iced Coffee", html);
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
    }
}