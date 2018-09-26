using System;
using System.Data.SQLite;
using EmployeesService.Models;
using EmployessService.DataSources.DsImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace EmployeeServiceTest
{
    [TestClass]
    public class SQLiteDdTests
    {
        const string CONNECTION_STRING = "Data Source=EmployeesTestDb;Version=3;New=true;";

        private static EmployeeInfo s_employee = new EmployeeInfo
        {
            ID = Guid.NewGuid(),
            FirstName = "Paul",
            MiddleInitial = "M",
            LastName = "George",
            DateOfBirth = new DateTime(1983, 12, 24),
            DateOfEmployment = new DateTime(2009, 05, 05),
            Active = true
        };
        static SQLiteDdTests()
        {
            // create test SQLite db 
            if (!System.IO.File.Exists("EmployeesTestDb"))
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    //create the DB Schema (just one table the Employees table)
                    using (SQLiteCommand command = new SQLiteCommand("Create Table Employees(ID Char(38) primary key, FirstName char(50), MiddleInitial char(50), LastName char(50), DateOfBirth DATETIME, DateOfEmployment DATETIME, Active integer, LastUpdated DATETime)", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        [TestMethod]
        public void ConnectTest()
        {
            SQLite sqliteDs = new SQLite();
            sqliteDs.Connect(CONNECTION_STRING);
            sqliteDs.CleanDs();
            Assert.AreEqual(sqliteDs.GetAllEmployees().Count, 0);
        }

        [TestMethod]
        public void CreateEmployeeTest()
        {
            SQLite sqliteDs = new SQLite();
            sqliteDs.Connect(CONNECTION_STRING);
            sqliteDs.CleanDs();
            sqliteDs.CreateOrUpdateEmployee(s_employee, true);
            Assert.AreEqual(sqliteDs.GetAllEmployees()[0],s_employee);
        }

        [TestMethod]
        public void UpdateEmployeeTest()
        {
            SQLite sqliteDs = new SQLite();
            sqliteDs.Connect(CONNECTION_STRING);
            sqliteDs.CleanDs();
            sqliteDs.CreateOrUpdateEmployee(s_employee, true);
            s_employee.FirstName = "Paul2";
            sqliteDs.CreateOrUpdateEmployee(s_employee, false);
            s_employee.FirstName = "Paul";
            Assert.AreNotEqual(sqliteDs.GetAllEmployees()[0], s_employee);
        }

        [TestMethod]
        public void DeleteEmployeeTest()
        {
            SQLite sqliteDs = new SQLite();
            sqliteDs.Connect(CONNECTION_STRING);
            sqliteDs.CleanDs();
            sqliteDs.CreateOrUpdateEmployee(s_employee, true);
            sqliteDs.DeleteEmployee(s_employee.ID);
            Assert.AreEqual(sqliteDs.GetAllEmployees().Count, 0);
        }

        [TestMethod]
        public void GetAllEmployeesTest()
        {
            SQLite sqliteDs = new SQLite();
            sqliteDs.Connect(CONNECTION_STRING);
            sqliteDs.CleanDs();
            sqliteDs.CreateOrUpdateEmployee(s_employee, true);
            Assert.AreEqual(sqliteDs.GetAllEmployees().Count, 1);
        }
    }
}
