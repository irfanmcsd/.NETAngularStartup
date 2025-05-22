using DevCodeArchitect.DBContext;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    /// <summary>
    /// Checks if a given email address exists in the system
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>True if the email exists, false otherwise</returns>
    Task<bool> EmailExists(string email);
}

public class UserService : IUserService
{
    private readonly ApplicationDBContext _dbContext;

    public UserService(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserNameExists(string UserName)
    {
        // Check if any user in the database has this username
        return await _dbContext.Users.AnyAsync(u => u.UserName == UserName);
    }

    public async Task<bool> EmailExists(string email)
    {
        // Check if any user in the database has this email
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }
}
