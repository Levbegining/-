using System;

namespace ProjectUser.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string HashPassword { get; set; }
    public string Role { get; set; }
}
