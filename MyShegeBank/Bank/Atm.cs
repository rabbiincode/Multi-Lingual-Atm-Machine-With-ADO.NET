﻿using ShegeBank.Bank.AtmFunctionality;
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
    static long selectedId;
    static string? locked;
    static int cardPin;
    static string? firstName;
    static string? lastName;
    static int totalLogin;
    static long myMobileNumber;
    static long myAccountNumber;
    static decimal accountBalance;

    private readonly decimal maximumWithdrawalAmount = 20000;
    private readonly decimal minimumAccountBalance = 500;
    public void ValidateCardNumberAndPassword()
    {
        bool login = false;
        long cardNumber = Validate.Convert<long>("Enter your card number[Insert your ATM Card]");

        string getData = @$"SELECT Customer_Id, Is_Locked, Card_Pin, Total_Login, First_Name, Last_Name, Mobile_Number,
                          Account_Number, Balance FROM customers WHERE Card_Number = {cardNumber}";

        string update = @$"
                           UPDATE customers SET Total_Login = {totalLogin} WHERE Customer_Id = {selectedId};
                           UPDATE customers SET Is_Locked = 'true' WHERE Customer_Id = {selectedId};";

        while (login == false)
        {
            Utility.Loading("Please wait..[Abeg wait]..[Biko chere]", ".", 6, 500);

            using (SqlConnection connect = new(connectionString))
            {
                using (SqlCommand command = new(getData, connect))
                {
                    connect.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            locked = (string)reader["Is_Locked"];
                            selectedId = (long)reader["Customer_Id"];
                            cardPin = (int)reader["Card_Pin"];
                            totalLogin = (int)reader["Total_Login"];
                            firstName = (string)reader["First_Name"];
                            lastName = (string)reader["Last_Name"];
                            myMobileNumber = (long)reader["Mobile_Number"];
                            myAccountNumber = (long)reader["Account_Number"];
                            accountBalance = (decimal)reader["Balance"];
                        }
                        else
                        {
                            Utility.PrintMessage("Your ATM card is invalid..[ATM card no dey valid]..[ATM anabataro card gi]", false);
                            Thread.Sleep(4000);
                            Pick.Cancel();
                            login = true;
                            break;
                        }
                    }
                }

                if (locked == "true")
                {
                    UserScreen.LockedAccount();
                    login = true;
                    break;
                }

                Languages.ValidateLanguageChoice();

                inputPin: int pin = Utility.GetUserPin($"{Languages.Display(4)}");

                if (pin == cardPin)
                {
                    Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);

                    Utility.PrintMessage($"Hello {firstName} {lastName}, welcome back...[Nnoo]", true);
                    Thread.Sleep(2000);
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
                        using (SqlCommand command = new(update, connect))
                        {
                            command.ExecuteNonQuery();
                        }
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
        Console.ForegroundColor = ConsoleColor.Blue;
        Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);

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

        string update = @$"UPDATE customers SET Balance += {depositAmount} WHERE Customer_Id = {selectedId}";

        string insert = @$"INSERT INTO transactionTracker(Customer_Id, Transaction_Type, Transaction_Amount, Transaction_Date, Description) VALUES
                           ({selectedId}, '{Languages.Display(87)}', {Utility.FormatCurrency(depositAmount)}, CURRENT_TIMESTAMP, '{Languages.Display(50)}')";

        using (SqlConnection connect = new(connectionString))
        {
            connect.Open();
            using (SqlCommand command = new(update, connect))
            {
                command.ExecuteNonQuery();
            }

            using (SqlCommand command = new(insert, connect))
            {
                command.ExecuteNonQuery();
            }
        }

        Utility.PressEnterToContinue();
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
        if (withdrawalAmount >= (accountBalance - minimumAccountBalance))
        {
            Utility.PrintMessage($"{Languages.Display(21)}", false);
            Utility.PressEnterToContinue();
            return;
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

        if (otherWithdrawalAmount >= (accountBalance - minimumAccountBalance))
        {
            Utility.PrintMessage($"{Languages.Display(21)}", false);
            Utility.PressEnterToContinue();
            return;
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

        string update = @$"UPDATE customers SET Balance -= {amount} WHERE Customer_Id = {selectedId}";

        string insert = $@"INSERT INTO transactionTracker(Customer_Id, Transaction_Type, Transaction_Amount, Transaction_Date, Description) VALUES
                           ({selectedId}, '{Languages.Display(88)}', {Utility.FormatCurrency(amount)}, CURRENT_TIMESTAMP, '{Languages.Display(51)}');";

        using (SqlConnection connect = new(connectionString))
        {
            connect.Open();
            using (SqlCommand command = new(update, connect))
            {
                command.ExecuteNonQuery();
            }

            using (SqlCommand command = new(insert, connect))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}