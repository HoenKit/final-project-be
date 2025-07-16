using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class AnswerDAO : GenericDAO<Answer>, IAnswerDAO
    {
        public AnswerDAO(ApplicationDbContext context) : base(context)
        {
        }
    }

}
