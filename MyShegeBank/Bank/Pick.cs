using ShegeBank.ATM_Main;
using ShegeBank.LanguageChoice;
using ShegeBank.UserInterface;

namespace MyShegeBank.Bank;
internal class Pick
{
    public static async Task QuestionAsync()
    {
        await Utility.LoadingAsync("", ".", 6, 300);
        Console.Write($"{Languages.Display(64)}");
        exit: int pick = Validate.Convert<int>($"{Languages.Display(65)}");
        switch (pick)
        {
            case 1:
                await UserScreen.DisplayAtmMenuAsync();
                break;
            case 2:
                await CancelAsync();
                break;
            default:
                Utility.PrintMessage($"{Languages.Display(3)}", false);
                goto exit;
        }
    }
    public static async Task CancelAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Clear();
        await Utility.LoadingAsync("", "___", 5, 500);
        await Utility.LoadingAsync($"---------------{Languages.Display(66)}---------------", "", 6, 400);
        await Utility.LoadingAsync($"---------------{Languages.Display(67)}---------------", "", 6, 500);
        await ALwaysOnScreen.DisplayAsync();
    }
}