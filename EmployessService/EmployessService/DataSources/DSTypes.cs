using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployessService.DataSources
{
    /// <summary>
    /// Represents the different repository types that may serve as the external repository for the Employees
    /// </summary>
    enum DSTypes
    {
        SQLite,
        MSSQL,
        Oracle,
        MongoDb,
        ElasticSearch,
        SalesForce
    }
}
