using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ILessonRepository : IRepository<Lesson>
    {
		public Task<Lesson> CreateLesson(LessonDto dto);
		public Task<bool> DeleteLesson(int id);
		public Task<Lesson> GetLesson(int id);
		public Task<Lesson> UpdateLesson(UpdateLessonDto dto);
		public Task<ICollection<LessonResponseDto>> GetAllLessonByModuleId(int moduleId);
	}
}
