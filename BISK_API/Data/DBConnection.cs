using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;
using Dapper;
using GardeningAPI.Model;
using SAPbobsCOM;
using System.Xml.Linq;

namespace GardeningAPI.Data
{
    public class OdbcClient
    {
        private readonly string _connectionString;
        private readonly string? _dbName;

        public OdbcClient()
        {
            var config = ConfigManager.Instance.getConfig();
            var dbServer = config["DB:ServerName"];
            var dbUser = config["DB:DbUser"];
            var dbPass = config["DB:DbPass"];
            _dbName = config["DB:DbName"];

            _connectionString =
                $@"Driver=HDBODBC;ServerNode={dbServer};UID={dbUser};PWD={dbPass};CS={_dbName}";
        }

        //----------------------Validation and Insertion Async-------------------------

        private async Task<List<T>> ExecuteReaderAsync<T>(string sql, object? param = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            try
            {
                await using var connection = new OdbcConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.QueryAsync<T>(sql, param);
                return result.AsList();
            }
            catch (OdbcException ex)
            {
                throw new InvalidOperationException($"ODBC ExecuteReaderAsync failed: {ex.Message}", ex);
            }
        }

        private async Task<T?> ExecuteReaderSingleAsync<T>(string sql, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            try
            {
                await using var connection = new OdbcConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            }
            catch (OdbcException ex)
            {
                throw new InvalidOperationException($"ODBC ExecuteReaderSingleAsync failed: {ex.Message}", ex);
            }
        }

        private async Task<bool> ExecuteReaderExistsAsync(string sql, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            try
            {
                await using var connection = new OdbcConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.ExecuteScalarAsync<int?>(sql, parameters);
                return result.HasValue;
            }
            catch (OdbcException ex)
            {
                throw new InvalidOperationException($"ODBC ExecuteReaderExistsAsync failed: {ex.Message}", ex);
            }
        }

        private async Task<int> ExecuteUpdateAsync(string sql, object? param = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            try
            {
                await using var connection = new OdbcConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sql, param);
            }
            catch (OdbcException ex)
            {
                throw new InvalidOperationException($"ODBC ExecuteUpdateAsync failed: {ex.Message}", ex);
            }
        }


        public async Task<bool> ValidateUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            var sql = @"SELECT 1 FROM OCRD WHERE ""U_Username"" = @Username LIMIT 1";
            var parameters = new { Username = username };
            return await ExecuteReaderExistsAsync(sql, parameters);
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var sql = $@"SELECT 1 FROM OCRD WHERE ""E_Mail"" ='{email}' LIMIT 1";

