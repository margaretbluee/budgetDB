public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}