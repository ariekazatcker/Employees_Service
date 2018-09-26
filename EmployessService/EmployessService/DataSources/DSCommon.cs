using EmployeesService.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace EmployessService.DataSources
{
    /// <summary>
    /// This class provide generalized services for data sources.
    /// </summary>
    public class DSCommon
    {
        /// <summary>
        /// This method sets the sql command text for creating new employee
        /// </summary>
        /// <param name="command">sql command object to update</param>
        /// <param name="employee">new employee info </param>
        public static void SetEmployeeInsertCommandForRelationalDB(DbCommand command, EmployeeInfo employee)
        {
            command.CommandText = string.Format(@"insert into Employees(ID, FirstName, MiddleInitial, LastName, DateOfBirth, DateOfEmployment, Active, LastUPdated) 
                                                        values('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', date('now'))",
                                                        employee.ID.ToString(),
                                                        employee.FirstName,
                                                        employee.MiddleInitial == null ? "" : employee.MiddleInitial,
                                                        employee.LastName,
                                                        employee.DateOfBirth.ToString("yyyy-MM-dd"),
                                                        employee.DateOfEmployment.ToString("yyyy-MM-dd"),
                                                        employee.Active ? 1 :0);
        }

        /// <summary>
        /// This method sets the sql command text for creating existent employee
        /// </summary>
        /// <param name="command">sql command object to update</param>
        /// <param name="employee">existent employee info</param>
        public static void SetEmployeeUpdateCommandForRelationalDB(DbCommand command, EmployeeInfo employee)
        {
            command.CommandText = string.Format(@"update Employees set FirstName='{0}', MiddleInitial='{1}', LastName='{2}', DateOfEmployment='{3}', LastUPdated=date('now') where ID = '{4}'", 
                                                employee.FirstName, 
                                                employee.MiddleInitial == null? "": employee.MiddleInitial, 
                                                employee.LastName,
                                                employee.DateOfEmployment.ToString("yyyy-MM-dd"), 
                                                employee.ID.ToString());
        }

        /// <summary>
        /// set the command text for employee delete command
        /// </summary>
        /// <param name="command">sql command object to update</param>
        /// <param name="id">Employee ID</param>
        public static void SetEmployeeDeleteCommandForRelationalDB(DbCommand command, Guid id)
        {
            command.CommandText = string.Format("update Employees set Active=0 where ID = '{0}'", id.ToString());
        }

        /// <summary>
        /// This method retrieves the list of all active employees
        /// </summary>
        /// <param name="connection">the SQlite connection used</param>
        /// <returns></returns>
        public static IList<EmployeeInfo> GetAllEmployees(SQLiteConnection connection)
        {
            IList<EmployeeInfo> employees = new List<EmployeeInfo>();
            using (SQLiteCommand command = new SQLiteCommand("Select * from Employees where Active>0", connection))
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read()) 
                    {
                        // Print out the content of the text field:
                        // System.Console.WriteLine("DEBUG Output: '" + sqlite_datareader["text"] + "'");

                        employees.Add(new EmployeeInfo
                        {
                            ID = new Guid(dataReader.GetString(0)),
                            FirstName = dataReader.GetString(1),
                            MiddleInitial = dataReader.GetString(2),
                            LastName = dataReader.GetString(3),
                            DateOfBirth = dataReader.GetDateTime(4),
                            DateOfEmployment = dataReader.GetDateTime(5),
                            Active = dataReader.GetInt32(6) == 0 ? false : true
                        });
                    };
                }
            }

            return employees;
        }
    }
}