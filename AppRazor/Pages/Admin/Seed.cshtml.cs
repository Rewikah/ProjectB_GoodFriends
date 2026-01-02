using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Text.Json;

namespace AppRazor.Pages.Admin;

public class SeedModel : PageModel
{
    private readonly IAdminService _admin;

    public SeedModel(IAdminService admin)
    {
        _admin = admin;
    }

    public string? ResultJson { get; set; }

    public void OnGet()
    {
    }

    // Körs när Seed-knappen trycks
    public async Task<IActionResult> OnPostSeedAsync(int nr)
    {
        var res = await _admin.SeedAsync(nr);
        ResultJson = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true });
        return Page();
    }

    // Körs när Remove seed-knappen trycks
    public async Task<IActionResult> OnPostRemoveAsync()
    {
        var res = await _admin.RemoveSeedAsync(true);
        ResultJson = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true });
        return Page();
    }
}
