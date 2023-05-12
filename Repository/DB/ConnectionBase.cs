using apiTicket.Repository.DB;
using apiTicket.Repository.Interfaces;
using apiTicket.Utils;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace apiTicket.Repository.DB
{
    public class ConnectionBase : IConnectionBase
    {
        private string strConexionOracle = null;
        private string strConexionOracleVTime = null;
        private string strConexionOracleConciliacion = null;

        OracleConnection DataConnectionOracle = new OracleConnection();
        OracleConnection DataConnectionOracleTIME = new OracleConnection();
        OracleConnection DataConnectionOracleConciliacion = new OracleConnection();

        private readonly AppSettings _appSettings;

        public enum enuTypeDataBase
        {
            OracleCanalP,
            OracleVTime,
            OracleConciliacion
        }

        public enum enuTypeExecute
        {
            ExecuteNonQuery,
            ExecuteReader
        }

        //public DbParameterCollection ParamsCollectionResult;

        public DbParameterCollection ParamsCollectionResult { get; set; }

        //Constructor de la clase 
        public ConnectionBase(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            this.strConexionOracle = _appSettings.ConnectionStringORA;
            DataConnectionOracle.ConnectionString = this.strConexionOracle;

            this.strConexionOracleVTime = _appSettings.ConnectionStringTimeP;
            DataConnectionOracleTIME.ConnectionString = this.strConexionOracleVTime;

            this.strConexionOracleConciliacion = _appSettings.ConnectionStringConciliacion;
            DataConnectionOracleConciliacion.ConnectionString = this.strConexionOracleConciliacion;
        }

        public OracleConnection/*DbConnection*/ ConnectionGet(enuTypeDataBase typeDataBase = enuTypeDataBase.OracleCanalP)
        {
            /*DbConnection*/
            OracleConnection DataConnection = null;
            switch (typeDataBase)
            {
                case enuTypeDataBase.OracleCanalP:
                    DataConnection = DataConnectionOracle;
                    break;
                case enuTypeDataBase.OracleVTime:
                    DataConnection = DataConnectionOracleTIME;
                    break;
                case enuTypeDataBase.OracleConciliacion:
                    DataConnection = DataConnectionOracleConciliacion;
                    break;
                default:
                    break;
            }
            return DataConnection;
        }

        public DbDataReader ExecuteByStoredProcedure(string nameStore,
                IEnumerable<DbParameter> parameters = null,
                enuTypeDataBase typeDataBase = enuTypeDataBase.OracleCanalP,
                enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader
                )
        {
            /*DbConnection*/
            OracleConnection DataConnection = ConnectionGet(typeDataBase);
            /*DbCommand*/
            OracleCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;
            cmdCommand.InitialLONGFetchSize = 32767;


            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }
            //DEV CY 11-04-22 INI
            //DataConnection.Open();
            //DbDataReader myReader;
            if (DataConnection.State == ConnectionState.Closed)
            {
                DataConnection.Open();
            }
            //DEV CY 11-04-22 FIN                
            OracleDataReader myReader;


            if (((cmdCommand.Parameters.Contains("C_TABLE") || cmdCommand.Parameters.Contains("C_POL_DET") || IsOracleReader(cmdCommand))) && typeExecute == enuTypeExecute.ExecuteReader)
            {
                myReader = cmdCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            else
            {
                cmdCommand.ExecuteNonQuery();
                ParamsCollectionResult = cmdCommand.Parameters;
                //z = ParamsCollectionResult;
                cmdCommand.Connection.Close();
                myReader = null;
            }

            return myReader;
        }
        public DbParameterCollection ExecuteByStoredProcedureNonQuery(string nameStore,
                   IEnumerable<DbParameter> parameters = null,
                   enuTypeDataBase typeDataBase = enuTypeDataBase.OracleCanalP,
                   enuTypeExecute typeExecute = enuTypeExecute.ExecuteNonQuery
                   )
        {
            DbConnection DataConnection = ConnectionGet(typeDataBase);
            DbCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            DataConnection.Open();
            DbParameterCollection myReader = null;

            if (typeExecute == enuTypeExecute.ExecuteNonQuery)
            {
                cmdCommand.ExecuteNonQuery();
                myReader = cmdCommand.Parameters;
                cmdCommand.Connection.Close();
            }
            return myReader;
        }

        /// <summary>
        /// </summary>
        /// <param name="cmdCommand"></param>
        /// <returns></returns>
        private bool IsOracleReader(DbCommand cmdCommand)
        {
            bool isOracleReader = false;
            foreach (DbParameter item in cmdCommand.Parameters)
            {
                if (item is OracleParameter)
                {
                    if ((item as OracleParameter).OracleDbType == OracleDbType.RefCursor)
                    {
                        isOracleReader = true;
                        break;
                    }
                }
            }
            return isOracleReader;
        }
        public async Task<DbDataReader> ExecuteByStoredProcedureVTAsync(string nameStore,
           IEnumerable<DbParameter> parameters = null,
           enuTypeDataBase typeDataBase = enuTypeDataBase.OracleVTime,
           enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        {
            OracleConnection DataConnection = ConnectionGet(typeDataBase);
            OracleCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;
            cmdCommand.InitialLONGFetchSize = 32767;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }
            if (DataConnection.State == ConnectionState.Closed)
            {
                DataConnection.Open();
            }

            OracleDataReader myReader;
            if (((cmdCommand.Parameters.Contains("C_TABLE") || IsOracleReader(cmdCommand))) && typeExecute == enuTypeExecute.ExecuteReader)
            {
                myReader = (OracleDataReader)await cmdCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection); //CommandBehavior.CloseConnection 
            }
            else
            {
                await cmdCommand.ExecuteNonQueryAsync();
                ParamsCollectionResult = cmdCommand.Parameters;
                cmdCommand.Connection.Close();
                myReader = null;
            }
            return myReader;
        }
        public int ExecuteByStoredProcedureInt(string nameStore,
                IEnumerable<DbParameter> parameters = null,
                ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
                ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader)
        {

            int result = 0;

            DbConnection DataConnection = ConnectionGet(typeDataBase);
            DbCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            DataConnection.Open();

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            cmdCommand.ExecuteNonQuery();

            cmdCommand.Connection.Close();

            result = Convert.ToInt32(cmdCommand.Parameters["P_AFTER_FEC_CORTE"].Value.ToString());

            return result;
        }
    }
}
