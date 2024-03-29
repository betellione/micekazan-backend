using System.ComponentModel.DataAnnotations;
using WebApp1.Enums;

namespace WebApp1.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Укажите вашу почту")]
    [EmailAddress(ErrorMessage = "Укажите валидную почту")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Укажите ваше имя")]
    [StringLength(64, ErrorMessage = "{0} должно быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [Display(Name = "Имя")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Укажите вашу фамилию")]
    [StringLength(64, ErrorMessage = "{0} должна быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Укажите ваше отчество")]
    [StringLength(64, ErrorMessage = "{0} должно быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [Display(Name = "Отчество")]
    public string Patronymic { get; set; } = null!;

    [Required(ErrorMessage = "Укажите ваш город")]
    [StringLength(32, ErrorMessage = "{0} должен быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 2)]
    [Display(Name = "Город")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Укажите ваш телефон")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Укажите валидный номер телефона")]
    [Display(Name = "Номер телефона")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Укажите вашу занятость")]
    [Display(Name = "Занятость")]
    public ActivityTypes Activity { get; set; }

    [Required(ErrorMessage = "Укажите ваш пароль")]
    [StringLength(100, ErrorMessage = "{0} должен быть минимум {2} и максимум {1} символов в длину.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = null!;
}