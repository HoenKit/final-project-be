using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Domain.DTOs.Module;

namespace final_project_be_Application.Interface
{
	public interface IModuleRepository : IRepository<Module>
	{
		public Task<Module> CreateModule(ModuleDto dto);
		public Task<bool> DeleteModule(int id);
		public Task<Module> GetModule(int id);
		public Task<Module> UpdateModule(UpdateModuleDto dto);
		public Task<ICollection<ModuleDto>> GetAllModulesByCourseId(int courseId);
	}
}
