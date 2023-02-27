using ConsoleTables;
using ShegeBank.Enum;
using ShegeBank.LanguageChoice;
using ShegeBank.Models;
using ShegeBank.UserInterface;
using System.Data.SqlClient;

namespace ShegeBank.Bank;

internal partial class Atm
{
    static string? receiverFirstName;
    static string? receiverLastName;
    static decimal receiverBalance;
    static long receiverId;
    static long selectedMobileNumber;
    private readonly decimal maximumRechargeAmount = 20000;
    private List<TransactionTracker> TransactionTrackerList = new List<TransactionTracker>();
    public void Transfer()
    {
        ValidateTransfer();
    }

    public void ValidateTransfer()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        transfer: decimal transferAmount = Validate.Convert<decimal>($"{Languages.Display(30)}");

        if (transferAmount <= 0)
        {
            Utility.PrintMessage($"{Languages.Display(23)}", false);
            Thread.Sleep(4000);
            goto transfer;
        }

        if (transferAmount >= (accountBalance - minimumAccountBalance))
        {
            Utility.PrintMessage($"{Languages.Display(21)}", false);
            Utility.PressEnterToContinue();
            return;
        }

        startTransfer: long receiverAccountNumber = Validate.Convert<long>($"{Languages.Display(31)}");

        string query = @$"SELECT Customer_Id, First_Name, Last_Name, Balance FROM customers
                          WHERE Account_Number = {receiverAccountNumber}";

        if (myAccountNumber == receiverAccountNumber)
        {
            Utility.PrintMessage($"{Languages.Display(33)}", false);
            Thread.Sleep(2000);
            goto startTransfer;
        }

