using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class AssignmentDAO : GenericDAO<Assignment>, IAssignmentDAO
    {
        public AssignmentDAO(ApplicationDbContext context) : base(context)
        {
        }

    }

}
