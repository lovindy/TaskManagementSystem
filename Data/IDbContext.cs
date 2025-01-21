using System.Data;

namespace TaskManagementSystem.Data
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }

}
