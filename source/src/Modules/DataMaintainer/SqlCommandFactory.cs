using System.Collections.Generic;
using System.Text;

namespace Testflow.DataMaintainer
{
    internal static class SqlCommandFactory
    {
        const string Delim = ",";
        const string AssignChar = "=";
        const string WhereFormat = " WHERE ({0})";
        public static string CreateQueryCmd(string filter, string tableName, params string[] columnName)
        {
            const string cmdFormat = "SELECT {0} FROM {1}";
            string columnNames = columnName.Length == 0 ? "*" : string.Join(Delim, columnName);
            string cmd = string.Format(cmdFormat, columnNames, tableName);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                cmd += string.Format(WhereFormat, filter);
            }
            return cmd;
        }

        public static string CreateQueryCmdWithOrder(string filter, string tableName, params string[] orderColumns)
        {
            const string cmdFormat = "SELECT * FROM {0}";
            const string orderFormat = " ORDER BY {0}";
            string orderColumnStr = orderColumns.Length == 0
                ? ""
                : string.Format(orderFormat, string.Join(Delim, orderColumns));
            string cmd = string.Format(cmdFormat, tableName);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                cmd += string.Format(WhereFormat, filter);
            }
            cmd += orderColumnStr;
            return cmd;
        }

        public static string CreateCalcCountCmd(string filter, string tableName)
        {
            const string cmdFormat = "SELECT COUNT (*) FROM {0}";
            string cmd = string.Format(cmdFormat, tableName);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                cmd += string.Format(WhereFormat, filter);
            }
            return cmd;
        }

        public static string CreateInsertCmd(string tableName, Dictionary<string, string> keyValues)
        {
            const string cmdFormat = "INSERT INTO {0} ({1}) VALUES ({2})";
            string columnStr = string.Join(Delim, keyValues.Keys);
            string valueStr = string.Join(Delim, keyValues.Values);
            return string.Format(cmdFormat, tableName, columnStr, valueStr);
        }

        public static string CreateUpdateCmd(string tableName, Dictionary<string, string> lastValues, Dictionary<string, string> newValues, string filter)
        {
            const string cmdFormat = "UPDATE {0} SET {1}{2}";
            StringBuilder valuePairStr = new StringBuilder(200);
            foreach (KeyValuePair<string, string> keyValuePair in newValues)
            {
                if (lastValues.ContainsKey(keyValuePair.Key) &&
                    lastValues[keyValuePair.Key].Equals(newValues[keyValuePair.Key]))
                {
                    continue;
                }
                valuePairStr.Append(keyValuePair.Key).Append(AssignChar).Append(keyValuePair.Value).Append(Delim);
            }
            if (0 == valuePairStr.Length)
            {
                return string.Empty;
            }
            valuePairStr.Remove(valuePairStr.Length - 1, 1);
            return string.Format(cmdFormat, tableName, valuePairStr, string.Format(WhereFormat, filter));
        }

        public static string CreateDeleteCmd(string tableName, string filter)
        {
            const string cmdFormat = "DELETE FROM {0}";
            string cmd = string.Format(cmdFormat, tableName);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                cmd += string.Format(WhereFormat, filter);
            }
            return cmd;
        }
    }
}