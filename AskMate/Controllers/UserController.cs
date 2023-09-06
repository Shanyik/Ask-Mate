using System.Security.Claims;
using AskMate.Model;
using AskMate.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly string _connectionString =
        "Server=localhost;Port=5432;User Id=postgres;Password=asd;Database=askmate";

    [HttpGet]
    public IActionResult GetAll()
    {
        var repository = new UserRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.GetAll());
    }
    
    [HttpPost()]
    public IActionResult Create(string username, string email, string password)
    {
        var repository = new UserRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.Create(username, email, password));
    }
    
    [HttpPost, Route("login")]
    public IActionResult Login(string username, string email, string password)
    {
        var repository = new UserRepository(new NpgsqlConnection(_connectionString));

        var claims = repository.Login(username, email, password);
        
        if (claims != null)
        {
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties{
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true
            };
        
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);
        
            return Ok();
        }

        return BadRequest("Something went wrong. Try again!");
    }

    [HttpPost, Route("signOut")]
    public IActionResult SignOutOfUser()
    {
       HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

       return Ok();
    }
}