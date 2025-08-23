using DocumentFormat.OpenXml.Wordprocessing;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Drawing.Printing;
using final_project_be_Domain.DTOs.SearchResult;
using final_project_be_Domain.DTOs.Courses;
using Microsoft.AspNetCore.Authorization;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IPostRepository _postRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        public PostController(IUserRepository userRepository,ICourseRepository courseRepository,IPostRepository postRepository, IHubContext<SignalRHub> hubContext, INotificationRepository notificationRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
        }

        [HttpGet("search-all")]
        public IActionResult SearchAll(
        int? page,
        int? pageSize,
        string? searchTerm,
        string? searchType = "all") // "users", "posts", "courses", "all"
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 20;

            var searchResults = new SearchResultDto
            {
                CurrentPage = currentPage,
                PageSize = currentSize
            };

            // Search Users
            if (searchType == "all" || searchType == "users")
            {
                var allUsers = _userRepository.GetAllUsers(currentPage, currentSize);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();

                    searchResults.Users = allUsers.Items
                        .Where(u =>
                            (u.Email?.ToLower().Contains(lowerSearchTerm) ?? false) ||
                            (u.UserMetaData != null &&
                                (
                                    (u.UserMetaData.FirstName?.ToLower().Contains(lowerSearchTerm) ?? false) ||
                                    (u.UserMetaData.LastName?.ToLower().Contains(lowerSearchTerm) ?? false)
                                )
                            )
                        )
                        .Cast<object>()
                        .ToList();
                }
                else
                {
                    searchResults.Users = allUsers.Items.Cast<object>().ToList();
                }
            }


            // Search Posts
            if (searchType == "all" || searchType == "posts")
            {
                var allPosts = _postRepository.GetAllPosts(currentPage, currentSize, null, searchTerm, null, false);
                searchResults.Posts = allPosts.Items.Cast<object>().ToList();
            }

            // Search Courses
            if (searchType == "all" || searchType == "courses")
            {
                var allCourses = _courseRepository.GetAllCourses(
                    currentPage, currentSize, null, searchTerm,
                    null, null, null, null, null, null, null, null, null, null
                );

                /* searchResults.Courses = allCourses.Items
                     .Select(c => new GetCourseDto
                     {
                         CourseId = c.CourseId,
                         CourseName = c.CourseName,
                         CourseContent = c.CourseContent,
                         Cost = c.Cost,
                         SkillLearn = c.SkillLearn,
                         Requirement = c.Requirement ?? string.Empty,
                         IntendedLearner = c.IntendedLearner ?? string.Empty,
                         Language = c.Language,
                         Level = c.Level,
                         StudentCount = c.StudentCount,
                         CoursesImage = c.CoursesImage,
                         CourseLength = c.CourseLength,
                         IsDeleted = c.IsDeleted,
                         Status = c.Status,
                         AverageRating = c.AverageRating,
                         TotalReviews = c.TotalReviews,
                         CreateAt = c.CreateAt,
                         Mentor = c.Mentor // nếu repository có include Mentor
                     })
                     .Cast<object>() 
                     .ToList();*/
                searchResults.Courses = allCourses.Items.Cast<object>().ToList();
            }

            searchResults.TotalResults = searchResults.Users.Count + searchResults.Posts.Count + searchResults.Courses.Count;

            return Ok(searchResults);
        }

        // GET: api/<PostController>
        //UpdateAsync GetAllPost
        [HttpGet]
        public IActionResult GetAll(int? page, int? pageSize, int? CategoryId, string? title, Guid? userId)
        {
            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 5;

            var pagedPosts = _postRepository.GetAllPosts(currentPage, currentSize, CategoryId, title, userId, false);
            return Ok(pagedPosts);
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _postRepository.GetPost(id));
        }


        // POST api/<PostController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var post = await _postRepository.CreatePost(postDto);

            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            await _notificationRepository.CreateNotification(new NotificationDto
            {
                UserId = post.UserId, 
                Message = $"New post has been created"
            });
            return Ok(post);
        }

        // PUT api/<PostController>/5
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var post = await _postRepository.UpdatePost(postDto);
            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            return Ok(post);
        }

        // DELETE api/<PostController>/5
        [Authorize]
        [HttpPut("toggle-deleted/{id}")]
        public async Task<IActionResult> TogglePostDeleteStatus(int id)
        {
            var updatedPost = await _postRepository.ToggleIsDeleted(id);
            if (updatedPost == null)
            {
                return StatusCode(500, "Failed to UpdateAsync post status.");
            }
            await _hubContext.Clients.All.SendAsync("ReceivePost", updatedPost);
            return Ok(updatedPost);
        }

        [Authorize]
        [HttpGet("monthly-stats")]
        public IActionResult GetPostStatisticsByMonth()
        {
            var stats = _postRepository.GetPostStatisticsByMonth();
            return Ok(stats);
        }
        [Authorize]
        [HttpGet("GetAllIsDeleted")]
        public IActionResult GetAllIsDeleted(int? page, int? CategoryId, string? title, Guid? userId)
        {
            int currentPage = page ?? 1;

            var pagedPosts = _postRepository.GetAllPosts(currentPage, 5, CategoryId, title, userId, true);
            return Ok(pagedPosts);
        }

        [HttpGet("GetDetail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _postRepository.GetPostDetail(id));
        }
    }
}
