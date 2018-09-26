using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;

namespace EmployeesService.Models
{
    /// <summary>
    /// The data model represting employee data
    /// </summary>
    [DataContract]
    public class EmployeeInfo
    {
        /// <summary>
        /// unique identifier for an employee
        /// </summary>
        [DataMember(Name = "id")]
        public Guid ID { get; set; }

        /// <summary>
        /// Employee first name
        /// </summary>
        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Employee middle initial
        /// </summary>
        [DataMember(Name = "middleInitial")]
        public string MiddleInitial { get; set; }

        /// <summary>
        /// Employee last name
        /// </summary>
        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Employee date of birth
        /// </summary>
        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Employee date of employment 
        /// </summary>
        [DataMember(Name = "dateOfEmployment")]
        public DateTime DateOfEmployment { get; set; }

        /// <summary>
        /// Determines whether employee is active or inactive (deleted)
        /// </summary>
        [DataMember(Name = "active")]
        public bool Active { get; set; }

        /// <summary>
        /// overriding the ToString method for the Employee model
        /// </summary>
        /// <returns>string represents the EMployee information</returns>
        public override string ToString()
        {
            StringBuilder value = new StringBuilder("Employee Info:\n").
                Append("ID: ").Append(ID).Append(".\n").
                Append("First Name: ").Append(FirstName).Append(".\n").
                Append("Middle Initial: ").Append(MiddleInitial).Append(".\n").
                Append("Last Name: ").Append(LastName).Append(".\n").
                Append("Date of Birth: ").Append(DateOfBirth.ToShortDateString()).Append(".\n").
                Append("Date of Employement: ").Append(DateOfEmployment.ToShortDateString()).Append(".\n").
                Append("Status: ").Append(Active ? "Active" : "Inactive.");
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public override bool Equals(object employee)
        {
            if (employee == null)
            {
                return false;
            }

            if (ReferenceEquals(this, employee))
            {
                return true;
            }

            if (this.GetType()!= employee.GetType())
            {
                return false;
            }

            EmployeeInfo employeeToCompare = (EmployeeInfo)employee;
            return (this.ID.Equals(employeeToCompare.ID) 
                && this.FirstName.Equals(employeeToCompare.FirstName)
                && this.MiddleInitial.Equals(employeeToCompare.MiddleInitial)
                && this.LastName.Equals(employeeToCompare.LastName)
                && this.DateOfBirth.Equals(employeeToCompare.DateOfBirth)
                && this.DateOfEmployment.Equals(employeeToCompare.DateOfEmployment)
                && this.Active.Equals(employeeToCompare.Active)
                );
        }
    }
}