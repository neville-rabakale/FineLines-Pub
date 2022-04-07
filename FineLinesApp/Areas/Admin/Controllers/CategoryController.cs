using FineLines.DataAccess;
using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using Microsoft.AspNetCore.Mvc;

namespace FineLinesApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            //retrieve from unitOfWork as list and pass to the CategotryList object
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
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
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
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

  //          var categoryFromDb = _unitOfWork.Categories.Find(id);
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id );
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
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully";

                //this takes you back to the index.
                //-- If you want to redirect to action in another controller,
                //you can just add controller name as second variable
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            //check for invalid id
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id
            if (id == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        //DELETE --Remocing items to Categories
        //You can explicitly give action name by using actionName("String")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int?id)
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            //this takes you back to the index.
            //-- If you want to redirect to action in another controller,
            //you can just add controller name as second variable
            return RedirectToAction("Index");

        }
    }
}
