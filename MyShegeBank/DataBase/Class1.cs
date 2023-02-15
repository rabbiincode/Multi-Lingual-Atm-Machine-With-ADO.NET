namespace MyShegeBank.DataBase
{
    internal class Class1
    {
        /*
         DataTable custTable = new DataTable("Customers");
            DataColumn Column = new DataColumn();
            DataRow Row;

            // Create Id column 
            Column.DataType = typeof(Int32);
            Column.ColumnName = "Id";
            Column.Caption = "Cust ID";
            Column.ReadOnly = true;
            Column.Unique = true;
            //Add column to the DataColumnCollection.
            custTable.Columns.Add(Column);

            // Create FirstName column.
            Column.DataType = typeof(String);
            Column.ColumnName = "First_Name";
            Column.Caption = "FirstName";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create LastName column.
            Column.DataType = typeof(String);
            Column.ColumnName = "Last_Name";
            Column.Caption = "LastName";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create OtherName column.
            Column.DataType = typeof(String);
            Column.ColumnName = "Other_Name";
            Column.Caption = "OtherName";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create Gender column.
            Column.DataType = typeof(Char);
            Column.ColumnName = "SEX";
            Column.Caption = "Sex";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create Address column.
            Column = new DataColumn();
            Column.DataType = typeof(String);
            Column.ColumnName = "Address";
            Column.Caption = "Address";
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create AcccountNumber column 
            Column.DataType = typeof(long);
            Column.ColumnName = "Account_Number";
            Column.Caption = "AccountNumber";
            Column.AutoIncrement = false;
            Column.ReadOnly = true;
            Column.Unique = true;
            custTable.Columns.Add(Column);

            // Create AccountType column.
            Column.DataType = typeof(String);
            Column.ColumnName = "Account_Type";
            Column.Caption = "AccountType";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create AcccountBalance column 
            Column.DataType = typeof(decimal);
            Column.ColumnName = "Account_Balance";
            Column.Caption = "AccountBalance";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Create MobileNumber column 
            Column.DataType = typeof(long);
            Column.ColumnName = "Mobile_Number";
            Column.Caption = "MobileNumber";
            Column.AutoIncrement = false;
            Column.ReadOnly = false;
            Column.Unique = false;
            custTable.Columns.Add(Column);

            // Make id column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = custTable.Columns["Id"];
            custTable.PrimaryKey = PrimaryKeyColumns;

            // Create a new DataSet
            DataSet dataSet = new();

            // Add custTable to the DataSet.
            dataSet.Tables.Add(custTable);

            // Add data rows to the custTable using NewRow method
            // I add three customers with their addresses, names and ids

            connect.Open();
         */

        /*
              DataTable branch = new DataTable("BRANCH");
          DataColumn Column = new DataColumn();

          // Create Id column 
          Column.DataType = typeof(Int32);
          Column.ColumnName = "Id";
          Column.Caption = "CustId";
          Column.ReadOnly = true;
          Column.Unique = true;
          branch.Columns.Add(Column);

          // Create BranchLocation column.
          Column.DataType = typeof(String);
          Column.ColumnName = "Location_Address";
          Column.Caption = "LocationAddress";
          Column.AutoIncrement = false;
          Column.ReadOnly = false;
          Column.Unique = false;
          branch.Columns.Add(Column);

          // Make id column the primary key column.
          DataColumn[] PrimaryKeyColumns = new DataColumn[1];
          PrimaryKeyColumns[0] = branch.Columns["Id"];
          branch.PrimaryKey = PrimaryKeyColumns;
         */

        /*
              DataTable atmCard = new DataTable("ATMCARD");
          DataColumn Column = new DataColumn();

          // Create FullName column 
          Column.DataType = typeof(string);
          Column.ColumnName = "Full_Name";
          Column.Caption = "FullName";
          Column.AutoIncrement = false;
          Column.ReadOnly = false;
          Column.Unique = false;
          atmCard.Columns.Add(Column);

          // Create CardNumber column 
          Column.DataType = typeof(long);
          Column.ColumnName = "Account_Number";
          Column.Caption = "AccountNumber";
          Column.AutoIncrement = false;
          Column.ReadOnly = false;
          Column.Unique = true;
          atmCard.Columns.Add(Column);

          // Create CardPin column 
          Column.DataType = typeof(long);
          Column.ColumnName = "Card_Pin";
          Column.Caption = "CardPin";
          Column.AutoIncrement = false;
          Column.ReadOnly = false;
          Column.Unique = false;
          atmCard.Columns.Add(Column);

          // Create IsLocked column 
          Column.DataType = typeof(bool);
          Column.ColumnName = "Is_Locked";
          Column.Caption = "IsLocked";
          Column.AutoIncrement = false;
          Column.ReadOnly = false;
          Column.Unique = false;
          atmCard.Columns.Add(Column);
         */
    }
}
