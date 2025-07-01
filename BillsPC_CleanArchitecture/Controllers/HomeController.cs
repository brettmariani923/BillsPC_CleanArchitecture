using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Models; 
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPokemonService _pokemonService;

    public HomeController(ILogger<HomeController> logger, IPokemonService pokemonService)
    {
        _logger = logger;
        _pokemonService = pokemonService;
    }

    public async Task<IActionResult> Index()
    {
        var team = await _pokemonService.GetTeamAsync();
        return View(team);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
