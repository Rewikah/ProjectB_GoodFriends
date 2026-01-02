using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Quotes;

public class DeleteModel : PageModel
{
    private readonly IQuotesService _quotesService;

    public DeleteModel(IQuotesService quotesService)
    {
        _quotesService = quotesService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IQuote? Quote { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            var res = await _quotesService.ReadQuoteAsync(Id, flat: true);
            Quote = res.Item;
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
            await _quotesService.DeleteQuoteAsync(Id);

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                return Redirect(ReturnUrl);

            return RedirectToPage("/Quotes/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;

            try
            {
                var res = await _quotesService.ReadQuoteAsync(Id, flat: true);
                Quote = res.Item;
            }
            catch { }

            return Page();
        }
    }
}
