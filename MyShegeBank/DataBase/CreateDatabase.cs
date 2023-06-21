using ShegeBank.DataBase;

namespace MyShegeBank.DataBase;

internal class CreateDatabase
{
    Admin admin = new();
    UserData userData = new();

    public async Task CreateDataBaseAsync()
    {
        await admin.CreateDatabaseAndTablesAsync();
        await userData.InsertTableDataAsync();
    }
}