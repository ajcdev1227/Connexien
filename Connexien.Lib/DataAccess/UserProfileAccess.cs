using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Connexien.Lib.DataAccess
{
    public class UserProfileAccess
    {
        private readonly IDbConnection _dbConnection;

        public UserProfileAccess()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnexienEntities"].ConnectionString;
            _dbConnection = new SqlConnection(connectionString);
        }

        public UserProfileAccess(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public UserProfileAccess(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }

        public List<CompleteProfile> GetUserProfiles()
        {
            return _dbConnection.Query<CompleteProfile>("usp_Profiles_GetCompleteProfile", commandType: CommandType.StoredProcedure).AsList();
        }
    }
}
