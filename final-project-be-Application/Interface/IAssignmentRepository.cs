using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
	public interface IAssignmentRepository : IRepository<Assignment>
	{
		public Task<Assignment> CreateAssignment(AssignmentDto dto);
		public Task<bool> DeleteAssignment(int id);
		public Task<Assignment> GetAssignment(int id);
		public Task<Assignment> UpdateAssignment(UpdateAssignmentDto dto);
		public Task<ICollection<AssignmentResponseDto>> GetAllAssignmentByLessonId(int lessonId);
	}
}
