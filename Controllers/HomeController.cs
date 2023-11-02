using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebuggingChallenge.Models;

namespace DebuggingChallenge.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Hint on a few errors: pay close attention to how things are named

    [HttpGet("")]
    public IActionResult Index()
    {
        Reset();
        return View();
    }

    [HttpPost("user/create")]
    public IActionResult CreateUser(User newUser)
    {
        if(ModelState.IsValid)
        {
            Console.WriteLine(newUser.Name);
            Console.WriteLine(newUser.Location == null ? "Undisclosed" : newUser.Location);
            HttpContext.Session.SetString("Username", newUser.Name);
            if(newUser.Location != null)
            {
                HttpContext.Session.SetString("Location", $"{newUser.Location}");
            } else {
                HttpContext.Session.SetString("Location", "Undisclosed");
            }
            return RedirectToAction("Generator");
        } else {
            return View("Index");
        }
    }

    [HttpGet("generator")]
    public IActionResult Generator()
    {
        if(HttpContext.Session.GetString("Username") == null)
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("Passcode") == null)
        {
            GeneratePasscode();
        }
        return View();
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        // Console.WriteLine("Resetting...");
        HttpContext.Session.Clear();
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("Location");
        HttpContext.Session.Remove("Passcode");
        return RedirectToAction("Index");
    }

    // Hint: Something isn't right here...
    [HttpPost("generate/new")]
    public IActionResult GenerateNew()
    {
        GeneratePasscode();
        return RedirectToAction("Generator");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void GeneratePasscode()
    {
        string passcode = "";
        string CharOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string NumOptions = "0123456789";
        Random rand = new Random();
        for(int i = 1; i < 15; i++)
        {
            int odds = rand.Next(2);
            if(odds == 0)
            {
                passcode += CharOptions[rand.Next(CharOptions.Length)];
            } else {
                passcode += NumOptions[rand.Next(NumOptions.Length)];
            }
        }
        HttpContext.Session.SetString("Passcode", passcode);
        
    }
}
