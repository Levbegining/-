using System;

namespace ProjectUser.Models.ViewModels;

public class SettingsVM // Settings View Model
{
    public bool IsAuthenticated { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; }
    public User CurrentUser { get; set; }

}
