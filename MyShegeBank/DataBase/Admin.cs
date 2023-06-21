using System.Configuration;
using System.Data.SqlClient;

namespace MyShegeBank.DataBase;

internal class Admin
{
    string connectionString = ConfigurationManager.ConnectionStrings["DATA"].ConnectionString;
    readonly string database = "CREATE DATABASE myShegeBank";

    readonly string branch = @"CREATE TABLE branch(
                                  Branch_Id INT PRIMARY KEY IDENTITY(1,1),
                                  Branch_Address VARCHAR(50) NOT NULL,
                               )";

    readonly string customer = @"CREATE TABLE customers(
                                    Customer_Id BIGINT PRIMARY KEY IDENTITY(1,1),
                                    First_Name VARCHAR(30) NOT NULL,
                                    Last_Name VARCHAR(30) NOT NULL,
                                    Middle_Name VARCHAR(30),
                                    Sex VARCHAR(1) NOT NULL,
                                    Account_Number BIGINT UNIQUE,
                                    Mobile_Number BIGINT NOT NULL,
                                    Address VARCHAR(50) NOT NULL,
                                    Balance DECIMAL(18,2) NOT NULL,
                                    Card_Number BIGINT UNIQUE,
                                    Card_Pin INT,
                                    Total_Login INT NOT NULL,
                                    Is_Locked VARCHAR(5) NOT NULL,
                                    Account_Branch_Id INT,
                                    FOREIGN KEY (Account_Branch_Id) REFERENCES branch(Branch_Id) ON DELETE SET NULL
                                 )";

    readonly string transactionsTracker = @"CREATE TABLE transactionTracker(
                                               Transaction_Id BIGINT PRIMARY KEY IDENTITY(1,1),
                                               Customer_Id BIGINT,
                                               Transaction_Type VARCHAR(20) NOT NULL,
                                               Transaction_Amount DECIMAL,
                                               Transaction_Date DATETIME DEFAULT CURRENT_TIMESTAMP,
                                               Description VARCHAR(50) NOT NULL,
                                               FOREIGN KEY (Customer_Id) REFERENCES customers(Customer_Id) ON DELETE SET NULL
                                            )";
    public async Task CreateDatabaseAndTablesAsync()
    {
        using (SqlConnection connect = new(connectionString))
        {
            try
            {
                connect.Open();
              /*  using (SqlCommand createDatabase = new(database, connect))
                {
                    await createDatabase.ExecuteNonQueryAsync();
                }*/
                using (SqlCommand createBranch = new(branch, connect))
                {
                    createBranch.ExecuteNonQuery();
                }
                using (SqlCommand createCustomer = new(customer, connect))
                {
                    await createCustomer.ExecuteNonQueryAsync();
                }
                using (SqlCommand createTransactionsTracker = new(transactionsTracker, connect))
                {
                    await createTransactionsTracker.ExecuteNonQueryAsync();
                }
            }
            catch
            {
                Console.WriteLine("Database and Tables already Exists");
            }
        }
    }
}