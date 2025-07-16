using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO
{
    public class UserEmbeddingDAO : GenericDAO<UserEmbedding>, IUserEmbeddingDAO
    {
        public UserEmbeddingDAO(ApplicationDbContext context) : base(context)
        {
        }
    }

}
