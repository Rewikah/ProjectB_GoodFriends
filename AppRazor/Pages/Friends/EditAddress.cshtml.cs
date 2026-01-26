using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace AppRazor.Pages.Friends;

public class EditAddressModel : PageModel
{
    private readonly IFriendsService _friendsService;
    private readonly IAddressesService _addressesService;

    public EditAddressModel(
        IFriendsService friendsService,
        IAddressesService addressesService)
    {
        _friendsService = friendsService;
        _addressesService = addressesService;
    }

    [BindProperty]
    public AddressInput Input { get; set; } = new();

    public Guid FriendId { get; private set; }
    public string? ErrorMessage { get; private set; }

    public class AddressInput
    {
        public Guid AddressId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ0-9\s]*$",
            ErrorMessage = "Street Address får bara innehålla bokstäver, siffror och mellanslag.")]
        public string StreetAddress { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "ZipCode måste vara större än 0.")]
        public int ZipCode { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ0-9\s]*$",
            ErrorMessage = "City får bara innehålla bokstäver, siffror och mellanslag.")]
        public string City { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ0-9\s]*$",
            ErrorMessage = "Country får bara innehålla bokstäver, siffror och mellanslag.")]
        public string Country { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        FriendId = id;

        try
        {
            var res = await _friendsService.ReadFriendAsync(id, flat: false);
            var friend = res.Item;

            if (friend?.Address == null)
            {
                ErrorMessage = "Friend eller Address hittades inte.";
                return Page();
            }

            Input = new AddressInput
            {
                AddressId = friend.Address.AddressId,
                StreetAddress = friend.Address.StreetAddress,
                ZipCode = friend.Address.ZipCode,
                City = friend.Address.City,
                Country = friend.Address.Country
            };

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
{
    FriendId = id;

    if (!ModelState.IsValid)
        return Page();

    try
    {
        var dto = new AddressCuDto
        {
            AddressId = Input.AddressId,
            StreetAddress = Input.StreetAddress.Trim(),
            ZipCode = Input.ZipCode,
            City = Input.City.Trim(),
            Country = Input.Country.Trim()
        };

        await _addressesService.UpdateAddressAsync(dto);

        // säkerställ att Friend fortfarande är kopplad till Address
        var friendRes = await _friendsService.ReadFriendAsync(id, flat: false);
        var friend = friendRes?.Item;

        if (friend != null)
        {
            var friendDto = new FriendCuDto(friend)
            {
                AddressId = Input.AddressId
            };

            friendDto.EnsureValidity();
            await _friendsService.UpdateFriendAsync(friendDto);
        }

        return RedirectToPage("/Friends/Details", new { id });
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
        ModelState.AddModelError(string.Empty, ex.Message);
        return Page();
    }
}

}
