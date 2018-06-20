using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using SQLite_LIB = System.Data.SQLite;

namespace SQLite_DB_LIB
{
    /// <summary>
    /// Klassen indholder static metoder 
    /// Der bestemmer hvordan vi skal snakke med 
    /// databasen
    /// </summary>
    public class Database
    {
        private static string _prefix;
        public static string Prefix { get { return _prefix; } }

        private static bool _isConnected = false;
        public static bool IsConnected { get { return _isConnected; } }

        private static string _lastQuery;
        public static string LastQuery { get { return _lastQuery; } }

        private static SQLite_LIB.SQLiteConnection sql_conn;
        private static SQLite_LIB.SQLiteCommand sql_cmd;


        /// <summary>
        /// Opret forbindelse til databasen
        /// </summary>
        public static bool Connect(string connectionString, string prefix = "")
        {

            if (IsConnected) return false;

            //Start connection
            sql_conn = new SQLite_LIB.SQLiteConnection(connectionString);
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();

            _prefix = prefix;
            _isConnected = true;

            return true;
        }

        /// <summary>
        /// Afbryd forbindelse til databasen
        /// </summary>
        public static bool Disconnect()
        {

            if (!IsConnected) return false;
            sql_cmd.Dispose(); //release all resouces
            sql_conn.Close();
            _isConnected = false;

            return true;
        }

        /// <summary>
        /// Indsæt en row med en enkelt værdi til tabel
        /// </summary>
        public static object Insert(string table, string column, object value)
        {
            return Insert(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Indsæt en row med flere værdier til tabel
        /// </summary>
        public static object Insert(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;
            values = StringsSQLready(values);

            string columnsCMD = string.Format("`{0}`", string.Join("`, `", columns.ToArray()));
            string valuesCMD = string.Join(", ", values.ToArray());

            ExecuteQuery(string.Format("INSERT INTO `{0}`({1}) VALUES({2});", tableWithPrefix, columnsCMD, valuesCMD));


            string pColumn = GetPrimaryKeyName(table);

            if (pColumn != "")
            {

                List<string> whrArr = new List<string>();
                for (int i = 0; i < columns.Length; i++)
                    whrArr.Add(string.Format("`{0}` = {1}", columns[i], values[i]));

                string whrCMD = "WHERE " + string.Join(" AND ", whrArr.ToArray());


                var result = GetRow<int>(table, new string[] { pColumn }, whrCMD);
                if (result.ContainsKey(0)) return result[0];
            }


            return false;
        }

        public static void Update(string table, Dictionary<string, object> values, object pkey)
        {
            string tableWithPrefix = Prefix + table;

            List<string> o = new List<string>();

            foreach (var item in values)
            {
                string clearValue = item.Value is int ? item.Value.ToString() : "'"+EscapeString(item.Value.ToString())+"'";
                o.Add(string.Format("`{0}` = {1}", item.Key, clearValue));
            }

            string pColumn = GetPrimaryKeyName(table);
            string key = pkey is int ? pkey.ToString() : "'" + pkey.ToString() + "'";

            string cmd = string.Format("UPDATE `{0}`\n SET {1}\n WHERE `{2}` = {3}", tableWithPrefix, string.Join(", ", o), pColumn, key);
            ExecuteQuery(cmd);
        }

        /// <summary>
        /// Hent den første row her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public static Dictionary<T, object> GetRow<T>(string table, string[] columns, string more = "")
        {
            string query = BuildGetRowsCMD(table, columns, more);

            var result = ExecuteQuery<T>(query);

            if (result.Count == 0) return null;
            else return result[0];
        }


        /// <summary>
        /// Hent flere rows her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public static List<Dictionary<T, object>> GetRows<T>(string table, string[] columns, string more = "")
        {
            string query = BuildGetRowsCMD(table, columns, more);
            var result = ExecuteQuery<T>(query);

            if (result.Count == 0) return null;
            else return result;
        }

        /// <summary>
        /// Opret en tabel i databasen
        /// </summary>
        public static void Create(string table, params Column[] columns)
        {
            string tableWithPrefix = Prefix + table;

            List<string> columnCMD = new List<string>();
            List<string> foreignCMD = new List<string>();


            foreach (var column in columns)
            {

                columnCMD.Add(column.GetColumn());

                if (column.foreignKeyReferences != null)
                    foreignCMD.Add(column.GetForeignKey());

            }

            string columnPart = string.Join(",\n ", columnCMD.ToArray());
            string foreignKeyPart = foreignCMD.Count > 0 ? ",\n " + string.Join(",\n ", foreignCMD.ToArray()) : "";

            string command = string.Format("CREATE TABLE `{0}`\n(\n {1}{2}\n);", tableWithPrefix, columnPart, foreignKeyPart);
            ExecuteQuery(command);
        }

        /// <summary>
        /// Slet en tabel i databasen
        /// </summary>
        public static void Drop(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("DROP TABLE `{0}`;", tableWithPrefix);
            ExecuteQuery(command);
        }

        public static void Delete(string table, string arg)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("DELETE FROM `{0}` {1};", tableWithPrefix, arg);
            ExecuteQuery(command);
        }

