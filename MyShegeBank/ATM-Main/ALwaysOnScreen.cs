using MyShegeBank.DataBase;
using ShegeBank.Bank;
using ShegeBank.UserInterface;

namespace ShegeBank.ATM_Main;

internal class ALwaysOnScreen
{
    internal static async Task DisplayAsync()
    {
        Atm atm = new();
        CreateDatabase database = new();
        await database.CreateDataBaseAsync();
        UserScreen.Welcome();
        await atm.ValidateCardNumberAndPasswordAsync();
        await UserScreen.DisplayAtmMenuAsync();
    }
}