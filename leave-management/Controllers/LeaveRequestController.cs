using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Models;


namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _repoleaverequest;
        private readonly ILeaveTypeRepository _repoleavetype;
        private readonly ILeaveAllocationRepository _repoleaveallocation;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;


        public LeaveRequestController(ILeaveAllocationRepository repoleaveallocation,ILeaveTypeRepository repoleavetype,ILeaveRequestRepository repoleaverequest,IMapper mapper, UserManager<Employee> userManager)
        {
            _repoleaverequest = repoleaverequest;
            _repoleavetype = repoleavetype;
            _repoleaveallocation = repoleaveallocation;
            _mapper = mapper;
            _userManager = userManager;
        }
        [Authorize(Roles ="Administrator")]
        // GET: LeaveRequest
        public ActionResult Index()
        {
            var leaveRequests = _repoleaverequest.FindAll().Where(x=>x.CancelRequest==false).ToList();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestsModel.Count,
                //ApprovedRequests = leaveRequestsModel.Where(x => x.Approved == true).Count(),
                ApprovedRequests = leaveRequestsModel.Count(x => x.Approved == true),
                PendingRequests = leaveRequestsModel.Where(x => x.Approved == null).Count(),
                RejectedRequests = leaveRequestsModel.Count(x => x.Approved == false),
                LeaveRequests = leaveRequestsModel
            };
            return View(model);
        }

        // GET: LeaveRequest/Details/5
        public ActionResult Details(int id)
        {
            var leaverequest = _repoleaverequest.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaverequest);
            return View(model);
        }
        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaverequest = _repoleaverequest.FindById(id);
                var allocation = _repoleaveallocation.GetLeaveAllocationsByEmployeeAndType(leaverequest.RequestingEmployeeId,leaverequest.LeaveTypeId);
                int daysrequested = (int)(leaverequest.EndDate - leaverequest.StartDate).Days;
                allocation.NumberOfDays = allocation.NumberOfDays - daysrequested;
                leaverequest.Approved = true;
                leaverequest.ApprovedById = user.Id;
                leaverequest.DateActioned = DateTime.Now;
                var isSuccess = _repoleaverequest.Update(leaverequest);
                if (!isSuccess)
                {
                    return RedirectToAction(nameof(Index),"Home");
                }
                _repoleaveallocation.Update(allocation);
                return RedirectToAction(nameof(Details),new { id= id });
            }
            catch
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }
        public ActionResult RejectRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaverequest = _repoleaverequest.FindById(id);
                leaverequest.Approved = false;
                leaverequest.ApprovedById = user.Id;
                leaverequest.DateActioned = DateTime.Now;
                var isSuccess = _repoleaverequest.Update(leaverequest);
                if (!isSuccess)
                {
                    return RedirectToAction(nameof(Index), "Home");
                }
                return RedirectToAction(nameof(Details));
            }
            catch
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }
        // GET: LeaveRequest/Create
        public ActionResult Create()
        {
            var leaveTypes = _repoleavetype.FindAll();
            var leavetypeitems = leaveTypes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }) ;
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leavetypeitems,

            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            var leaveTypes = _repoleavetype.FindAll();
            var leavetypeitems = leaveTypes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            model.LeaveTypes = leavetypeitems;
            try
            {
                var StartDate = Convert.ToDateTime(model.StartDate);
                var EndDate = Convert.ToDateTime(model.EndDate);
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                if(DateTime.Compare(StartDate,EndDate)>1)
                {
                    ModelState.AddModelError("", "Starting Date cannot be greater then End Date...");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _repoleaveallocation.GetLeaveAllocationsByEmployeeAndType(employee.Id,model.LeaveTypeId);
                var daysRequested = (int)(EndDate.Date - StartDate.Date).TotalDays;
                if (daysRequested > allocation.NumberOfDays) 
                {
                    ModelState.AddModelError("", "You do not sufficient days for this Request...");
                    return View(model);
                }
                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId=model.LeaveTypeId,
                    CommentRequest=model.CommentRequest
                };

                var leaverequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = _repoleaverequest.Create(leaverequest);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went Wrong with submitting your record...");
                    return View(model);
                }
                return RedirectToAction(nameof(MyLeave));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong...");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult MyLeave()
        {
            try
            {
                var employee = _userManager.GetUserAsync(User).Result;
                var employeeid = employee.Id;
                var employeeAllocations = _repoleaveallocation.GetLeaveAllocationsByEmployee(employeeid);
                var employeeRequests = _repoleaverequest.GetLeaveRequestsByEmployee(employeeid);
                var employeeAllocationsModel = _mapper.Map<List<LeaveAllocationVM>>(employeeAllocations);
                var employeeRequestsModel = _mapper.Map<List<LeaveRequestVM>>(employeeRequests);
                var model = new EmployeeLeaveRequestVM
                {
                    LeaveAllocations = employeeAllocationsModel,
                    LeaveRequests = employeeRequestsModel
                };
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult CancelRequest(int id)
        {
            try
            {
                var requestcancel = _repoleaverequest.FindById(id);
                requestcancel.CancelRequest = true;
                _repoleaverequest.Update(requestcancel);
            }
            catch
            { }
            return RedirectToAction(nameof(MyLeave));
        }




        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
