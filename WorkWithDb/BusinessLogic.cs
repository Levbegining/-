using System;
using Microsoft.EntityFrameworkCore;
using ProjectUser.Data;
using ProjectUser.Models;

namespace ProjectUser.WorkWithDb;

public class BusinessLogic
{
    public UserContext userContext;

    public BusinessLogic(UserContext userContext)
    {
        this.userContext = userContext;
    }

    public List<User>? GetAllUsers()
    {
        return userContext.Users.ToList();
    }
    public User? GetUser(string username, string password){
        var user = userContext.Users.FirstOrDefault(x => 
            x.UserName == username
        );
        if (user == null || BCrypt.Net.BCrypt.Verify(password, user.HashPassword) == false){
            return new User();
        }
        
        return user;
    }
    public User? GetUser(string username){
        var user = userContext.Users.FirstOrDefault(x => 
            x.UserName == username
        );
        
        return user;
    }
    public void AddUser(User user){
        userContext.Users.Add(user);
        userContext.SaveChanges();
    }
    public void UpdateUser(User user){
        // search users
        var u = userContext.Users.FirstOrDefault(x => x.UserName == user.UserName);
        if(u == null){
            return;
        }
        // update
        // временно(костыль)
        if(u == null){
            u.Role = "user";
        }
        else{
            u.Role = user.Role;
        }

        u.UserName = user.UserName;
        u.HashPassword = user.HashPassword;

        // save
        userContext.SaveChanges();
    }
    public void UpdateUser(User user, string trueUserName){
        // search users
        var u = userContext.Users.FirstOrDefault(x => x.UserName ==  trueUserName);
        if(u == null){
            return;
        }
        // update
        // временно(костыль)
        if(u == null){
            u.Role = "default";
        }
        else{
            u.Role = user.Role;
        }

        u.UserName = user.UserName;
        // костыль???
        u.HashPassword = user.HashPassword;

        // save
        userContext.SaveChanges();
    }
    public void DeleteUser(string username){
        userContext.Users.Remove(userContext.Users.FirstOrDefault(x => x.UserName == username));
        userContext.SaveChanges();
    }
}
