using FineLinesApp.Data;
using FineLinesApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FineLinesApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            //retrieve from db as list and pass to the CategotryList object
            IEnumerable<Category> objCategoryList = _db.Categories;
            return View(objCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST --Addind items to Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayOrder and Name cannot match ");
            }

            //Validation
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                //this takes you back to the index.
                //-- If you want to redirect to action in another controller,
                //you can just add controller name as second variable
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            //check for invalid id
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryFromDb = _db.Categories.Find(id);
            //check if item from Db has valid Id
            if (id == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        //POST --Addind items to Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {

            //check if item from Db has valid Id
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayOrder and Name cannot match ");
            }

            //Validation
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                //this takes you back to the index.
                //-- If you want to redirect to action in another controller,
                //you can just add controller name as second variable
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
