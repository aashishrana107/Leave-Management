using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private ApplicationDbContext _db;
        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CheckAllocation(int leavetypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            return FindAll().Where(x => x.EmployeeId == employeeid && x.LeaveTypeId == leavetypeid && x.Period == period).Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            return _db.LeaveAllocations.Include(x=>x.LeaveType).Include(x => x.Employee).ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _db.LeaveAllocations.Include(x => x.Employee).Include(x => x.LeaveType).FirstOrDefault(x=>x.Id==id);
            //return _db.LeaveAllocations.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return FindAll().Where(x => x.EmployeeId == id && x.Period==period).ToList();
        }

        public bool isExists(int id)
        {
            return _db.LeaveAllocations.Any(x => x.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
