using EmployeesService.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace EmployessService.Controllers
{
    /// <summary>
    /// This is the controller for exposing Employees Service methods
    /// </summary>
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        private static readonly ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// This service provides a list of all Active employees
        /// </summary>
        /// <returns>list of all Active employees</returns>
        [HttpGet]
        [Route("GetAllEmployees/")]
        public IEnumerable<EmployeeInfo> GetAllEmployees()
        {
            s_log.Info("Retrieving all employees!");
            try
            {
                return EmployeesDataManager.GetAllEmployees();
            }
            catch (Exception ex)
            {
                throw GenerateInternalServerErrorResponse(ex);
            }
        }

        /// <summary>
        /// This service returns employee data by id
        /// </summary>
        /// <param name="id">Unique identifier represents the employee</param>
        /// <returns>employee data by employee ID</returns>
        [HttpGet]
        [Route("GetEmployeeById/{id}")]
        public EmployeeInfo GetEmployeeById(Guid id)
        {
            s_log.Info(string.Format("Receive employee info for employee ID: {0}.", id));
            EmployeeInfo employee;
            try
            {
                employee = EmployeesDataManager.GetEmployeeById(id);
            }
            catch (Exception ex)
            {
                throw GenerateInternalServerErrorResponse(ex);
            }

            if (employee == null)
            {
                string errMsg = string.Format("Employee not found for user ID {0}", id.ToString());
                s_log.Error(errMsg);
                throw GenerateHttpResponseError(errMsg, HttpStatusCode.NotFound);
            }
            return employee;

        }

        /// <summary>
        /// This service creates a new employee
        /// </summary>
        /// <param name="employeeInfo">info for adding new employee</param>
        [HttpPost]
        [ActionName("createNewEmployee")]
        [Route("CreateNewEmployee")]
        public void CreateNewEmployee(EmployeeInfo employeeInfo)
        {
            if (!EmployeesDataManager.ValidateNewEmployeeData(out string validationError, employeeInfo))
            {
                string errMsg = string.Format("Employee info for creating a new employee is not valid. {0}.", validationError);
                s_log.Error(errMsg);
                throw GenerateHttpResponseError(errMsg, HttpStatusCode.BadRequest);
            }
            try
            {
                EmployeesDataManager.CreateNewEmployee(new EmployeeInfo
                {
                    ID = Guid.NewGuid(),
                    FirstName = employeeInfo.FirstName,
                    MiddleInitial = employeeInfo.MiddleInitial,
                    LastName = employeeInfo.LastName,
                    DateOfBirth = employeeInfo.DateOfBirth,
                    DateOfEmployment = employeeInfo.DateOfEmployment,
                    Active = true
                });
            }
            catch (Exception ex)
            {
                throw GenerateInternalServerErrorResponse(ex);
            }
            s_log.Info(string.Format("Created new employe.\nEmploye info: {0}", employeeInfo));
        }

        /// <summary>
        /// This service updates existing employee. 
        /// </summary>
        /// <param name="employeeInfo">employee info to update</param>
        [HttpPut]
        [ActionName("updateEmployee")]
        [Route("UpdateEmployee")]
        public void UpdateEmployee(EmployeeInfo employeeInfo)
        {
            if (!EmployeesDataManager.ValidateExistingEmployeeData(out string validationError, employeeInfo))
            {
                throw GenerateHttpResponseError(string.Format("Employee info for updating existent employee is not valid. {0}.", validationError), HttpStatusCode.BadRequest);
            }

            if (!EmployeesDataManager.ExistEmployee(employeeInfo.ID))
            {
                throw GenerateHttpResponseError(string.Format("Employee not found for user ID {0}", employeeInfo.ID.ToString()), HttpStatusCode.NotFound);
            }
            try
            {
                EmployeesDataManager.UpdateEmployee(
                    new EmployeeInfo
                    {
                        ID = employeeInfo.ID,
                        FirstName = employeeInfo.FirstName,
                        MiddleInitial = employeeInfo.MiddleInitial,
                        LastName = employeeInfo.LastName,
                        DateOfBirth = employeeInfo.DateOfBirth,
                        DateOfEmployment = employeeInfo.DateOfEmployment,
                        Active = employeeInfo.Active
                    });
            }
            catch (Exception ex)
            {
                throw GenerateInternalServerErrorResponse(ex);
            }
            s_log.Info(string.Format("Epdate employee Id: {0}", employeeInfo.ID));
        }


        /// <summary>
        /// This service deletes (mark as Inactive) the an existing employee.
        /// Inactive employee cannot be retrieved through GetAllEmployees or GetEmployeeById.
        /// </summary>
        /// <param name="id">Employee ID to delete</param>
        [HttpDelete]
        [BasicAuthenticationAttribute]
        [Route("DeleteEmployee")]
        public void DeleteEmployee(Guid id)
        {
            s_log.Info(string.Format("Deleting employee ID: {0}.", id));
            string username = Thread.CurrentPrincipal.Identity.Name;
            if (EmployeeScurity.AuthenticateUser(username))
            {
                //check if employee to delete exist
                if (!EmployeesDataManager.ExistEmployee(id))
                {
                    throw GenerateHttpResponseError(string.Format("Employee not found for user ID {0}", id.ToString()), HttpStatusCode.NotFound);
                }

                try
                {
                    EmployeesDataManager.DeleteEmployee(id);
                }
                catch (Exception ex)
                {
                    throw GenerateInternalServerErrorResponse(ex);
                }

            }
            else
            {
                throw GenerateHttpResponseError("Logged in user is unauthorized to delete an employee.", HttpStatusCode.Unauthorized);
            }

            s_log.Info(string.Format("Employee with ID {0}, has been deleted!.", id));
        }


        private static HttpResponseException GenerateHttpResponseError(string content, HttpStatusCode httpStatusCode)
        {
            s_log.Warn(content);
            HttpResponseMessage httpMsg = new HttpResponseMessage();
            httpMsg.Content = new StringContent(content);
            httpMsg.StatusCode = httpStatusCode;
            return new HttpResponseException(httpMsg);
        }

        private static HttpResponseException GenerateInternalServerErrorResponse(Exception additionalInfo)
        {
            return GenerateHttpResponseError(string.Format("Employees Service is not available. Additional information: {0}", additionalInfo.Message), HttpStatusCode.InternalServerError);
        }
    }
}
