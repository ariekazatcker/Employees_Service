using EmployeesService.Models;
using EmployessService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceTest
{
    [TestClass]
    public class EmployeesDataManagerTests
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
        public void GetAllEmployeesTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            List< EmployeeInfo> actual = new List<EmployeeInfo>(EmployeesDataManager.GetAllEmployees());
            CollectionAssert.AreEqual( s_testEmployeesLIst,actual);

        }

        [TestMethod]
        public void GetEmployeeByIdTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            EmployeeInfo employee = EmployeesDataManager.GetEmployeeById(s_testEmployeesLIst[0].ID);
            Assert.AreEqual(employee, s_testEmployeesLIst[0]);
            
        }

        [TestMethod]
        public void GetNotExistEmployeeByIdTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            EmployeeInfo employee = EmployeesDataManager.GetEmployeeById(Guid.NewGuid());
            Assert.IsNull(employee);
        }

        [TestMethod]
        public void CreateNewEmployeeTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            s_testEmployeesLIst.Add(s_newEmployee);
            EmployeesDataManager.CreateNewEmployee(s_newEmployee);
            CollectionAssert.AreEqual(s_testEmployeesLIst, new List<EmployeeInfo>(EmployeesDataManager.GetAllEmployees()));
            s_testEmployeesLIst.Remove(s_newEmployee);
        }


        [TestMethod]
        public void UpdateExistentEmployeeTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            string origName = s_testEmployeesLIst[0].FirstName;
            s_testEmployeesLIst[0].FirstName = "TestName";
            EmployeesDataManager.UpdateEmployee(s_testEmployeesLIst[0]);
            Assert.AreEqual(EmployeesDataManager.GetEmployeeById(s_testEmployeesLIst[0].ID).FirstName, "TestName");
            s_testEmployeesLIst[0].FirstName = origName;
        }

        [TestMethod]
        public void DeleteEmployeeTest()
        {
            EmployeesDataManager.InitEmployeeManagerTestEnvironment(EXTERNAL_DS_NAME, INTERNAL_REPOSITORY_NAME, s_testEmployeesLIst);
            EmployeesDataManager.DeleteEmployee(s_testEmployeesLIst[0].ID);
            Assert.IsNull(EmployeesDataManager.GetEmployeeById(s_testEmployeesLIst[0].ID));
        }




    }
}
