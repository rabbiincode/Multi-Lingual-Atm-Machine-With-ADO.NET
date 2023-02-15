using ConsoleTables;
using ShegeBank.DataBase;
using ShegeBank.Enum;
using ShegeBank.LanguageChoice;
using ShegeBank.Models;
using ShegeBank.UserInterface;
using System.Data.SqlClient;
using System.Transactions;

namespace ShegeBank.Bank;

internal partial class Atm
{
    int transferId;
    long selectedMobileNumber;
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

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT withdrawable_balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            decimal withdrawableBalance = (decimal)command.ExecuteScalar();

            if (transferAmount >= withdrawableBalance)
            {
                Utility.PrintMessage($"{Languages.Display(21)}", false);
                Utility.PressEnterToContinue();
                return;
            }
        }

        startTransfer: long accountNumber = Validate.Convert<long>($"{Languages.Display(31)}");

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT customer_id FROM ATMCARD
                             WHERE account_number == accountNumber";
            try
            {
                SqlCommand command = new(query, connect);
                connect.Open();

                transferId = (int)command.ExecuteScalar();
            }
            catch
            {
                Utility.PrintMessage($"{Languages.Display(32)}", false);
                Thread.Sleep(2000);
                goto startTransfer;
            }

            string myAccountNumber = @"SELECT account_number FROM ATMCARD
                                       WHERE selectedId == customer_id";

            SqlCommand number = new(myAccountNumber, connect);
            connect.Open();

            long myNumber = (int)number.ExecuteScalar();

            if (accountNumber == myNumber)
            {
                Utility.PrintMessage($"{Languages.Display(33)}", false);
                Thread.Sleep(2000);
                goto startTransfer;
            }
        }

        Utility.Loading($"{Languages.Display(34)}", ".", 7, 500);

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT full_name FROM ATMCARD
                             WHERE account_number == accountNumber"
            ;

            SqlCommand name = new(query, connect);
            connect.Open();
            string fullName = (string)name.ExecuteScalar();

            Console.WriteLine($"{Languages.Display(35)} {Utility.FormatCurrency(transferAmount)} {Languages.Display(36)} {fullName}");

            question: int answer = Validate.Convert<int>($"{Languages.Display(37)}");

            if (answer == 2)
                return;

            if (answer <= 0 || answer > 2)
            {
                Utility.PrintMessage($"{Languages.Display(20)}", false);
                goto question;
            }

            Utility.Loading($"{Languages.Display(5)}", ".", 6, 400);
            Utility.PrintMessage($"{Languages.Display(38)} {Utility.FormatCurrency(transferAmount)} {Languages.Display(39)} {fullName} {Languages.Display(40)}", true);
        }

        using (SqlConnection connect = new(connectionString))
        {
            SqlTransaction transaction = connect.BeginTransaction();
            try
            {
                //update senders balance
                string senderUpdate = @"UPDATE ATMCARD
                                        SET balance -= transferAmount 
                                        WHERE selectedId == customer_id";

                SqlCommand sender = new(senderUpdate, connect, transaction);
                connect.Open();
                sender.ExecuteNonQuery();

                //update recievers balance
                string recieverUpdate = @"UPDATE ATMCARD
                                        SET balance += transferAmount 
                                        WHERE selectedId == customer_id";

                SqlCommand receiver = new(recieverUpdate, connect, transaction);
                receiver.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        Utility.PressEnterToContinue();

        //update senders transaction history
        //InsertTransaction(UserData.selectedAccount.Id, $"{Languages.Display(89)}", Utility.FormatCurrency(transferAmount), $"{Languages.Display(52)} {UserData.transferAccount.FullName} {Languages.Display(54)}");

        //update recievers transaction history
        //InsertTransaction(UserData.transferAccount.Id, $"{Languages.Display(87)}", Utility.FormatCurrency(transferAmount), $"{Languages.Display(53)} {UserData.selectedAccount.FullName} {Languages.Display(54)}");
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

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT mobile_number FROM ATMCARD
                             WHERE selectedId == customer_id"
            ;

            SqlCommand number = new(query, connect);
            connect.Open();

            long mobileNumber = (long)number.ExecuteScalar();

            switch (airtimeOption)
            {
                case (int)MobileChoice.Self:
                    selectedMobileNumber = mobileNumber;
                    break;
                case (int)MobileChoice.Others:
                    selectedMobileNumber = Validate.Convert<long>($"{Languages.Display(41)}");
                    break;
                default:
                    Utility.PrintMessage($"{Languages.Display(20)}", false);
                    goto option;
            }
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
        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            decimal balance = (decimal)command.ExecuteScalar();

            if (airtimeAmount >= balance)
            {
                Utility.PrintMessage($"{Languages.Display(21)}", false);
                Utility.PressEnterToContinue();
                return;
            }
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

        if (otherRechargeAmount > 20000)
        {
            Utility.PrintMessage($"{Languages.Display(43)} {Utility.FormatCurrency(20000)} {Languages.Display(44)}", false);
            Thread.Sleep(2000);
            goto startRecharge;
        }

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"SELECT balance FROM ATMCARD
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();

            decimal balance = (decimal)command.ExecuteScalar();

            if (otherRechargeAmount >= balance)
            {
                Utility.PrintMessage($"{Languages.Display(21)}", false);
                Utility.PressEnterToContinue();
                return;
            }
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

        using (SqlConnection connect = new(connectionString))
        {
            string query = @"UPDATE balance FROM ATMCARD
                             SET balance -= amount 
                             WHERE selectedId == customer_id";

            SqlCommand command = new(query, connect);
            connect.Open();
            command.ExecuteNonQuery();
        }

        //InsertTransaction(UserData.selectedAccount.Id, $"{Languages.Display(90)}", Utility.FormatCurrency(rechargeAmount), $"{Languages.Display(55)}");
    }

    public void InsertTransaction(int userBankAccountId, string transactionType, string amount, string description)
    {
        var tracker = new TransactionTracker
        {
            TransactionId = Utility.GenerateTransactionId(),
            UserBankAccountId = userBankAccountId,
            TransactionType = transactionType,
            TransactionAmount = amount,
            TransactionDate = DateTime.Now,
            Description = description
        };
        //UserData.TransactionTrackerList.Add(tracker);
    }

    public void ViewTransaction()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        /*var filteredList = from list in UserData.TransactionTrackerList
                           where list.UserBankAccountId == UserData.selectedAccount.Id
                           select list;

        var count = filteredList.Count();

        if (count > 0)
        {
            ConsoleTable table = new(Languages.Display(56), Languages.Display(57), Languages.Display(58), Languages.Display(59), Languages.Display(60));

            foreach (var display in filteredList)
            {
                table.AddRow(display.TransactionId, display.TransactionDate, display.TransactionType, display.TransactionAmount, display.Description);
            }
            table.Options.EnableCount = false;
            table.Write();

            Utility.PrintMessage($"{Languages.Display(61)} : {count}", true);
        }
        else
            Utility.PrintMessage($"{Languages.Display(62)}", false);

        Utility.PressEnterToContinue();*/
    }
}