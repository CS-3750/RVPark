namespace RVPark.Core.Models;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual bool Authenticate(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public virtual bool UpdateProfile(UserData data)
    {
        if (data == null) return false;
        
        FirstName = data.FirstName ?? FirstName;
        LastName = data.LastName ?? LastName;
        Email = data.Email ?? Email;
        
        return true;
    }

    public virtual string ResetPassword()
    {
        var newPassword = GenerateRandomPassword();
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        return newPassword;
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public class UserData
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}