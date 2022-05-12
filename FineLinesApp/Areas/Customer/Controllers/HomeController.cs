using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using FineLines.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FineLinesApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(objProductList);
        }

        public IActionResult Details( int  productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product  = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
            };
        
            return View(cartObj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //Only logged in users can access post method
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;


            //get obj fron db to be able to increment count
            //Conditions, AppId == claim.Value && ProductId == ShoppingCart.ProductId
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value &&
                u.ProductId == shoppingCart.ProductId);
            
            // if obj from db meeting the conditions doesnt exist == item is new
            if(cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
               //cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);

            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}