namespace ShegeBank.Interfaces;

public interface IUserLogin
{
    Task ValidateCardNumberAndPasswordAsync();
}