using EmployeesService.Models;
using EmployessService.DataSources;
using EmployessService.DataSources.DsImpl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Web;
using log4net;

namespace EmployessService
{
    /// <summary>
    /// This class provides services for managing Employees Data Repository.
    /// </summary>
    public static class EmployeesDataManager
    {

        private static readonly ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string EMPLOYEE_INTERNAL_DB = "EmployeesInternalDB.db";
        private static SQLiteConnection s_sqLiteConnection;
        private static IDataSource s_extDs;

        private static IDataSource ExternalDataSourceFactory(string dataSourceType)
        {
            IDataSource dataSource;
            var dsType = (DSTypes)Enum.Parse(typeof(DSTypes), dataSourceType);
            switch (dsType)
            {
                case DSTypes.SQLite:
                    dataSource = new SQLite();
                    break;
                    // here will come calls to other data-source implementations once they'll be implemented
                default:
                    dataSource = new SQLite();
                    break;
            }

            return dataSource;
        }

        /// <summary>
        /// This method is called on service startup and will do the foloowing:
        /// 1) Read the configuration for the external data-source and data credentials.
        /// 2) Create the service internal Employees repository, in case it is not exist.
        /// 3) Clear the internal Empoyess repository, in case it holds data from previous run.
        /// 4) Connects to the external repository (data source), reads the employees data from there and updates the internal repository.
        /// </summary>
        public static void Initate()
        {
            s_log.Info("===============================");
            s_log.Info("Employees Service initializing!");
            try
            {
                // Reading configuration
                var applicationSettings = ConfigurationManager.GetSection("ApplicationSettings") as NameValueCollection;
                string dataSourceType = applicationSettings["DataSourceType"] != null ? applicationSettings["DataSourceType"] : "SQLite";
                string connectionString = applicationSettings["ConnectionString"] != null ? applicationSettings["ConnectionString"] : "Data Source=c://EmployeesServiceExtSourceDemo//EmployeesDb.db;Version=3;New=true;";
                string intSQLiteFolder = applicationSettings["IntSQLiteFolder"] != null ? applicationSettings["IntSQLiteFolder"] : "c://EmployeesServiceData";

                s_log.Info(string.Format("Configuration Parameters retrieved:\nExternal Data Source Type: {0}.\nExternal Data-Source Connection String: {1}.\nInternal Repository Folder: {2}.",
                                           dataSourceType, connectionString, intSQLiteFolder));

                //create the internal database repository if not exist
                if (!System.IO.Directory.Exists(intSQLiteFolder))
                {
                    s_log.Info(string.Format("Creating the internal repository folder: {0}.", intSQLiteFolder));
                    System.IO.Directory.CreateDirectory(intSQLiteFolder);
                }

                string emptyDbPath = intSQLiteFolder + System.IO.Path.DirectorySeparatorChar + EMPLOYEE_INTERNAL_DB;

                if (!System.IO.File.Exists(emptyDbPath))
                {
                    s_log.Info(string.Format("Creating the Internal Repository file: {0}.", EMPLOYEE_INTERNAL_DB));
                    s_sqLiteConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New=true;", emptyDbPath));
                    s_sqLiteConnection.Open();

                    //create the DB Schema (just one table the Employees table)
                    using (SQLiteCommand command = new SQLiteCommand("Create Table Employees(ID Char(38) primary key, FirstName char(50), MiddleInitial char(50), LastName char(50), DateOfBirth DATETIME, DateOfEmployment DATETIME, Active integer, LastUpdated DATETime)", s_sqLiteConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                    s_log.Info(string.Format("Internal Repository file has been created: {0}.", EMPLOYEE_INTERNAL_DB));
                }
                else
                {
                    s_log.Info(string.Format("Connecting to Internal Repository: {0}.", EMPLOYEE_INTERNAL_DB));
                    s_sqLiteConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New=true;", emptyDbPath));
                    s_sqLiteConnection.Open();
                    s_log.Info("Connected!");
                }

                //connect to the exteranal repository and update the internal database with Employees Info
                s_log.Info("Connecting to the External Database and updating the Internal Repository!");
                s_extDs = ExternalDataSourceFactory(dataSourceType);
                s_extDs.Connect(connectionString);
                using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
                {
                    // Clean the Employees Internal DB (from previous run)
                    command.CommandText = "delete from Employees";
                    command.ExecuteNonQuery();

                    foreach (var employee in s_extDs.GetAllEmployees())
                    {
                        DSCommon.SetEmployeeInsertCommandForRelationalDB(command, employee);
                        command.ExecuteNonQuery();
                    }
                }
                s_log.Info("Employees Service ready!");
            }
            catch (Exception ex)
            {
                s_log.Fatal(string.Format("Fatal error occurred during Employees Service initialization. Exception Details: {0}", ex));
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExtDsTestName"></param>
        /// <param name="internalDbTestName"></param>
        /// <param name="testEmployeesList"></param>
        public static void InitEmployeeManagerTestEnvironment(string ExtDsTestName, string internalDbTestName, IList<EmployeeInfo> testEmployeesList)
        {
            //creating external test data-source

            using (SQLiteConnection sqliteConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New=true;", ExtDsTestName)))
            {
                sqliteConnection.Open();

                //create the DB Schema (just one table the Employees table)
                using (SQLiteCommand command = new SQLiteCommand("create table if not exists Employees(ID Char(38) primary key, FirstName char(50), MiddleInitial char(50), LastName char(50), DateOfBirth DATETIME, DateOfEmployment DATETIME, Active integer, LastUpdated DATETime)", sqliteConnection))
                {
                    command.ExecuteNonQuery();
                }

                //cleaning and recreating the external test DS 
                //create the DB Schema (just one table the Employees table)
                using (SQLiteCommand command = new SQLiteCommand("Delete from Employees", sqliteConnection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(sqliteConnection))
                {
                    foreach (var employee in testEmployeesList)
                    {
                        command.CommandText = string.Format("insert into Employees (ID, FirstName, MiddleInitial,LastName, DateOfBirth, DateOfEmployment,Active,LastUPdated) values ('{0}','{1}','{2}','{3}','{4}','{5}',1,date('now'))", 
                            employee.ID.ToString(),
                            employee.FirstName,
                            employee.MiddleInitial,
                            employee.LastName, 
                            employee.DateOfBirth.ToString("yyyy-MM-dd"),
                            employee.DateOfEmployment.ToString("yyyy-MM-dd")
                            );
                        command.ExecuteNonQuery();
                    }
                }
            }

            s_sqLiteConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New=true;", internalDbTestName));
            s_sqLiteConnection.Open();

            //create the DB Schema (just one table the Employees table)
            using (SQLiteCommand command = new SQLiteCommand("create table if not exists Employees(ID Char(38) primary key, FirstName char(50), MiddleInitial char(50), LastName char(50), DateOfBirth DATETIME, DateOfEmployment DATETIME, Active integer, LastUpdated DATETime)", s_sqLiteConnection))
            {
                command.ExecuteNonQuery();
            }

            //connect to the exteranal test repository and update the internal database with Employees Info
            s_extDs = new SQLite();
            s_extDs.Connect(string.Format("Data Source={0};Version=3;New=true;", ExtDsTestName));
            using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
            {
                // Clean the Employees Internal DB (from previous run)
                command.CommandText = "delete from Employees";
                command.ExecuteNonQuery();

                foreach (var employee in s_extDs.GetAllEmployees())
                {
                    DSCommon.SetEmployeeInsertCommandForRelationalDB(command, employee);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// This service checks if employee with particular ID exist.
        /// </summary>
        /// <param name="id">Employee ID to check</param>
        /// <returns>'true' if employee is in the repository and Active and 'false' otherwise.</returns>
        internal static bool ExistEmployee(Guid id)
        {
            s_log.Debug(string.Format("Check whether employee exist for ID: {0}", id));
            using (SQLiteCommand command = new SQLiteCommand(string.Format("Select * from Employees where ID='{0}' and Active = 1", id), s_sqLiteConnection))
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                        return true;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// Validates whether the information for updating existing employee is complete.
        /// </summary>
        /// <param name="validationError">Error messagte in case that the data is not complete</param>
        /// <param name="employee">employee info to validate</param>
        /// <returns></returns>
        internal static bool ValidateExistingEmployeeData(out string validationError, EmployeeInfo employee)
        {
            StringBuilder errorMessage = new StringBuilder("Employee data missing the following information: \n");
            bool dataIsValid = true;
            if (string.IsNullOrEmpty(employee.FirstName))
            {
                errorMessage.Append("First Name must be provided.\n");
                dataIsValid = false;
            }

            if (string.IsNullOrEmpty(employee.LastName))
            {
                errorMessage.Append("Last Name must be provided.\n");
                dataIsValid = false;
            }

            if (employee.DateOfEmployment == null)
            {
                errorMessage.Append("Date Of Employment must be provided.\n");
                dataIsValid = false;
            }

            validationError = errorMessage.ToString();

            return dataIsValid;
        }

        /// <summary>
        /// Validates whether the information for adding new employee is complete.
        /// </summary>
        /// <param name="validationError">Error messagte in case that the data is not complete</param>
        /// <param name="employee">Employee info</param>
        /// <returns></returns>
        internal static bool ValidateNewEmployeeData(out string validationError, EmployeeInfo employee)
        {
            bool dataIsValid;
            StringBuilder errorMessage = new StringBuilder();
            if (!(dataIsValid = ValidateExistingEmployeeData(out validationError, employee)))
            {
                errorMessage.Append(validationError);
            }
            else if (employee.DateOfBirth == null)
            {
                errorMessage.Append("Date Of Birth must be provided.\n");
                dataIsValid = false;
            }

            validationError = errorMessage.ToString();

            return dataIsValid;
        }

        /// <summary>
        /// This method returns all active employees in the repository.
        /// </summary>
        /// <returns>LIst of all active employees in the repository</returns>
        public static IEnumerable<EmployeeInfo> GetAllEmployees()
        {
            s_log.Debug("Retrieving all employees!");
            return DSCommon.GetAllEmployees(s_sqLiteConnection);
        }

        /// <summary>
        /// This method returns employee information for an employee ID.
        /// </summary>
        /// <param name="id">Employee ID to retrieve</param>
        /// <returns>employee information if exist or null otherwise</returns>
        public static EmployeeInfo GetEmployeeById(Guid id)
        {
            s_log.Debug(string.Format("Retrieve employee info for ID: {0}.", id));
            EmployeeInfo employee = null;
            using (SQLiteCommand command = new SQLiteCommand(string.Format("Select * from Employees where ID='{0}'",id.ToString()), s_sqLiteConnection))
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        // Print out the content of the text field:
                        // System.Console.WriteLine("DEBUG Output: '" + sqlite_datareader["text"] + "'");

                        if (dataReader.GetInt32(6) == 1)
                        {
                            employee = new EmployeeInfo
                            {
                                ID = new Guid(dataReader.GetString(0)),
                                FirstName = dataReader.GetString(1),
                                MiddleInitial = dataReader.GetString(2),
                                LastName = dataReader.GetString(3),
                                DateOfBirth = (DateTime)dataReader.GetValue(4),
                                DateOfEmployment = (DateTime)dataReader.GetValue(5),
                                Active = dataReader.GetInt32(6) == 0 ? false : true
                            };
                        }

                        break;
                    };
                }
            }

            return employee;
        }


        /// <summary>
        /// This method set an employees status to Inactive (deleting employee) and updating the external repository as well.
        /// </summary>
        /// <param name="id">Employee ID to delete</param>
        public static void DeleteEmployee(Guid id)
        {
            s_log.Debug(string.Format("Deleting employee for ID: {0}.", id));
            using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
            {
                DSCommon.SetEmployeeDeleteCommandForRelationalDB(command, id);
                command.ExecuteNonQuery();
            }

            //Sync External Data Source
            using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
            {
                DSCommon.SetEmployeeDeleteCommandForRelationalDB(command, id);
                command.ExecuteNonQuery();
            }
            s_extDs.DeleteEmployee(id);
        }

        /// <summary>
        /// This method creates a new employee in the service repository and updates the external repository as well
        /// </summary>
        /// <param name="employee">New employee info to create</param>
        public static void CreateNewEmployee(EmployeeInfo employee)
        {
            s_log.Debug(string.Format("Creating new employee. New employee ID: {0}.", employee.ID));
            using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
            {
                DSCommon.SetEmployeeInsertCommandForRelationalDB(command, employee);
                command.ExecuteNonQuery();
            }

            //Sync External Data Source
            s_extDs.CreateOrUpdateEmployee(employee, true);
        }

        /// <summary>
        /// This method updates existent employee in the service repository and updates the external repository as well
        /// </summary>
        /// <param name="employee"></param>
        public static void UpdateEmployee(EmployeeInfo employee)
        {
            s_log.Debug(string.Format("Updating employee. Employee ID: {0}.", employee.ID));
            // Create internal DB
            using (SQLiteCommand command = new SQLiteCommand(s_sqLiteConnection))
            {
                DSCommon.SetEmployeeUpdateCommandForRelationalDB(command, employee);
                command.ExecuteNonQuery();
            }

            //Sync External Data Source
            s_extDs.CreateOrUpdateEmployee(employee, false);
        }

    }
}