using System.Data;

namespace TaskManagementSystem.Repositories
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }

}
