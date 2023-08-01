using CoffeeShopMVC.DataAccess;
using Microsoft.AspNetCore.Mvc;
using CoffeeShopMVC.Models;

namespace CoffeeShopMVC.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CoffeeShopMVCContext _context;

        public ItemsController(CoffeeShopMVCContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Items.ToList();
            return View(items);
        }

        [Route("/Items/details/{itemId:int}")]
        public IActionResult Show(int itemId)
        {
            var item = _context.Items.Find(itemId);
            return View(item);
        }

        [HttpPost]
        public IActionResult Delete(int Id)
        {
            var item = _context.Items.Find(Id);
            _context.Items.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult New()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Item item)
        {
            
            _context.Items.Add(item);
            _context.SaveChanges();
            var newItemId = item.Id;
            return RedirectToAction("index");
        }

        [Route("/items/edit/{itemId:int}")]
        public IActionResult Edit(int itemId)
        {
            var item = _context.Items.Find(itemId);
            return View(item);
        }

        [HttpPost]
        public IActionResult Update(Item item, int id)
        {
            item.Id = id;
            _context.Items.Update(item);
            _context.SaveChanges();

            return Redirect($"/items/details/{item.Id}");
        }
    }
}
