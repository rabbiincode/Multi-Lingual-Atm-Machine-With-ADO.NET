namespace ShegeBank.Interfaces;

public interface IUserMainOptions
{
    Task CheckBalanceAsync();
    Task DepositAsync();
    Task WithdrawalAsync();
    Task TransferAsync();
    Task AirtimeAsync();
}