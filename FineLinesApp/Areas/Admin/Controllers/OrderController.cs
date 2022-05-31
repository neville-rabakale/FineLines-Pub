using FineLines.DataAccess.Repositories;
using FineLines.DataAccess.Repositories.IRepositories;
using FineLines.Models;
using FineLines.Models.ViewModels;
using FineLines.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
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
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateToProcessing()
        {
            //Change order status to processing
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Staus Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderId= OrderVM.OrderHeader.Id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            //We want to update the OrderHeader values in db with the values in the Current orderHeader values in the VM
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.ShippingDate = DateTime.Now;

            //update and save
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Staus Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancellOrder()
        {
            //We want to update the OrderHeader values in db with the values in the Current orderHeader values in the VM
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                //Here we need to use the stripe protocol to do a refund
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    //We use Payment intent instead of charge
                    PaymentIntent = orderHeader.PaymentIntentId,
                    //You can either choose an amount to be payed here or leave out and full amount will be payed
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                //Update order and payment status
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id,SD.StatusCancelled, SD.StatusRefunded);

            }
            else
            {
                //Update status to cancelled
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            // save changes
            _unitOfWork.Save();
            TempData["Success"] = "Order cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
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
