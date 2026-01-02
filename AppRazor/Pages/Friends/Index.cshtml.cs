using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends;

public class IndexModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public IndexModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public List<IFriend> Friends { get; private set; } = new();

    public int PageNr { get; private set; } = 1;
    public int PageSize { get; private set; } = 10;
    public int PageCount { get; private set; } = 0;
    public int DbItemsCount { get; private set; } = 0;

    public string Filter { get; private set; } = "";

    public async Task OnGetAsync(int pageNr = 1, int pageSize = 10, string filter = "")
    {
        PageNr = pageNr < 1 ? 1 : pageNr;
        PageSize = pageSize < 1 ? 10 : pageSize;
        Filter = filter ?? "";

        var resp = await _friendsService.ReadFriendsAsync(
            seeded: true,
            flat: true,
            filter: Filter,
            pageNumber: PageNr - 1,
            pageSize: PageSize
        );

        Friends = resp.PageItems ?? new();
        DbItemsCount = resp.DbItemsCount;
        PageCount = resp.PageCount;
    }
}
