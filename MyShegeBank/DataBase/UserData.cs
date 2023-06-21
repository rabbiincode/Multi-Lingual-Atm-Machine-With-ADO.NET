using System.Configuration;
using System.Data.SqlClient;

namespace ShegeBank.DataBase;

internal class UserData
{
    string connectionString = ConfigurationManager.ConnectionStrings["DATA"].ConnectionString;

    string branch = @"INSERT INTO branch VALUES
                            ('Cooperate Office'), ('Nig Road'), ('Air Close'), ('Reserve Avenue'), ('City State'),
                            ('Village Road'), ('Shear Avenue'), ('Federal Road'), ('Daner Street'), ('First Street');";

    string customer = @"INSERT INTO customers VALUES
                              ('Adam', 'Gray', NULL, 'M', 2234567890, 0808675732, '90 Vic Cresent', 15000, 987654, 4444, 0, 'false', 2),
                              ('John', 'Len', 'Santos', 'M', 2489975191, 0705648347, '49 National Rd', 180000, 123456, 3333, 0, 'false', 5),
                              ('Sarah', 'White', 'Pearl', 'F', 2805598895, 0805456789, '1 Port Avenue', 2000, 227455, 5555, 0, 'false', 5),
                              ('Don', 'Bill', NULL, 'M', 2690454433, 0812356742, '5 Shade Close', 5000, 667902, 7777, 0, 'false', 7),
                              ('Jane', 'Louis', 'Matt', 'F', 2678532354, 0905958877, '100 Visit Estate', 107000, 654321, 2222, 0, 'false', 1),
                              ('Abel', 'Mark', 'Bright', 'M', 2385383558, 0809005837, '50 Park Area', 10700, 185477, 7809, 0, 'false', 8),
                              ('Lyna', 'Voine', 'Jane', 'F', 2690784321, 0708856334, '9 Vicent Close', 1007000, 873200, 3341, 0, 'false', 9),
                              ('Pearl', 'Wright', NULL, 'F', 2590687544, 0812350867, '70 Main Street', 10900, 671063, 7777, 0, 'false', 2),
                              ('Vicent', 'Tryle', 'Boss', 'M', 2400093255, 0904876631, '4 Grid Layout', 7000, 598991, 8889, 0, 'false', 7),
                              ('Robin', 'Hinett', 'Voice', 'M', 2311098455, 0914560832, '29 Busy Street Layout', 170000, 111660, 3059, 0, 'false', 3);";
    public async Task InsertTableDataAsync()
    {
        try
        {
            using (SqlConnection connect = new(connectionString))
            {
                connect.Open();
                using (SqlCommand insertBranch = new(branch, connect))
                {
                    await insertBranch.ExecuteNonQueryAsync();
                }
                using (SqlCommand insertCustomer = new(customer, connect))
                {
                    await insertCustomer.ExecuteNonQueryAsync();
                }  
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error: {0}", e);
        }
    }
}