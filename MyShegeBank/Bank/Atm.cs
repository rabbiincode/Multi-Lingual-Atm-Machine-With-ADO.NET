using ShegeBank.Bank.AtmFunctionality;
using ShegeBank.Enum;
using ShegeBank.Interfaces;
using ShegeBank.LanguageChoice;
using ShegeBank.UserInterface;
using System.Configuration;
using System.Data.SqlClient;

namespace ShegeBank.Bank;

internal partial class Atm : IUserLogin, IUserMainOptions, ITrackTransaction
{
    string connectionString = ConfigurationManager.ConnectionStrings["DATA"].ConnectionString;
    int selectedId;
    int totalLogin;

    private decimal maximumWithdrawalAmount = 40000;
    public void ValidateCardNumberAndPassword()
    {
        bool login = false;
        long cardNumber = Validate.Convert<long>("Enter your card number[Insert your ATM Card]");

        while (login == false)
        {
            Utility.Loading("Please wait..[Abeg wait]..[Biko chere]", ".", 6, 500);

            using (SqlConnection connect = new(connectionString))
            {
                string query = @"SELECT customer_id FROM ATMCARD
                                 WHERE card_number = cardNumber";
                /*try
                {*/
                    SqlCommand command = new(query, connect);
                    connect.Open();

                    selectedId = (int)command.ExecuteScalar();
                //}
                /*catch
                {
                    Utility.PrintMessage("Your ATM card is invalid..[ATM card no dey valid]..[ATM anabataro card gi]", false);
                    Thread.Sleep(4000);
                    Pick.Cancel();
                    login = true;
                    break;
                }*/

            }

            using (SqlConnection connect = new(connectionString))
            {
                string query = @"SELECT is_locked FROM ATMCARD
                                 WHERE selectedId == customer_id";

                SqlCommand command = new(query, connect);
                connect.Open();

                bool isLocked = (bool)command.ExecuteScalar();

                if (isLocked == true)
                {
                    UserScreen.LockedAccount();
                    login = true;
                    break;
                }

            }

            Languages.ValidateLanguageChoice();

            inputPin: int pin = Utility.GetUserPin($"{Languages.Display(4)}");

            using (SqlConnection connect = new(connectionString))
            {
                string query = @"SELECT card_pin FROM ATMCARD
                                 WHERE selectedId == customer_id";

                SqlCommand command = new(query, connect);
                connect.Open();

                int cardPin = (int)command.ExecuteScalar();

                string getFullName = @"SELECT full_name FROM ATMCARD
                                       WHERE selectedId == customer_id";

                SqlCommand name = new(getFullName, connect);
                connect.Open();
                string fullName = (string)command.ExecuteScalar();

                if (pin == cardPin)
                {
                    Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);

                    Utility.PrintMessage($"Hello {fullName}, welcome back...[Nnoo]", true);
                    Thread.Sleep(2000);
                    totalLogin = 0;
                    login = true;
                    break;
                }
                else
                {
                    Utility.PrintMessage($"\n{Languages.Display(6)}", false);
                    totalLogin++;

                    if (totalLogin == 2)
                    {
                        Utility.PrintMessage($"{Languages.Display(7)}", false);
                        Thread.Sleep(4000);
                    }

                    if (totalLogin == 3)
                    {
                        Thread.Sleep(2000);
                        string update = @"UPDATE ATMCARD
                                          SET is_locked == true
                                          WHERE selectedId == customer_id";

                        SqlCommand updated = new(update, connect);

                        command.ExecuteNonQuery();
                        UserScreen.LockAccount();
                        login = true;
                        break;
                    }
                    goto inputPin;
                }
            }
        }
    }
    public void CheckBalance()
    {
        decimal accountBalance;
        Console.ForegroundColor = ConsoleColor.Blue;
        Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            accountBalance = (decimal)command.ExecuteScalar();
        }

        Console.WriteLine($"| {Languages.Display(8)} : {Utility.FormatCurrency(accountBalance)} |");
        Utility.PressEnterToContinue();
    }
    public void Deposit()
    {
        ValidateDeposit();
    }

    public void ValidateDeposit()
    {
        Console.Clear();
        startDeposit: Utility.PrintMessage($"{Languages.Display(9)}", false);
        Utility.PressEnterToContinue();
        decimal depositAmount = Validate.Convert<decimal>($"{Languages.Display(10)}");

        if (depositAmount % 500 != 0 || depositAmount == 0)
            goto startDeposit;

        decimal thousandCount = depositAmount / 1000;
        decimal hundredCount = (depositAmount % 1000) / 500;

        Utility.PrintMessage($"{Languages.Display(11)}", false);
        Utility.PressEnterToContinue();

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Utility.Loading($"{Languages.Display(12)}", "<<", 7, 800);
        Utility.Loading($"{Languages.Display(13)}", ".", 6, 500);
        Utility.Loading($"{Languages.Display(14)}", ".", 7, 600);


        Console.WriteLine($"------------------{Languages.Display(15)}------------------");
        if (depositAmount == 500)
        {
            Console.WriteLine($">500 X {hundredCount} = {Utility.FormatCurrency(500 * hundredCount)}");
        }
        else if (depositAmount % 1000 == 0)
        {
            Console.WriteLine($">1000 X {thousandCount} = {Utility.FormatCurrency(depositAmount)}");
        }
        else
        {
            Console.WriteLine($">1000 X {(int)thousandCount} = {Utility.FormatCurrency(1000 * (int)thousandCount)}");
            Console.WriteLine($">500 X {hundredCount} = {Utility.FormatCurrency(500 * hundredCount)}");
        }
        Console.WriteLine($"\n{Languages.Display(16)} : {Utility.FormatCurrency(depositAmount)}");
        Console.WriteLine("---------------------------------------------------");

        Utility.PressEnterToContinue();

        Utility.PrintMessage($"{Languages.Display(17)} {Utility.FormatCurrency(depositAmount)} {Languages.Display(18)}", true);

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"UPDATE ATMCARD
                             SET balance += depositAmount 
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();
            command.ExecuteNonQuery();
        }

        Utility.PressEnterToContinue();

        //InsertTransaction(UserData.selectedAccount.Id, $"{Languages.Display(87)}", Utility.FormatCurrency(depositAmount), $"{Languages.Display(50)}");
    }
    public void Withdrawal()
    {
        UserScreen.WithdrawalOption();
        ValidateWithdrawal();
    }

    public void ValidateWithdrawal()
    {
        option: int withdrawalOption = Validate.Convert<int>($"{Languages.Display(19)}");

        switch (withdrawalOption)
        {
            case (int)Withdraw.FiveHundred:
                OptionWithdrawal(500);
                break;
            case (int)Withdraw.OneThousand:
                OptionWithdrawal(1000);
                break;
            case (int)Withdraw.TwoThousand:
                OptionWithdrawal(2000);
                break;
            case (int)Withdraw.FiveThousand:
                OptionWithdrawal(5000);
                break;
            case (int)Withdraw.TenThousand:
                OptionWithdrawal(10000);
                break;
            case (int)Withdraw.TwentyThousand:
                OptionWithdrawal(20000);
                break;
            case (int)Withdraw.Others:
                OtherWithdrawal();
                break;
            default:
                Utility.PrintMessage($"{Languages.Display(20)}", false);
                goto option;
        }
    }
    public void OptionWithdrawal(decimal withdrawalAmount)
    {
        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT withdrawable_balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            decimal withdrawableBalance = (decimal)command.ExecuteScalar();

            if (withdrawalAmount >= withdrawableBalance)
            {
                Utility.PrintMessage($"{Languages.Display(21)}", false);
                Utility.PressEnterToContinue();
                return;
            }
        }

        WithdrawalMessage(withdrawalAmount);
    }
    public void OtherWithdrawal()
    {
        startWithdrawal: int otherWithdrawalAmount = Validate.Convert<int>($"{Languages.Display(22)}");

        if (otherWithdrawalAmount <= 0)
        {
            Utility.PrintMessage($"{Languages.Display(23)}", false);
            goto startWithdrawal;
        }

        if (otherWithdrawalAmount % 500 != 0)
        {
            Utility.PrintMessage($"{Languages.Display(24)}", false);
            goto startWithdrawal;
        }

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT withdrawable_balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            decimal withdrawableBalance = (decimal)command.ExecuteScalar();

            if (otherWithdrawalAmount >= withdrawableBalance)
            {
                Utility.PrintMessage($"{Languages.Display(21)}", false);
                Utility.PressEnterToContinue();
                return;
            }
        }

        if (otherWithdrawalAmount > maximumWithdrawalAmount)
        {
            Utility.PrintMessage($"{Languages.Display(25)} {Utility.FormatCurrency(maximumWithdrawalAmount)}", false);
            goto startWithdrawal;
        }

        WithdrawalMessage(otherWithdrawalAmount);
    }

    public void WithdrawalMessage(decimal amount)
    {
        Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);
        Utility.Loading($"...........{Languages.Display(26)}...........", "", 6, 400);

        Utility.PrintMessage($"{Languages.Display(27)} {Utility.FormatCurrency(amount)} {Languages.Display(28)}", true);
        Utility.Loading("","", 5, 500);
        Utility.PrintMessage($"{Languages.Display(29)}", true);
        Thread.Sleep(4000);

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"UPDATE ATMCARD
                             SET balance -= amount 
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();
            command.ExecuteNonQuery();
        }

        //InsertTransaction(UserData.selectedAccount.Id, $"{Languages.Display(88)}", Utility.FormatCurrency(amount), $"{Languages.Display(51)}");
    }
} 