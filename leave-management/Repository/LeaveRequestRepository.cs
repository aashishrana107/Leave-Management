using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private ApplicationDbContext _db;
        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            var leaveRequest= _db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .ToList();
            return leaveRequest;
                
        }

        public LeaveRequest FindById(int id)
        {
            return _db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .FirstOrDefault(x=>x.Id==id);
            //return _db.LeaveHistorys.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeid)
        {
            return FindAll().Where(x => x.RequestingEmployeeId == employeeid).ToList();
            //var leaveRequest = _db.LeaveRequests
            //   .Include(x => x.RequestingEmployee)
            //   .Include(x => x.ApprovedBy)
            //   .Include(x => x.LeaveType)
            //   .Where(x => x.RequestingEmployeeId == employeeid)
            //   .ToList();
            //return leaveRequest;
        }

        public bool isExists(int id)
        {
            return _db.LeaveRequests.Any(x => x.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
