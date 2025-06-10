using BillsPC_CleanArchitecture.Data.Implementation;
using BillsPC_CleanArchitecture.Data.Interfaces;
using Final_BillsPC.Data.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Tests.Helpers
{
    public abstract class DataTest
    {
        protected readonly IDataAccess _dataAccess;

        public DataTest()
        {
            /* 
                Hidden class is added to git ignore. You may need to create the class called 'Hidden' in this same namespace/folder.
                Ensure that your class has a constant named 'ConnectionString' which will be used to connect to the Database you use for running unit tests.

                Example:
                
                internal static class Hidden
                    {
                        public const string ConnectionString = "Server=myServerAddress;Database=myDataBaseForUnitTests;Trusted_Connection=True;Trust Server Certificate=True;";
                    }
                        */


            var connectionFactory = new SqlConnectionFactory(Hidden.ConnectionString);

            _dataAccess = new DataAccess(connectionFactory);
        }
    }
}
