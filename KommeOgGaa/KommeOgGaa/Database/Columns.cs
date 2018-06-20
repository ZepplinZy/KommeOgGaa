namespace SQLite_DB_LIB
{
    /// <summary>
    /// Klassen indeholder alt det man kan
    /// lave med SQLite column
    /// </summary>
    public class Column
    {
        public const string TYPE_INT = "INTEGER";
        public const string TYPE_STRING = "TEXT";
        public const string TYPE_BLOB = "BLOB";

        public string name;
        public string type;
        public string defaultValue = null;
        public string foreignKeyReferences = null;
        public bool isAutoIncrement = false;
        public bool isNotNull = false;
        public bool isPrimaryKey = false;
        public bool isUniqueKey = false;


        /// <summary>
        /// Laver SQL query delen for denne column
        /// (uden foregin key)
        /// </summary>
        public string GetColumn()
        {
            string query = string.Format("`{0}` {1}", name, type);

            if (isPrimaryKey) query += " PRIMARY KEY";
            if (isUniqueKey) query += " UNIQUE";
            if (isAutoIncrement) query += " AUTOINCREMENT";
            if (isNotNull) query += " NOT NULL";

            if (defaultValue != null)
            {
                if (type.ToUpper() == Column.TYPE_INT) query += string.Format(" DEFAULT {0}", defaultValue);
                else query += string.Format(" DEFAULT '{0}'", defaultValue);
            }

            return query;
        }

        /// <summary>
        /// Laver SQL query delen for foregin
        /// key på denne column
        /// </summary>
        public string GetForeignKey()
        {
            if (foreignKeyReferences == null) return null;
            return string.Format("FOREIGN KEY(`{0}`) REFERENCES {1}", name, foreignKeyReferences);
        }
    }
}
