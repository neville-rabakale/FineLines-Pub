using FineLines.DataAccess.Repositories;
using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using FineLines.Models.ViewModels;
using FineLines.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Claims;

namespace FineLinesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details( int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u=>u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderId == orderId  ,includeProperties: "Product")
            };
            
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail()
        {
            //We want to update the OrderHeader values in db with the values in the Current orderHeader values in the VM
            var orderHeaderfromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderfromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderfromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderfromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderfromDb.City = OrderVM.OrderHeader.City;
            orderHeaderfromDb.County = OrderVM.OrderHeader.County;
            orderHeaderfromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            //allow change of carrier
            if(OrderVM.OrderHeader.Carrier != null)
            {
                orderHeaderfromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            //allow change of Tracking Number
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderfromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            //update and save
            _unitOfWork.OrderHeader.Update(orderHeaderfromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", "Order", new {orderId = orderHeaderfromDb.Id});
        }
        #region API CALLS
        [HttpGet]
        //call to retrieve datatable/all items in database-
        //ret json because it is for the "datatables.net datatable
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u=> u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }




            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
