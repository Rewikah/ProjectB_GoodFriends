using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Pets;

public class IndexModel : PageModel
{
    private readonly IPetsService _petsService;

    public IndexModel(IPetsService petsService)
    {
        _petsService = petsService;
    }

    public List<IPet> Pets { get; set; } = new();

    public string Filter { get; set; } = "";
    public int PageNr { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int DbItemsCount { get; set; }
    public int PageCount { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? filter, int pageNr = 1, int pageSize = 10)
    {
        Filter = filter ?? "";
        PageNr = pageNr < 1 ? 1 : pageNr;
        PageSize = pageSize < 1 ? 10 : pageSize;

        try
        {

            var res = await _petsService.ReadPetsAsync(
                seeded: true,
                flat: false,
                filter: Filter.ToLower(),
                pageNumber: PageNr - 1,
                pageSize: PageSize);

            Pets = res.PageItems ?? new List<IPet>();
            DbItemsCount = res.DbItemsCount;
            PageCount = res.PageCount;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Pets = new();
            DbItemsCount = 0;
            PageCount = 1;
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid petId, string? filter, int pageNr = 1, int pageSize = 10)
{
    await _petsService.DeletePetAsync(petId);
    return RedirectToPage("/Pets/Index", new { filter, pageNr, pageSize });
}   
}
