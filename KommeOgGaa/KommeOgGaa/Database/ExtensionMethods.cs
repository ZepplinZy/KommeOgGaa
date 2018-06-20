namespace SQLite_DB_LIB
{
    /// <summary>
    /// Denne klasse bruges de metoder der 
    /// skal være extension methods på objecter
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Tjekker om et object er et hel tal
        /// </summary>
        public static bool IsInt(this object value)
        {
            return value is short || value is ushort || value is int
                || value is uint || value is long || value is ulong;
        }
    }
}
