using leave_management.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class LeaveRequestVM
    {
        public int Id { get; set; }
        public EmployeeVM RequestingEmployee { get; set; }
        [Display(Name ="Employee Name")]
        public string RequestingEmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }
        public DateTime DateActioned { get; set; }
        public bool? Approved { get; set; }
        public EmployeeVM ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
        public string CommentRequest { get; set; }
        public bool CancelRequest { get; set; }

    }
    public class AdminLeaveRequestViewVM
    {
        [Display(Name = "Total Numbers of Requests")]
        public int TotalRequests { get; set; }
        [Display(Name = "Approved Requests")] 
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pending Requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
    public class CreateLeaveRequestVM
    {
        [Display(Name = "Start Date")]
        [Required]
        //[DataType(DataType.Date)]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        //[DataType(DataType.Date)]   DataPicker
        public string EndDate { get; set; }
        [Display(Name = "Pending Requests")]
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }
        [Display(Name ="Comments")]
        [MaxLength(300)]
        public string CommentRequest { get; set; }
    }
    public class EmployeeLeaveRequestVM
    {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}
