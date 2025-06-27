using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests.TestDAOs
{
    public class NoTransactionAssignmentDAO : AssignmentDAO
    {
        public NoTransactionAssignmentDAO(ApplicationDbContext context) : base(context) { }

        public override Task BeginTransactionAsync() => Task.CompletedTask;
        public override Task CommitTransactionAsync() => Task.CompletedTask;
        public override Task RollbackTransactionAsync() => Task.CompletedTask;
    }

}
