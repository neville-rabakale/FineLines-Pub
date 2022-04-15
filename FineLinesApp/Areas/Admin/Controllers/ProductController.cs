using FineLines.DataAccess;
using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using FineLines.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FineLinesApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;

        }
        public IActionResult Index()
        {
            //retrieve from unitOfWork as list and pass to the CategotryList object
            //IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll();
            return View();
        }

        //GET for Upsert
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                product = new(),
                //For the Dropdowns for Category and CoverType
                CategoryList = _unitOfWork.Category.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
            };

            //Create Product
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            //Update Product
            else
            {
                productVM.product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                if(productVM.product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
            //return View(productVM);


        }

        //POST --Addind items to Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {

            //Validation
            if (ModelState.IsValid)
            {
                //img
                //--Get rootPath
                string wwwRootPath = _hostEnvironment.WebRootPath;
                //file was uploaded/exists
                if (file != null)
                {
                    //generate new fileName - using Guid to avoid errors from possible duplicate names
                    string fileName = Guid.NewGuid().ToString();
                    //path for the file to be uploaded
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    //Getting the fileName of the file
                    var extension = Path.GetExtension(file.FileName);
                    if(obj.product.ImageUrl != null)
                    {
                        //get path for existing image + remove \ as used in db --> see diff between uploads & bj.product.ImageUrl
                        var oldImageUrl = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                        //Check if old image exists at this old path
                        if (System.IO.File.Exists(oldImageUrl))
                        {
                            //If it exists  we should delete
                            System.IO.File.Delete(oldImageUrl);

                        }
                    }

                    //copy uploaded file into the Product folder
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        file.CopyTo(fileStreams);
                    }
                    //what we save in the database
                    obj.product.ImageUrl = @"\images\products\" + fileName + extension;

                }

                //Check if we are adding new product or updating
                if(obj.product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.product);

                }

                _unitOfWork.Save();
                TempData["success"] = "Product Created successfully";

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

            var categoryFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id
            if (id == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        //DELETE --Removing items From Products
        //You can explicitly give action name by using actionName("String")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int?id)
        {
            //to get path for img, we need to use rootpath here too
            string wwwRootPath = _hostEnvironment.WebRootPath;
            //get product from db
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            //check if item from Db has valid Id/exists
            if (obj == null)
            {
                return NotFound();
            }

            //Here we need to first check if an image exists
            //if it does, we need to delete it
            if (obj.ImageUrl != null)
            {
                //image exests
                //get path for existing image + remove \ as used in db --> see diff between uploads & bj.product.ImageUrl
                var oldImageUrl = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                //Check if old image exists at this old path
                if (System.IO.File.Exists(oldImageUrl))
                {
                    //If it exists  we should delete
                    System.IO.File.Delete(oldImageUrl);

                }

            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            //this takes you back to the index.
            //-- If you want to redirect to action in another controller,
            //you can just add controller name as second variable
            return RedirectToAction("Index");

        }

        #region API CALLS
        [HttpGet]
        //call to retrieve datatable/all items in database-
        //ret json because it is for the "datatables.net datatable
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }
        #endregion
    }

}