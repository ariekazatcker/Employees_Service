using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeesService.Models;

namespace EmployessService.DataSources
{
    /// <summary>
    ///   This interface need being implemented for any external data source to ingest employees info from upon server startup
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Connecting to the database.
        /// </summary>
        /// <param name="connectionString">Connection string for connecting to the database</param>
        void Connect(string connectionString);

        /// <summary>
        /// Retreiving all Active employees
        /// </summary>
        /// <returns> List of all active employees.</returns>
        IList<EmployeeInfo> GetAllEmployees();

        /// <summary>
        /// Creating new employee or updating the information for existing employee.
        /// </summary>
        /// <param name="employee">employee info</param>
        /// <param name="create">'true' to create or 'false' to update</param>
        void CreateOrUpdateEmployee(EmployeeInfo employee, bool create);

        /// <summary>
        /// Deleting (setting to inactive) employee from data source 
        /// </summary>
        /// <param name="id">Employee ID to delete</param>
        void DeleteEmployee(Guid id);

        /// <summary>
        /// This method cleans the DS and starts fresh (mainly for test purposes);
        /// </summary>
        void CleanDs();
    }
}
