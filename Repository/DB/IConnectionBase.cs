using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace apiTicket.Repository.DB
{
    public interface IConnectionBase
    {
        DbParameterCollection ParamsCollectionResult
        {
            get;
            set;
        }

        /*DbConnection*/
        OracleConnection ConnectionGet(ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleCanalP);

        DbDataReader ExecuteByStoredProcedure(
            string nameStore,
            //ref DbParameterCollection z, 
            IEnumerable<DbParameter> parameters = null,
            ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleCanalP,
            ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader
            );
        DbParameterCollection ExecuteByStoredProcedureNonQuery(
           string nameStore,
           //ref DbParameterCollection z, 
           IEnumerable<DbParameter> parameters = null,
           ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleCanalP,
           ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteNonQuery
           );
        Task<DbDataReader> ExecuteByStoredProcedureVTAsync(string nameStore,
    IEnumerable<DbParameter> parameters = null,
    ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleCanalP,
    ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader);

        int ExecuteByStoredProcedureInt(string nameStore,
               IEnumerable<DbParameter> parameters = null,
               ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
               ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader);
    }
}