        /// <summary>
        /// Tjek om en tabel findes i databasen
        /// </summary>
        public static bool Exist(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("SELECT `name` FROM `sqlite_master` WHERE `type`='table' AND `name`='{0}';", tableWithPrefix);
            var result = ExecuteQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med disse værdier
        /// </summary>
        public static bool Exist(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;

            List<string> statements = new List<string>();

            values = StringsSQLready(values);

            
            for (int i = 0; i < columns.Length; i++)
                statements.Add(string.Format("`{0}` = {1}", columns[i], values[i]));


            string statementsCMD = string.Join(" AND ", statements.ToArray());
            string command = string.Format("SELECT * FROM `{0}` WHERE {1};", tableWithPrefix, statementsCMD);
            var result = ExecuteQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med denne værdi
        /// </summary>
        public static bool Exist(string table, string column, object value)
        {
            return Exist(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Kan udføre en selv skrevet SQL query/kommando
        /// og hvis det er en select kan man selv 
        /// vælge man bruge columns eller position som key
        /// </summary>
        public static List<Dictionary<T, object>> ExecuteQuery<T>(string cmd)
        {
            SQLite_LIB.SQLiteDataReader sql_reader;
            var result = new List<Dictionary<T, object>>();
            bool isRead = false;
            //Metoder der vil hente data
            string[] readMethods = { "SELECT", "PRAGMA" };

            //Find ud af om vi skal hente data
            foreach (var item in readMethods)
            {
                if (cmd.ToUpper().StartsWith(item))
                {
                    isRead = true;
                    break;
                }
            }

            //SQLite_LIB.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter();

            _lastQuery = cmd;
            sql_cmd.CommandText = cmd;

            //udfør kommando hvor man ikke venter på data
            if (!isRead)
            {
                sql_cmd.ExecuteNonQuery();
                return null;
            }

            //Udfør kommando hvor man skal have data tilbage
            sql_reader = sql_cmd.ExecuteReader();
            while (sql_reader.Read())
            {
                var row = new Dictionary<T, object>();

                for (int i = 0; i < sql_reader.FieldCount; i++)
                {

                    //Find ud af om vi skal bruge columns navn eller position som key
                    object key;
                    if (typeof(T) == typeof(String)) key = sql_reader.GetName(i);
                    else key = i;

                    row.Add((T)key, sql_reader[i]);
                }
                result.Add(row);
            }
            sql_reader.Close();
            return result;
        }

        /// <summary>
        /// Kan udføre en selv skrevet SQL query/kommando
        /// og hvis det er en select vil position være key
        /// </summary>
        public static List<Dictionary<int, object>> ExecuteQuery(string cmd) { return ExecuteQuery<int>(cmd); }

        /// <summary>
        /// Hent primary key column navn fra en tabel
        /// </summary>
        public static string GetPrimaryKeyName(string table)
        {

            string tableWithPrefix = Prefix + table;
            var result = ExecuteQuery(string.Format("PRAGMA table_info(`{0}`)", tableWithPrefix));

            for (int i = 0; i < result.Count; i++)
            {
                if (Convert.ToBoolean(result[i][5])) return (string)result[i][1];
            }

            return null;
        }

        /// <summary>
        /// Fjerner ' fra strings
        /// </summary>
        public static string EscapeString(string value) { return value.Replace("'", ""); }

        /// <summary>
        /// Sætter object som er string i ''
        /// </summary>
        private static object[] StringsSQLready(object[] values)
        {

            for (int i = 0; i < values.Length; i++)
                if (values[i] is string)
                    values[i] = string.Format("'{0}'", values[i]);

            return values;
        }


        /// <summary>
        /// Bygger SQL for SELECT query
        /// </summary>
        private static string BuildGetRowsCMD(string table, string[] columns, string more = "")
        {

            string[] sqlMethod = { "*", "COUNT" };

            string tableWithPrefix = Prefix + table;


            for (int i = 0; i < columns.Length; i++)
            {

                bool isMethod = false;
                foreach (string method in sqlMethod)
                {
                    if (columns[i].ToUpper().StartsWith(method))
                    {
                        isMethod = true;
                        break;
                    }
                }

                if (isMethod) continue;
                if (columns[i].StartsWith("`") && columns[i].EndsWith("`")) continue;
                columns[i] = "`" + columns[i] + "`";
            }

            return string.Format("SELECT {1} FROM `{0}`{2};", tableWithPrefix, string.Join(", ", columns), " " + more);
        }
    }
}
