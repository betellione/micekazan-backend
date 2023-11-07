using System.ComponentModel.DataAnnotations;
using WebApp1.Enums;

namespace WebApp1.ViewModels.Account;

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Укажите валидную почту")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    
    [Required]
    [StringLength(64, ErrorMessage = "{0} должно быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [RegularExpression(@"^\p{L}+$", ErrorMessage = "{0} должно состоять только из букв")]
    [Display(Name = "Имя")]
    public string Name { get; set; } = null!;
    
    [Required]
    [StringLength(64, ErrorMessage = "{0} должна быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [RegularExpression(@"^\p{L}+$", ErrorMessage = "{0} должна состоять только из букв")]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; } = null!;
    
    [Required]
    [StringLength(64, ErrorMessage = "{0} должно быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [RegularExpression(@"^\p{L}+$", ErrorMessage = "{0} должно состоять только из букв")]
    [Display(Name = "Отчество")]
    public string Patronymic { get; set; } = null!;
    
    [Required]
    [StringLength(32, ErrorMessage = "{0} должен быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [Display(Name = "Город")]
    public string City { get; set; } = null!;
    
    [Required]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Укажите валидный номер телефона")]
    [Display(Name = "Номер телефона")]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    [Display(Name = "Занятость")]
    public ActivityTypes Activity { get; set; } = ActivityTypes.Individual;

    [Required]
    [StringLength(100, ErrorMessage = "{0} должен быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = null!;
}