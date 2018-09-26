using EmployeesService.Models;
using EmployessService;
using EmployessService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmployeeServiceTest
{
    [TestClass]
    public class EmployeesControllerTests
    {
        private static string EXTERNAL_DS_NAME = "ExTestDs";
        private static string INTERNAL_REPOSITORY_NAME = "EmployeesTestDb";

        private static List<EmployeeInfo> s_testEmployeesLIst = new List<EmployeeInfo>
        {
                new EmployeeInfo
            {
                ID = Guid.NewGuid(),
                FirstName = "Paul",
                MiddleInitial = "M",
                LastName = "George",
                DateOfBirth = new DateTime(1983, 12, 24),
                DateOfEmployment = new DateTime(2009, 05, 05),
                Active = true
            },
            new EmployeeInfo
            {
                ID = Guid.NewGuid(),
                FirstName = "Arie",
                MiddleInitial = "H",
                LastName = "Kazatcker",
                DateOfBirth = new DateTime(1973, 12, 24),
                DateOfEmployment = new DateTime(2012, 12, 05),
                Active = true
            },
            new EmployeeInfo
            {
                ID = Guid.NewGuid(),
                FirstName = "Daniel",
                MiddleInitial = "D",
                LastName = "James",
                DateOfBirth = new DateTime(1970, 01, 05),
                DateOfEmployment = new DateTime(2010, 10, 05),
                Active = true
            }
    };

        private static EmployeeInfo s_newEmployee = new EmployeeInfo
        {
            ID = Guid.NewGuid(),
            FirstName = "Yael",
            MiddleInitial = "F",
            LastName = "Bierkatz",
            DateOfBirth = new DateTime(1974, 01, 12),
            DateOfEmployment = new DateTime(2018, 05, 05),
            Active = true
        };

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void GetAllEmployeesWithServerErrorTest()
        {
            var employeesController = new EmployeesController();
            employeesController.GetAllEmployees();
        }

        [TestMethod]
        public void GetEmployeeByIdErrorTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            var employeesController = new EmployeesController();
            Assert.ThrowsException<HttpResponseException>(() => employeesController.GetEmployeeById(Guid.NewGuid()), "Employee not found for user ID", MSTestExtensions.ExceptionMessageCompareOptions.Contains);
        }

        [TestMethod]
        public void CreateNewEmployeeWithNoValidDataTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            var employeesController = new EmployeesController();
            string tmpName = s_testEmployeesLIst[0].FirstName;
            s_testEmployeesLIst[0].FirstName = null;
            Assert.ThrowsException<HttpResponseException>(() => employeesController.CreateNewEmployee(s_testEmployeesLIst[0]), "Employee info for creating a new employee is not valid", MSTestExtensions.ExceptionMessageCompareOptions.Contains);
            s_testEmployeesLIst[0].FirstName= tmpName;
        }

        [TestMethod]
        public void DeleteEmployeeTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            EmployeesController employeesController = new EmployeesController();
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Admin"), null);
            employeesController.DeleteEmployee(s_testEmployeesLIst[0].ID);
            Assert.IsNull(EmployeesDataManager.GetEmployeeById(s_testEmployeesLIst[0].ID));
        }

        [TestMethod]
        public void DeleteEmployeeWithWrongCredentialsTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            EmployeesController employeesController = new EmployeesController();
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("AdminX"), null);
            Assert.ThrowsException<HttpResponseException>(() => employeesController.DeleteEmployee(s_testEmployeesLIst[0].ID), "Logged in user is unauthorized to delete an employee.", MSTestExtensions.ExceptionMessageCompareOptions.Contains);
        }
    }
}
