using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Pets;

public class DeleteModel : PageModel
{
    private readonly IPetsService _petsService;

    public DeleteModel(IPetsService petsService)
    {
        _petsService = petsService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IPet? Pet { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            var res = await _petsService.ReadPetAsync(Id, flat: true);
            Pet = res.Item;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _petsService.DeletePetAsync(Id);

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                return Redirect(ReturnUrl);

            return RedirectToPage("/Pets/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;

            try
            {
                var res = await _petsService.ReadPetAsync(Id, flat: true);
                Pet = res.Item;
            }
            catch { }

            return Page();
        }
    }
}
