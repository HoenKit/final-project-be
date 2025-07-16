using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserModuleDAO : IGenericDAO<UserModule>
    {
        Task<UserModule> GetUserModule(Guid userId, int moduleId);
        Task<bool> UserModuleExists(Guid userId, int moduleId);
        Task AddUserModuleAsync(UserModule userModule);
        Task<List<UserModule>> GetUserModulesAsync(Guid userId);
        Task UpdateUserModule(UserModule userModule);
    }

}
