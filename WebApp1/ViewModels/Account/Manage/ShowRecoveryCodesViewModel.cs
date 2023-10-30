using Microsoft.AspNetCore.Mvc;

namespace WebApp1.ViewModels.Account.Manage;

public class ShowRecoveryCodesViewModel
{
    [TempData]
    public IList<string> RecoveryCodes { get; set; } = null!;

    [TempData]
    public string StatusMessage { get; set; } = null!;
}