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

    public async Task<IActionResult> OnPostSeedAsync(int nr)
    {
        var res = await _admin.SeedAsync(nr);
        ResultJson = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true });
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync()
    {
        var res = await _admin.RemoveSeedAsync(true);
        ResultJson = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true });
        return Page();
    }
}
