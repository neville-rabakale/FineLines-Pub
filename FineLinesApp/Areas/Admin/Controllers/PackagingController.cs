using FineLines.DataAccess;
using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using FineLines.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FineLinesApp.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PackagingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackagingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            //retrieve from unitOfWork as list and pass to the CategotryList object
            IEnumerable<Packaging> objPackagingList = _unitOfWork.Packaging.GetAll();
            return View(objPackagingList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST --Addind items to Packaging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Packaging obj)
        {

            //Validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Packaging.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Packaging created successfully";
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
            var packagingFromDb = _unitOfWork.Packaging.GetFirstOrDefault(u => u.Id == id );
            //check if item from Db has valid Id
            if (id == null)
            {
                return NotFound();
            }

            return View(packagingFromDb);
        }

        //POST --Addind items to Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Packaging obj)
        {

            //Validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Packaging.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "CoverType edited successfully";

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

            var packagingFromDb = _unitOfWork.Packaging.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id
            if (id == null)
            {
                return NotFound();
            }

            return View(packagingFromDb);
        }

        //DELETE --Remocing items to Categories
        //You can explicitly give action name by using actionName("String")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int?id)
        {
            var obj = _unitOfWork.Packaging.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Packaging.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfully";
            //this takes you back to the index.
            //-- If you want to redirect to action in another controller,
            //you can just add controller name as second variable
            return RedirectToAction("Index");

        }
    }
}
