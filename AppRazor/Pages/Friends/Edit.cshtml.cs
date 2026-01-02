using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppRazor.Pages.Friends;

public class EditModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public EditModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Guid FriendId { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel : IValidatableObject
    {
        [Display(Name = "Förnamn")]
        [Required(ErrorMessage = "Förnamn är obligatoriskt.")]
        [StringLength(50, ErrorMessage = "Förnamn får vara max 50 tecken.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Förnamn får bara innehålla bokstäver, siffror och mellanslag.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Efternamn")]
        [Required(ErrorMessage = "Efternamn är obligatoriskt.")]
        [StringLength(50, ErrorMessage = "Efternamn får vara max 50 tecken.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Efternamn får bara innehålla bokstäver, siffror och mellanslag.")]
        public string LastName { get; set; } = "";

        [Display(Name = "E-post")]
        [Required(ErrorMessage = "E-post är obligatoriskt.")]
        [EmailAddress(ErrorMessage = "E-post måste vara en giltig adress.")]
        [StringLength(254, ErrorMessage = "E-post får vara max 254 tecken.")]
        public string Email { get; set; } = "";

        [Display(Name = "Födelsedag")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Birthday.HasValue)
            {
                var d = Birthday.Value.Date;
                if (d < new DateTime(1900, 1, 1) || d > DateTime.Today)
                {
                    yield return new ValidationResult(
                        "Födelsedag måste vara ett datum mellan 1900-01-01 och idag (eller lämnas tom).",
                        new[] { nameof(Birthday) }
                    );
                }
            }
        }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        FriendId = id;

        try
        {
            var response = await _friendsService.ReadFriendAsync(id, flat: true);
            var friend = response?.Item;

            if (friend == null)
            {
                ErrorMessage = "Kunde inte hitta friend.";
                return Page();
            }

            Input = new InputModel
            {
                FirstName = friend.FirstName ?? "",
                LastName = friend.LastName ?? "",
                Email = friend.Email ?? "",
                Birthday = friend.Birthday
            };

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        FriendId = id;

        if (!ModelState.IsValid)
            return Page();

        Input.FirstName = Input.FirstName?.Trim() ?? "";
        Input.LastName  = Input.LastName?.Trim() ?? "";
        Input.Email     = Input.Email?.Trim() ?? "";

        if (!Regex.IsMatch(Input.FirstName, @"^[a-zA-Z0-9\s]*$"))
            ModelState.AddModelError("Input.FirstName", "Förnamn får bara innehålla bokstäver, siffror och mellanslag.");

        if (!Regex.IsMatch(Input.LastName, @"^[a-zA-Z0-9\s]*$"))
            ModelState.AddModelError("Input.LastName", "Efternamn får bara innehålla bokstäver, siffror och mellanslag.");

        if (!Regex.IsMatch(Input.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            ModelState.AddModelError("Input.Email", "E-post måste vara en giltig e-postadress.");

        if (Input.Birthday.HasValue)
        {
            var d = Input.Birthday.Value.Date;
            if (d < new DateTime(1900, 1, 1) || d > DateTime.Today)
                ModelState.AddModelError("Input.Birthday", "Födelsedag måste vara ett datum mellan 1900-01-01 och idag (eller lämnas tom).");
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new FriendCuDto
            {
                FriendId = id,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Birthday = Input.Birthday
            };

            dto.EnsureValidity();

            await _friendsService.UpdateFriendAsync(dto);

            return RedirectToPage("/Friends/Details", new { id });
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
