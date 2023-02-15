using System.Configuration;
using System.Data.SqlClient;

namespace MyShegeBank.DataBase;

internal class Admin
{
    string connectionString = ConfigurationManager.ConnectionStrings["DATA"].ConnectionString;

    private void CreateDatabase()
    {
        string query = "CREATE DATABASE MYSHEGEBANK";

        using (SqlConnection connect = new(connectionString))
        {
            try
            {
                connect.Open();
                SqlCommand command = new(query, connect);
                command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Database Already Exists");
            }
        }

    }
    public void CreateBankBranch()
    {
        string query = @"CREATE TABLE BRANCH(
                            Branch_Id INT,
                            Branch_Address VARCHAR(50) NOT NULL,
                            PRIMARY KEY(Branch_Id)
                       )";

        using (SqlConnection connect = new(connectionString))
        {
            try
            {
                connect.Open();
                SqlCommand command = new(query, connect);
                command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table Already Exists");
            }
        }
    }

    public void CreateCustomersTable()
    {
        string query = @"CREATE TABLE CUSTOMERS(
                            Customer_Id INT,
                            First_Name VARCHAR(20) NOT NULL,
                            Last_Name VARCHAR(20) NOT NULL,
                            Middle_Name VARCHAR(20),
                            Sex VARCHAR(1),
                            Account_Number BIGINT UNIQUE,
                            Mobile_Number BIGINT,
                            Address VARCHAR(50),
                            Balance DECIMAL,
                            Account_Branch_Id INT,
                            PRIMARY KEY(Customer_Id),
                            FOREIGN KEY (Account_Branch_Id) REFERENCES BRANCH(Branch_Id) ON DELETE SET NULL
                        )";

        using (SqlConnection connect = new(connectionString))
        {
            try
            {
                connect.Open();
                SqlCommand command = new(query, connect);
                command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table Already Exists");
            }
        }
    }
    //MAXIMUN WITHDRAWABLE BALANCE - update check balance
    public void CreateAtm()
    {
        string query = @"CREATE TABLE ATMCARD(
                            Customer_Id INT,
                            Full_Name VARCHAR(50) NOT NULL,
                            Account_Number BIGINT,
                            Card_Number BIGINT,
                            Card_Pin INT,
                            Mobile_Number BIGINT,
                            Balance DECIMAL,
                            Withdrawable_Balance DECIMAL,
                            Is_Locked VARCHAR(5),
                            PRIMARY KEY(Customer_Id),
                            FOREIGN KEY (Customer_Id) REFERENCES CUSTOMERS(Customer_Id) ON DELETE CASCADE,
                       )";

        using (SqlConnection connect = new(connectionString))
        {
            try
            {
                connect.Open();
                SqlCommand command = new(query, connect);
                command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table Already Exists");
            }
        }
    }

    public void CreateTables()
    {
            CreateBankBranch();
            CreateCustomersTable();
            CreateAtm();

    }
}