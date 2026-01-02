using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends;

public class OverviewFriendsPetsByCountryModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public OverviewFriendsPetsByCountryModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public string? ErrorMessage { get; private set; }

    public string Country { get; private set; } = "";

    public List<string> Countries { get; private set; } = new();

    public List<Row> Rows { get; private set; } = new();

    public class Row
    {
        public string City { get; set; } = "";
        public int FriendsCount { get; set; }
        public int PetsCount { get; set; }
    }

    public async Task OnGetAsync(string? country)
    {
        try
        {

            var resp = await _friendsService.ReadFriendsAsync(
                seeded: true,
                flat: false,
                filter: "",
                pageNumber: 0,
                pageSize: 2000
            );

            List<IFriend> friends = resp?.PageItems ?? new List<IFriend>();

            Countries = friends
                .Select(f => (f.Address?.Country ?? "").Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            Country = (country ?? "").Trim();
            if (string.IsNullOrWhiteSpace(Country) && Countries.Count > 0)
                Country = Countries[0];

            if (string.IsNullOrWhiteSpace(Country))
            {
                Rows = new();
                return;
            }

            Rows = friends
                .Where(f => string.Equals((f.Address?.Country ?? "").Trim(), Country, StringComparison.OrdinalIgnoreCase))
                .GroupBy(f => (f.Address?.City ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => new Row
                {
                    City = string.IsNullOrWhiteSpace(g.Key) ? "Unknown" : g.Key,
                    FriendsCount = g.Count(),
                    PetsCount = g.Sum(f => f.Pets?.Count ?? 0)
                })
                .OrderBy(r => r.City)
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Countries = new();
            Country = "";
            Rows = new();
        }
    }
}
