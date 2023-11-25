using System.Net;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.PhoneConfirmationService.PhoneCaller;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.PhoneConfirmationService;

public class PhoneConfirmationService : IPhoneConfirmationService
{
    private readonly ILogger _logger = Log.ForContext<IPhoneConfirmationService>();
    private readonly IPhoneCaller _phoneCaller;
    private readonly ApplicationDbContext _dbContext;

    public PhoneConfirmationService(IPhoneCaller phoneCaller, ApplicationDbContext dbContext)
    {
        _phoneCaller = phoneCaller;
        _dbContext = dbContext;
    }

    public async Task<bool> MakePhoneCallWithToken(Guid userId, string phoneNumber, string confirmationToken, IPAddress? ip = null)
    {
        string code;

        try
        {
            code = await _phoneCaller.MakePhoneCall(phoneNumber, ip);
        }
        catch (Exception)
        {
            return false;
        }

        var userPhone = new UserConfirmationPhoneCall
        {
            UserId = userId,
            UserPhoneNumber = phoneNumber,
            ConfirmationPhoneCode = code,
            ConfirmationToken = confirmationToken,
            Timestamp = DateTime.UtcNow,
        };

        try
        {
            _dbContext.UserConfirmationPhoneCalls.Add(userPhone);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot add User Confirmation Phone Call in the database for user with ID {Id}", userId);
            return false;
        }
    }

    public async Task<string> GetConfirmationTokenForUser(Guid userId, string phoneNumber, string code)
    {
        var userToken = await _dbContext.UserConfirmationPhoneCalls
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp)
            .Select(x => x.ConfirmationToken)
            .FirstOrDefaultAsync();

        return userToken ?? string.Empty;
    }
}