using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserAssignmentDAO : IGenericDAO<UserAssignment>
    {
        public  Task<List<UserAssignment>> GetUserAssignmentsByAssignmentIdAsync(int assignmentId);
        public Task<UserAssignment?> GetUserAssignmentAsync(Guid userId, int assignmentId);

        public Task<List<UserAssignment>> GetUserAssignmentsByUserIdsAndAssignmentIdAsync(List<Guid> userIds, int assignmentId);
    }

}
