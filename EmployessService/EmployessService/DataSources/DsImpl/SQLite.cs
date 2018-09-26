using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using EmployeesService.Models;

namespace EmployessService.DataSources.DsImpl
{
    /// <summary>
    /// This class implements IDataSource for SQLite (embedded relational database).
    /// </summary>
    public class SQLite : IDataSource
    {
        private SQLiteConnection m_sqLiteConnection;

        /// <summary>
        /// Connecting to the database.
        /// </summary>
        /// <param name="connectionString">Connection string for connecting to the database</param>
        public void Connect(string connectionString)
        {
            m_sqLiteConnection = new SQLiteConnection(connectionString);
            m_sqLiteConnection.Open();
        }

        /// <summary>
        /// Retreiving all Active employees
        /// </summary>
        /// <returns> List of all active employees.</returns>
        public IList<EmployeeInfo> GetAllEmployees()
        {
            return DSCommon.GetAllEmployees(m_sqLiteConnection);
        }

        /// <summary>
        /// Creating new employee or updating the information for existing employee.
        /// </summary>
        /// <param name="employee">employee info</param>
        /// <param name="create">'true' to create or 'false' to update</param>
        public void CreateOrUpdateEmployee(EmployeeInfo employee, bool create)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_sqLiteConnection))
            {
                if (create)
                    DSCommon.SetEmployeeInsertCommandForRelationalDB(command, employee);
                else
                    DSCommon.SetEmployeeUpdateCommandForRelationalDB(command, employee);
                
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deleting (setting to inactive) employee from data source 
        /// </summary>
        /// <param name="id">Employee ID to delete</param>
        public void DeleteEmployee(Guid id)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_sqLiteConnection))
            {
                DSCommon.SetEmployeeDeleteCommandForRelationalDB(command, id);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// This method cleans the DS and starts fresh (mainly for test purposes);
        /// </summary>
        public void CleanDs()
        {
            using (SQLiteCommand command = new SQLiteCommand("Delete from Employees", m_sqLiteConnection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}