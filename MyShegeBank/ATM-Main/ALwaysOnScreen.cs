using MyShegeBank.DataBase;
using ShegeBank.Bank;
using ShegeBank.UserInterface;

namespace ShegeBank.ATM_Main;

internal class ALwaysOnScreen
{
    internal static void Display()
    {
        Atm atm = new();
        CreateDatabase database = new();
        database.CreateDataBase();
        UserScreen.Welcome();
        atm.ValidateCardNumberAndPassword();
        UserScreen.DisplayAtmMenu();
    }
}