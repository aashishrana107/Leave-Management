using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveAllocationRepository _repoleaveAllocation;
        private readonly ILeaveTypeRepository _repoleaveTypes;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        
        public LeaveAllocationController(ILeaveAllocationRepository repoleaveAllocation, ILeaveTypeRepository repoleaveTypes, IMapper mapper, UserManager<Employee> userManager)
        {
            _repoleaveAllocation = repoleaveAllocation;
            _repoleaveTypes = repoleaveTypes;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: LeaveAllocation
        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _repoleaveTypes.FindAll();
            var mappedLeaveType = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveType,
                NumberUpdated = 0
            };
            return View(model);
            //var leaveAllocation = _repo.FindAll().ToList();
            //var model = _mapper.Map<List<LeaveAllocation>, List<LeaveAllocationVM>>(leaveAllocation);
            //return View(model);
        }
        public async Task<ActionResult> SetLeave(int id)
        {
            var leavetype = await _repoleaveTypes.FindById(id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                var checkallocation = await _repoleaveAllocation.CheckAllocation(id, emp.Id);
                if (checkallocation)
                    continue;
                    var allocation = new LeaveAllocationVM
                    {
                        DateCreated = DateTime.Now,
                        EmployeeId = emp.Id,
                        LeaveTypeId = id,
                        NumberOfDays = leavetype.DefaultDays,
                        Period = DateTime.Now.Year,
                    };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                await _repoleaveAllocation.Create(leaveallocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee =_mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            var allocations = _mapper.Map<List<LeaveAllocationVM>>(await _repoleaveAllocation.GetLeaveAllocationsByEmployee(id));
            var model = new ViewAllocationVM { Employee = employee, LeaveAllocationVMs = allocations };
            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: LeaveAllocation/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var leaveallocation = await _repoleaveAllocation.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveallocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                int idd=model.Id;
                if(!ModelState.IsValid)
                {
                    var leaveallocation =await _repoleaveAllocation.FindById(idd);
                    var model1 = _mapper.Map<EditLeaveAllocationVM>(leaveallocation);
                    model1.NumberOfDays = model.NumberOfDays;
                    return View(model1);
                }
                var record =await _repoleaveAllocation.FindById(model.Id);
                //var allocation = _mapper.Map<LeaveAllocation>(model);
                record.NumberOfDays = model.NumberOfDays;
                var isSuccess =await _repoleaveAllocation.Update(record);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Error while saving ");
                    return View(model);
                }
                return RedirectToAction(nameof(Details), new { id=model.EmployeeId});
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
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
