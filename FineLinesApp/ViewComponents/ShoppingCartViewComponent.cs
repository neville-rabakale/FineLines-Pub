using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FineLinesApp.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //check for signed in user
            if (claim != null)
            {
                //check if session exists
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                //if session doesnt exist check db
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
                return View(HttpContext.Session.GetInt32(SD.SessionCart));

            }
            else
            {
                //If we come here, user has either not signed in or if user signs out 
                HttpContext.Session.Clear();
                return View(0);

            }


        }

    }
}
