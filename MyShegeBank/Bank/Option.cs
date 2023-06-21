using ShegeBank.Bank;
using ShegeBank.Enum;
using ShegeBank.LanguageChoice;
using ShegeBank.UserInterface;

namespace MyShegeBank.Bank;
internal class Option
{
    public static async Task UserInputAsync()
    {
        Atm atm = new();

        start: int input = Validate.Convert<int>($"{Languages.Display(63)}");

        switch (input)
        {
            case (int)UserChoice.CheckBalance:
                await atm.CheckBalanceAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.Deposit:
                await atm.DepositAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.Withdrawal:
                await atm.WithdrawalAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.Transfer:
                await atm.TransferAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.Airtime:
                await atm.AirtimeAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.TransactionHistory:
                await atm.ViewTransactionAsync();
                await Pick.QuestionAsync();
                break;
            case (int)UserChoice.Cancel:
                await Pick.QuestionAsync();
                break;
            default:
                Utility.PrintMessage($"{Languages.Display(3)}", false);
                goto start;
        }
    }
}