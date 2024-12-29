using System;
using ProjectUser.Models;

namespace ProjectUser.Data;

public static class UserRepository
{
    public static List<User> Users = new List<User>();

    public static void AddUser(User user)
    {
        Users.Add(user);
    }

    public static User GetUser(string username, string password)
    {
        return Users.FirstOrDefault(x => x.UserName == username && x.HashPassword == password);
    }
    public static User GetUser(string username){
        return Users.FirstOrDefault(x => x.UserName == username);
    }
    public static void UpdateUser(User user){
        // search user
        var u = Users.FirstOrDefault(x => x.UserName == user.UserName);

        // update
        u.Role = user.Role;

        // save(сюда писать ничего не надо)
    }
}
