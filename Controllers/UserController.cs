using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using ProjectUser.Data;
using ProjectUser.Models;
using ProjectUser.Models.ViewModels;
using ProjectUser.WorkWithDb;

// Дз3 - сделать так, чтобы данные хранились в бд
// при регистрации пользователь записывается в бд

namespace ProjectUser.Controllers;


public class UserController : Controller
{
    // объект шифрования
    private readonly IDataProtector dataProtector;
    UserContext userContext;
    BusinessLogic context;
    public UserController(UserContext userContext, IDataProtectionProvider protectionProvider)
    {
        // указываем ключ для защиты
        dataProtector = protectionProvider.CreateProtector("AuthCookieProtector");

        this.userContext = userContext;
        context = new BusinessLogic(userContext);
    }

    // метод шифрования данных
    private string ProtectCookieValue(string data)
    {
        return dataProtector.Protect(data);
    }

    // метод для расшифровки данных
    private string UnProtectCookieValue(string protectedData)
    {
        return dataProtector.Unprotect(protectedData);
    }

    public ActionResult Index()
    {
        return View();
    }
    public IActionResult Registr()
    {
        return View();
    }
    #region Через json
    /*
    [HttpPost]
    public IActionResult Registr(string username, string password)
    {
        // Дз2 - сделать так, чтобы можно было создать только одного
        // юзера с определенным UserName СДЕЛАНО

        // Дз1 - использовать шифрование данных
        // хранить можно только хэш
        if(UserRepository.Users.FirstOrDefault(x => x.UserName == username) != null){
            return View();
        }
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
        UserRepository.AddUser(new Models.User(){
            UserName = username,
            HashPassword = hashPassword
        });
        return RedirectToAction("Index");
    }

    public IActionResult Login(){
        return View();
    }
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = UserRepository.GetUser(username, password);
        if (user == null){
            return View();
        }
        
        HttpContext.Response.Cookies.Append("auth", "1");
        return RedirectToAction("Index");
    }

    public IActionResult Download()
    {
        //Если не удается получить значение из cookie auth
        if(!HttpContext.Request.Cookies.TryGetValue("auth", out var val)){
            return RedirectToAction("Index");
        }
        return View();
    }
    */
    #endregion

    #region Через базу данных
    [HttpPost]
    public IActionResult Registr(string username, string password, string role = "default")
    {
        // Дз2 - сделать так, чтобы можно было создать только одного
        // юзера с определенным UserName СДЕЛАНО

        // Дз1 - использовать шифрование данных
        // хранить можно только хэш

        if (context.GetUser(username) != null)
        {
            return View();
        }
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
        context.AddUser(new Models.User()
        {
            UserName = username,
            HashPassword = hashPassword,
            Role = role
        });
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult SetAdminRole(string username)
    {
        var user = context.GetUser(username);
        if (user != null)
        {
            user.Role = "admin";
            context.UpdateUser(user);
        }
        return RedirectToAction("Index");
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = context.GetUser(username, password);
        if (user == null)
        {
            return View();
        }

        var cookieValue = username;
        var protectedUserName = ProtectCookieValue(cookieValue);
        HttpContext.Response.Cookies.Append("auth", protectedUserName);

        //context.
        if (username == "lex" && password == "1") user.Role = "admin";
        // назначить роль
        HttpContext.Response.Cookies.Append("role", user.Role);

        return RedirectToAction("Index");
    }


    #endregion
    public IActionResult Download()
    {
        //Если не удается получить значение из cookie auth
        if (!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue))
        {
            return RedirectToAction("Index");
        }

        try
        {
            // расшифровать
            var cookieValue = UnProtectCookieValue(protectedCookieValue);

            return View();
        }
        catch
        {
            return RedirectToAction("Login");
        }
    }
    public IActionResult Settings()
    {
        SettingsVM viewModel = new SettingsVM();
        //Если не удается получить значение из cookie auth
        if (!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue))
        {
            viewModel.IsAuthenticated = false;
        }

        try
        {
            // расшифровать
            var cookieValue = UnProtectCookieValue(protectedCookieValue);

            viewModel.IsAuthenticated = true;
            //viewModel.Name = cookieValue;
            viewModel.Users = context.GetAllUsers().Where(x => x.UserName != cookieValue).ToList();
            viewModel.CurrentUser = new Models.User()
            {
                UserName = cookieValue,
                HashPassword = "",
                Role = HttpContext.Request.Cookies["role"]
            };
            // костыль
            // context.UpdateUser(viewModel.CurrentUser);
            // var users = context.GetAllUsers();
        }
        catch
        {
            viewModel.IsAuthenticated = false;
        }
        return View(viewModel);
    }
    public IActionResult Delete(string username)
    {
        context.DeleteUser(username);
        return RedirectToAction("Index");
  }
    [HttpPost]
    public IActionResult SettingsForUser(string username)
    {
        var user = context.GetUser(username);
        return View(user);
    }
    [HttpPost]
    public IActionResult SettingsProfile(string username){
        var user = context.GetUser(username);
        return View(user);
    }
    [HttpPost]
    public IActionResult SettingsForUserPost(string prevHashPassword, string trueUserName, string name, string role, string password)
    {
        string hashPassword = "";
        if (password == "" || password == null) hashPassword = prevHashPassword;
        else { hashPassword = BCrypt.Net.BCrypt.HashPassword(password); }
        context.UpdateUser(new Models.User()
        {
            UserName = name,
            Role = role,
            HashPassword = hashPassword
        }, trueUserName);
        //HttpContext.Response.Cookies.Append("role", role);
        return RedirectToAction("Index");
    }
}
