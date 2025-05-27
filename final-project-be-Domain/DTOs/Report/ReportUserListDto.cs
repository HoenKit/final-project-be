using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.User;
using final_project_be_Domain.DTOs.Report;

namespace final_project_be_Domain.DTOs.Report
{
	public class ReportUserListDto
    {
        public ReportDto ReportDto { get; set; }
        public UserDto userDto { get; set; }
    }
}
