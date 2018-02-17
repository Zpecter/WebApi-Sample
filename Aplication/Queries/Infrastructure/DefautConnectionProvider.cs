﻿using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Aplication.Queries.Infrastructure
{
    public class DefautConnectionProvider : IConnectionProvider
    {
        public IDbConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}