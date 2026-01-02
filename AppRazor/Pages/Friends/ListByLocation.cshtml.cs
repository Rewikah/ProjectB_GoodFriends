using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends;

public class ListByLocationModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public ListByLocationModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public string? ErrorMessage { get; private set; }

    public string Country { get; private set; } = "";
    public string City { get; private set; } = "";

    public List<string> Countries { get; private set; } = new();
    public List<string> Cities { get; private set; } = new();

    public List<Row> Rows { get; private set; } = new();

    public class Row
    {
        public Guid FriendId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
    }

    public async Task OnGetAsync(string? country, string? city)
    {
        try
        {
            var resp = await _friendsService.ReadFriendsAsync(
                seeded: true,
                flat: false,
                filter: "",
                pageNumber: 0,
                pageSize: 5000
            );

            List<IFriend> friends = resp?.PageItems ?? new();

            Country = (country ?? "").Trim();
            City = (city ?? "").Trim();

            Countries = friends
                .Select(f => (f.Address?.Country ?? "").Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            IEnumerable<IFriend> citySource = friends;
            if (!string.IsNullOrWhiteSpace(Country))
            {
                citySource = citySource.Where(f => string.Equals(
                    (f.Address?.Country ?? "").Trim(),
                    Country,
                    StringComparison.OrdinalIgnoreCase));
            }

            Cities = citySource
                .Select(f => (f.Address?.City ?? "").Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();


            IEnumerable<IFriend> filtered = friends;

            if (!string.IsNullOrWhiteSpace(Country))
            {
                filtered = filtered.Where(f => string.Equals(
                    (f.Address?.Country ?? "").Trim(),
                    Country,
                    StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(City))
            {
                filtered = filtered.Where(f => string.Equals(
                    (f.Address?.City ?? "").Trim(),
                    City,
                    StringComparison.OrdinalIgnoreCase));
            }

            Rows = filtered
                .OrderBy(f => f.LastName)
                .ThenBy(f => f.FirstName)
                .Select(f => new Row
                {
                    FriendId = f.FriendId,
                    Name = $"{f.FirstName} {f.LastName}",
                    Email = f.Email,
                    City = (f.Address?.City ?? "-").Trim(),
                    Country = (f.Address?.Country ?? "-").Trim()
                })
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Countries = new();
            Cities = new();
            Rows = new();
            Country = "";
            City = "";
        }
    }
}
