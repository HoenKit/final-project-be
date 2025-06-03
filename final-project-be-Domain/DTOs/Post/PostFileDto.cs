namespace final_project_be_Domain.DTOs.Post
{
	public class PostFileDto
	{
		public int PostFileId { get; set; }
		public int PostId { get; set; }
		public string FileUrl { get; set; }
		public string PostFileType { get; set; }
		public bool? IsDeleted { get; set; }
	}

	//AddAsync PostFileCreateDto
	public class PostFileCreateDto
	{
		public int PostFileId { get; set; }
		public string FileUrl { get; set; }
		public string PostFileType { get; set; }
		public bool? IsDeleted { get; set; }
	}

}
