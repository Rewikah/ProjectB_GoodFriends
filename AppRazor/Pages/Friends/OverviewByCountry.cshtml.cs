using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace AppRazor.Pages.Friends;

public class OverviewByCountryModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public OverviewByCountryModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public string? ErrorMessage { get; private set; }

    public List<Row> Rows { get; private set; } = new();

    public class Row
    {
        public string Country { get; set; } = "";
        public int Count { get; set; }
    }

    public async Task OnGetAsync()
    {
        try
        {

            var resp = await _friendsService.ReadFriendsAsync(
                seeded: true,
                flat: false,
                filter: "",
                pageNumber: 0,  
                pageSize: 1000
            );

            var friends = resp?.PageItems ?? new();

            Rows = friends
                .GroupBy(f => (f.Address?.Country ?? "").Trim())
                .Select(g => new Row
                {
                    Country = string.IsNullOrWhiteSpace(g.Key) ? "Unknown" : g.Key,
                    Count = g.Count()
                })
                .OrderBy(r => r.Country)
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Rows = new();
        }
    }
}
