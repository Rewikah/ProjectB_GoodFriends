using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends;

public class DetailsModel : PageModel
{
    private readonly IFriendsService _friendsService;
    private readonly IPetsService _petsService;
    private readonly IQuotesService _quotesService;

    public DetailsModel(IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService)
    {
        _friendsService = friendsService;
        _petsService = petsService;
        _quotesService = quotesService;
    }

    public IFriend? Friend { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(Guid id) => await LoadFriendAsync(id);

    public async Task<IActionResult> OnPostDeletePetAsync(Guid id, Guid petId)
    {
        try
        {
            await _petsService.DeletePetAsync(petId);
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadFriendAsync(id);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteQuoteAsync(Guid id, Guid quoteId)
    {
        try
        {
            await _quotesService.DeleteQuoteAsync(quoteId);
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadFriendAsync(id);
            return Page();
        }
    }

    private async Task LoadFriendAsync(Guid id)
    {
        try
        {
            var resp = await _friendsService.ReadFriendAsync(id, flat: false);
            Friend = resp.Item;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
