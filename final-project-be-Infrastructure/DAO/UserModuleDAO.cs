using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class UserModuleDAO : GenericDAO<UserModule>
	{
		private readonly ApplicationDbContext _context;
		public UserModuleDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<UserModule> GetUserModule(Guid userId, int moduleId)=> await _context.UserModules.FirstOrDefaultAsync(um => um.UserId == userId && um.ModuleId == moduleId);
        public async Task<bool> UserModuleExists(Guid userId, int moduleId)=>await _context.UserModules.AnyAsync(um => um.UserId == userId && um.ModuleId == moduleId);
        public async Task AddUserModuleAsync(UserModule userModule)=> await _context.UserModules.AddAsync(userModule);
        public Task<List<UserModule>> GetUserModulesAsync(Guid userId)
        {
            return _context.UserModules
                .Where(um => um.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateUserModule(UserModule userModule)
        {
            _context.UserModules.Update(userModule);
            await _context.SaveChangesAsync();
        }
    }
}
