using System.Configuration;
using System.Data.SqlClient;

namespace ShegeBank.DataBase;

internal class UserData
{
    string connectionString = ConfigurationManager.ConnectionStrings["DATA"].ConnectionString;
    public void InsertCustomer()
    {
        string create = @"INSERT INTO CUSTOMERS VALUES(1, 'Adam','Gray', NULL, 'M', 2234567890, 0808675732, '90 Vic Cresent', 15000, 2);
                          INSERT INTO CUSTOMERS VALUES(2, 'John','Len', 'Santos', 'M', 2489975191, 0705648347, '49 National Rd', 180000, 5);
                          INSERT INTO CUSTOMERS VALUES(3, 'Sarah','White', 'Pearl', 'F', 2805598895, 0805456789, '1 Port Avenue', 2000, 5);
                          INSERT INTO CUSTOMERS VALUES(4, 'Don','Bill', NULL, 'M', 2690454433, 0812356742, '5 Shade Close', 5000, 7);
                          INSERT INTO CUSTOMERS VALUES(5, 'Jane','Louis', 'Matt', 'F', 2678532354, 0905958877, '100 Visit Estate', 107000, 1)";

        try
        {
            using (SqlConnection connect = new(connectionString))
            {
                SqlCommand command = new(create, connect);
                connect.Open();
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            Console.WriteLine("Data Already Exists");
        }
    }

    public void InsertBankBranch()
    {
        string create = @"INSERT INTO BRANCH VALUES(1, 'Cooperate Office');
                          INSERT INTO BRANCH VALUES(2, 'Nig Road');
                          INSERT INTO BRANCH VALUES(3, 'Air Close');
                          INSERT INTO BRANCH VALUES(4, 'Reserve Avenue');
                          INSERT INTO BRANCH VALUES(5, 'City State');
                          INSERT INTO BRANCH VALUES(6, 'Village Road');
                          INSERT INTO BRANCH VALUES(7, 'Shear Avenue');
                          INSERT INTO BRANCH VALUES(8, 'Federal Road');
                          INSERT INTO BRANCH VALUES(9, 'Daner Street');
                          INSERT INTO BRANCH VALUES(10, 'First Street')";

        try
        {
            using (SqlConnection connect = new(connectionString))
            {
                SqlCommand command = new(create, connect);
                connect.Open();
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            Console.WriteLine("Data Already Exists");
        }
    }

    
    public void InsertAtmCard()
    {
        string create = @"INSERT INTO ATMCARD VALUES(1, 'Adam Gray', 2234567890, 987654, 4444, 0808675732, 15000, 15000, 'false');
                          INSERT INTO ATMCARD VALUES(2, 'John Len', 2489975191, 123456, 3333, 0705648347, 180000, 180000, 'false');
                          INSERT INTO ATMCARD VALUES(3, 'Sarah White', 2805598895, 227455, 5555, 0805456789, 2000, 2000,  'false');
                          INSERT INTO ATMCARD VALUES(4, 'Don Bill', 2690454433, 667902, 7777, 0812356742, 5000, 5000, 'false');
                          INSERT INTO ATMCARD VALUES(5, 'Jane Louis', 2678532354, 654321, 2222, 0905958877, 107000, 107000, 'false')";

        using (SqlConnection connect = new(connectionString))
        {
            SqlCommand command = new(create, connect);
            connect.Open();
            command.ExecuteNonQuery();
        }
    }

    public void InsertData()
    {
            InsertBankBranch();
            InsertCustomer();
            InsertAtmCard();
    }

}