            return await ExecuteReaderExistsAsync(sql);
        }
        public async Task<bool> CheckUserVerificationStatus(string email)
        {
            var sql = @"SELECT ""U_Verified"" FROM OCRD WHERE ""E_Mail"" = ?";

            using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            string? status = await connection.ExecuteScalarAsync<string>(sql, new { email });

            return status == "Y";
        }

        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var sql = $@"SELECT 1 FROM OCRD WHERE ""E_Mail"" = '{email}' AND ""U_Password"" = '{password}' LIMIT 1";
            return await ExecuteReaderExistsAsync(sql);
        }

        //------------------Get Functions-------------------


        public async Task<string?> GetCardCodeByEmailAsync(string email)
        {
            const string sql = @"SELECT ""CardCode"" FROM ""OCRD"" WHERE ""E_Mail"" = ?";

            using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { email });

            return result;
        }


        public async Task<SignUpResponse?> GetLoginDetailsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var sql = $@"
            SELECT 
                T0.""CardCode"",
                T0.""CardName"" as userName,
                T0.""Cellular"" as Mobile,
                T0.""E_Mail"" AS EmailAddress,
                T1.""U_BPSeries"" as customerSeries,
                T1.""U_BPGroup"" as customerGroup,
                T1.""U_SOSeries"" as saleOrderSeries ,
                T1.""U_DelSeries"" as deliverySeries,
                T1.""U_InvSeries"" as invoiceSeries,
                T1.""U_IncSeries"" as incomingSeries,
                T1.""U_MemoSeries"" as memoSeries,
                T1.""U_DPSeries"" as downPaymentSeries,
                T1.""U_WHCode"" as warehouseCode,
                T0.""LangCode"" as langCode
            FROM OCRD T0
            LEFT JOIN ""@GR_CONF"" T1 ON 1=1
            WHERE T1.""Code"" = 'GR'
              AND T0.""E_Mail"" = '{email}'";

            using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<SignUpResponse, ConfigurationData, SignUpResponse>(
                sql,
                (user, config) =>
                {
                    user.configuration = config;
                    return user;
                },
                splitOn: "customerSeries" // Column where mapping to Configuration starts
            );

            return result.FirstOrDefault();
        }

        public async Task<ConfigurationData?> GetConfiguration()
        {

            var sql = $@"
            SELECT 
                T1.""U_BPSeries"" as customerSeries,
                T1.""U_BPGroup"" as customerGroup,
                T1.""U_SOSeries"" as saleOrderSeries ,
                T1.""U_DelSeries"" as deliverySeries,
                T1.""U_InvSeries"" as invoiceSeries,
                T1.""U_IncSeries"" as incomingSeries,
                T1.""U_MemoSeries"" as memoSeries,
                T1.""U_DPSeries"" as downPaymentSeries,
                T1.""U_WHCode"" as whsCode
            from ""@GR_CONF"" T1
            WHERE T1.""Code"" = 'GR'";

            return await ExecuteReaderSingleAsync<ConfigurationData>(sql);

        }

        public async Task<UserDetails> GetUserDetailsForOTP(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));


            var sql = $@"
            SELECT 
                T0.""CardCode"" as CardCode,
                T0.""CardName"" as userName,
                T0.""Cellular"" as Mobile,
                T0.""E_Mail"" AS EmailAddress,
                T0.""U_OTPCode"" as U_OTPCode         
            FROM OCRD T0
              WHERE T0.""E_Mail"" = '{email}'";

            using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<UserDetails>(
                sql
            );

            return result.FirstOrDefault();

        }


        internal async Task<OTP?> GetOtpRecordAsync(string CardCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CardCode))
                    throw new ArgumentException("CardCode is required.", nameof(CardCode));
                var sql = @"SELECT ""Code"", ""U_Email"", ""U_OTPCode"", ""U_CreatedAt"", ""U_ExpireAt"", ""U_IsUsed"" FROM ""@OTP_TABLE"" WHERE ""Code"" = ?";

                using var connection = new OdbcConnection(_connectionString);
                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { CardCode });

                if (result == null)
                    return null;

                return new OTP
                {
                    Code = result.Code,
                    U_Email = result.U_Email,
                    U_OTPCode = result.U_OTPCode,
                    U_CreatedAt = DateTime.Parse(result.U_CreatedAt),
                    U_ExpireAt = DateTime.Parse(result.U_ExpireAt),
                    U_IsUsed = result.U_IsUsed
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in GetOtpRecordAsync: {ex.Message}", ex);
            }
        }


        public async Task<Cart?> GetCartDetails(string CardCode)
        {
            if (string.IsNullOrWhiteSpace(CardCode))
                throw new ArgumentException("CardCode is required.", nameof(CardCode));

            var sql = $@"SELECT T0.""CardCode"", T0.""CardName"", T0.""U_Cart"", T1.""DocEntry"", T1.""LineNum"", T1.""ItemCode"", T1.""Dscription"", T1.""Quantity"", T1.""Price"" as ""UnitPrice"" FROM ODRF T0  INNER JOIN DRF1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" WHERE T0.""ObjType"" ='17' and T0.""CardCode""='{CardCode}' and IFNULL(""U_Cart"",'N')='Y'";

            using var connection = new OdbcConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<Cart, List<CartLines>, Cart>(
                sql,
                (user, config) =>
                {
                    user.DocumentLines = config;
                    return user;
                },
                splitOn: "LineNum" // Column where mapping to Configuration starts
            );

            return result.FirstOrDefault();
        }


        // ======================= Items ========================= //

        public async Task<List<ItemsMasterData>> GetItems(string WhsCode)
        {
            try
            {
                var sql = $@"CALL ""{_dbName}"".GetItemDetailsGarden('{WhsCode}');";

                return await ExecuteReaderAsync<ItemsMasterData>(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in Fetching Items from DB: {ex.Message}", ex);
            }
        }



        //----------------------Useless functions----------------------------



    }
}