        using (SqlConnection connect = new(connectionString))
        {
            using (SqlCommand command = new(query, connect))
            {
                connect.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        receiverId = (long)reader["Customer_Id"];
                        receiverFirstName = (string)reader["First_Name"];
                        receiverLastName = (string)reader["Last_Name"];
                        receiverBalance = (decimal)reader["Balance"];
                    }
                    else
                    {
                        Utility.PrintMessage($"{Languages.Display(32)}", false);
                        Thread.Sleep(2000);
                        goto startTransfer;
                    }
                }
            }
        }

        Utility.Loading($"{Languages.Display(34)}", ".", 7, 500);

        Console.WriteLine($"{Languages.Display(35)} {Utility.FormatCurrency(transferAmount)} {Languages.Display(36)} {receiverFirstName} {receiverLastName}");

        question: int answer = Validate.Convert<int>($"{Languages.Display(37)}");

        if (answer == 2)
            return;

        if (answer <= 0 || answer > 2)
        {
            Utility.PrintMessage($"{Languages.Display(20)}", false);
            goto question;
        }

        Utility.Loading($"{Languages.Display(5)}", ".", 6, 400);

        //update senders and reciever's balance
        string updateBalance = @$"
                                  UPDATE customers SET Balance -= {transferAmount} WHERE Customer_Id = {selectedId};
                                  UPDATE customers SET Balance +''= {transferAmount} WHERE Account_Number = {receiverAccountNumber};";

        //update sender and receiver's transaction history
        string updateSender = @$"INSERT INTO transactionTracker(Customer_Id, Transaction_Type, Transaction_Amount, Transaction_Date, Description) VALUES
                                 ({selectedId}, '{Languages.Display(89)}', {Utility.FormatCurrency(transferAmount)},
                                 CURRENT_TIMESTAMP, '{Languages.Display(52)} {firstName} {lastName} {Languages.Display(54)}')";

        string updateReceiver = @$"INSERT INTO transactionTracker(Customer_Id, Transaction_Type, Transaction_Amount, Transaction_Date, Description) VALUES
                                   ({receiverId}, '{Languages.Display(87)}', {Utility.FormatCurrency(transferAmount)}, CURRENT_TIMESTAMP,
                                   '{Languages.Display(53)} {receiverFirstName} {receiverLastName} {Languages.Display(54)}')";

        using (SqlConnection connect = new(connectionString))
        {
            connect.Open();
            SqlTransaction transaction = connect.BeginTransaction();

            try
            {
                using (SqlCommand command = new(updateBalance, connect, transaction))
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                Utility.PrintMessage($"{Languages.Display(38)} {Utility.FormatCurrency(transferAmount)} {Languages.Display(39)} {receiverFirstName} {receiverLastName} {Languages.Display(40)}", true);
            }
            catch
            {
                transaction.Rollback();
                Utility.PrintMessage($"{Languages.Display(91)}", false);
                return;
            }

            using (SqlCommand command = new(updateSender, connect))
            {
                command.ExecuteNonQuery();
            }

            using (SqlCommand command = new(updateReceiver, connect))
            {
                command.ExecuteNonQuery();
            }
        }
        Utility.PressEnterToContinue();
    }
    public void Airtime()
    {
        MobileNumberChoice();
        UserScreen.AirtimeOption();
        ValidateAirtime();
    }

    public void MobileNumberChoice()
    {
        UserScreen.RechargeChoice();
        option: int airtimeOption = Validate.Convert<int>($"{Languages.Display(19)}");

        switch (airtimeOption)
        {
            case (int)MobileChoice.Self:
                selectedMobileNumber = myMobileNumber;
                break;
            case (int)MobileChoice.Others:
                selectedMobileNumber = Validate.Convert<long>($"{Languages.Display(41)}");
                break;
            default:
                Utility.PrintMessage($"{Languages.Display(20)}", false);
                goto option;
        }
    }
    public void ValidateAirtime()
    {
        option: int airtimeOption = Validate.Convert<int>($"{Languages.Display(19)}");

        switch (airtimeOption)
        {
            case (int)Recharge.TwoHundred:
                OptionAirtime(200);
                break;
            case (int)Recharge.FiveHundred:
                OptionAirtime(500);
                break;
            case (int)Recharge.OneThousand:
                OptionAirtime(1000);
                break;
            case (int)Recharge.TwoThousand:
                OptionAirtime(2000);
                break;
            case (int)Recharge.Others:
                OtherAirtime();
                break;
            default:
                goto option;
        }
    }

    public void OptionAirtime(decimal airtimeAmount)
    {
        if (airtimeAmount >= accountBalance)
        {
            Utility.PrintMessage($"{Languages.Display(21)}", false);
            Utility.PressEnterToContinue();
            return;
        }
        RechargeMessage(airtimeAmount);
    }
    public void OtherAirtime()
    {
        startRecharge: int otherRechargeAmount = Validate.Convert<int>($"{Languages.Display(42)}");

        if (otherRechargeAmount <= 0)
        {
            Utility.PrintMessage($"{Languages.Display(23)}", false);
            Thread.Sleep(2000);
            goto startRecharge;
        }

        if (otherRechargeAmount > maximumRechargeAmount)
        {
            Utility.PrintMessage($"{Languages.Display(43)} {Utility.FormatCurrency(maximumRechargeAmount)} {Languages.Display(44)}", false);
            Thread.Sleep(2000);
            goto startRecharge;
        }

        if (otherRechargeAmount >= accountBalance)
        {
            Utility.PrintMessage($"{Languages.Display(21)}", false);
            Utility.PressEnterToContinue();
            return;
        }

        RechargeMessage(otherRechargeAmount);
    }

    public void RechargeMessage(decimal amount)
    {
        Utility.Loading($"{Languages.Display(5)}", ".", 6, 500);

        Console.WriteLine($"{Languages.Display(45)} : {selectedMobileNumber} {Languages.Display(46)} {Utility.FormatCurrency(amount)}");
        Thread.Sleep(3000);

        question: int answer = Validate.Convert<int>($"{Languages.Display(37)}");

        if (answer == 2)
            return;

        if (answer <= 0 || answer > 2)
        {
            Utility.PrintMessage($"{Languages.Display(20)}", false);
            goto question;
        }

        Utility.Loading("", "", 5, 500);
        Utility.PrintMessage($"{Languages.Display(47)} : {selectedMobileNumber} {Languages.Display(48)} {Utility.FormatCurrency(amount)} {Languages.Display(49)}", true);
        Utility.PressEnterToContinue();

        string update = @$"UPDATE customers SET Balance -= {amount} WHERE customer_id = {selectedId}";

        string insert = @$"INSERT INTO transactionTracker(Customer_Id, Transaction_Type, Transaction_Amount, Transaction_Date, Description) VALUES
                           ({selectedId}, '{Languages.Display(90)}', {Utility.FormatCurrency(amount)}, CURRENT_TIMESTAMP, '{Languages.Display(55)}')";
        
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

    public void InsertTransaction()
    {
        string getId = @$"SELECT * FROM transactionTracker
                          WHERE Customer_Id = {selectedId}";

        using (SqlConnection connect = new(connectionString))
        {
            using (SqlCommand command = new(getId, connect))
            {
                connect.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var transactionList = new TransactionTracker
                        {
                            TransactionId = (long)reader["Transaction_Id"],
                            UserBankAccountId = (long)reader["Customer_Id"],
                            TransactionType = (string)reader["Transaction_Type"],
                            TransactionAmount = (decimal)reader["Transaction_Amount"],
                            TransactionDate = (DateTime)reader["Transaction_Date"],
                            Description = (string)reader["Description"]
                        };
                        TransactionTrackerList.Add(transactionList);
                    }
                    else                    
                        Utility.PrintMessage($"{Languages.Display(62)}", false);
                }
            }
        }
    }

    public void ViewTransaction()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;

        InsertTransaction();

        if (TransactionTrackerList?.Count > 0)
        {
            ConsoleTable table = new(Languages.Display(56), Languages.Display(57), Languages.Display(58), Languages.Display(59), Languages.Display(60));

            foreach (var display in TransactionTrackerList)
            {
                table.AddRow(display.TransactionId, display.TransactionDate, display.TransactionType, display.TransactionAmount, display.Description);
            }
            table.Options.EnableCount = false;
            table.Write();

            Utility.PrintMessage($"{Languages.Display(61)} : {TransactionTrackerList?.Count}", true);
        }

        Utility.PressEnterToContinue();
    }
}