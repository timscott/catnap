using System;
using System.Runtime.InteropServices;

namespace Catnap.Sqlite
{
    public static class Sqlite3
    {
        [DllImport("sqlite3", EntryPoint = "sqlite3_open")]
        public static extern SqliteResult Open(string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_close")]
        public static extern SqliteResult Close(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_changes")]
        public static extern int Changes(IntPtr db);

        public static IntPtr Prepare(IntPtr db, string query)
        {
            IntPtr stmt;
            var result = Prepare(db, query, query.Length, out stmt, IntPtr.Zero);
            if (result != SqliteResult.OK)
            {
                throw new SqliteException(Errmsg(db));
            }
            return stmt;
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid")]
        public static extern long LastInsertRowid(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_step")]
        public static extern SqliteResult Step(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_finalize")]
        public static extern SqliteResult Finalize(IntPtr stmt);

        public static string Errmsg(IntPtr db)
        {
            return Marshal.PtrToStringUni(getErrmsg(db));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index")]
        public static extern int BindParameterIndex(IntPtr stmt, string name);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null")]
        public static extern int BindNull(IntPtr stmt, int index);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int")]
        public static extern int BindInt(IntPtr stmt, int index, int val);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64")]
        public static extern int BindInt64(IntPtr stmt, int index, long val);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double")]
        public static extern int BindDouble(IntPtr stmt, int index, double val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text")]
        public static extern int BindText(IntPtr stmt, int index, string val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_count")]
        public static extern int ColumnCount(IntPtr stmt);
        
        public static string ColumnName(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16(stmt, index));
        }

        public static string ColumnText(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnText16(stmt, index));
        }
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_column_type")]
        public static extern SqliteColumnType ColumnType(IntPtr stmt, int index);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int")]
        public static extern int ColumnInt(IntPtr stmt, int index);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64")]
        public static extern long ColumnInt64(IntPtr stmt, int index);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_column_double")]
        public static extern double ColumnDouble(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob")]
        public static extern int ColumnBlob(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2")]
        private static extern SqliteResult Prepare(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

        [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16")]
        private static extern IntPtr getErrmsg(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16")]
        private static extern IntPtr ColumnName16(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16")]
        public static extern IntPtr ColumnText16(IntPtr stmt, int index);
    }
}