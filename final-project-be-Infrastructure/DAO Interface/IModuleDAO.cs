using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IModuleDAO : IGenericDAO<Module>
    {
        Task<Module?> GetByIdAsync(int id);
        Task<List<Module>> GetModulesByCourseId(int courseId);
        Task<List<Module>> GetModulesWithLessonsByCourseIdAsync(int courseId);
        Task<Module?> GetByCourseIdAsync(int courseId);
    }

}
