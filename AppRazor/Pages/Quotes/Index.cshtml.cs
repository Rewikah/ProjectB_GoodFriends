using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Quotes
{
    public class IndexModel : PageModel
    {
        private readonly IQuotesService _quotesService;

        public IndexModel(IQuotesService quotesService)
        {
            _quotesService = quotesService;
        }

        public List<IQuote> Quotes { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNr { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int DbItemsCount { get; private set; }
        public int PageCount { get; private set; }
        public string? ErrorMessage { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {

                var filter = (Filter ?? "").ToLower();

                var res = await _quotesService.ReadQuotesAsync(
                    seeded: true,
                    flat: true,
                    filter: filter,
                    pageNumber: PageNr,
                    pageSize: PageSize
                );

                Quotes = res.PageItems ?? new List<IQuote>();
                DbItemsCount = res.DbItemsCount;
                PageCount = res.PageCount;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Quotes = new List<IQuote>();
                DbItemsCount = 0;
                PageCount = 0;
            }
        }
    }
}
