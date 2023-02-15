using ShegeBank.DataBase;

namespace MyShegeBank.DataBase
{
    internal class CreateDatabase
    {
        Admin admin = new();
        UserData userData = new();

        public void CreateDataBase()
        {
            admin.CreateTables();
            userData.InsertData();
        }

    }
}
