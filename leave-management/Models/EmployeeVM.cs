using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class EmployeeVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "First Name")]
        public string Firstname { get; set; }
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }
        public string TaxId { get; set; }
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }
    }
}
