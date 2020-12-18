using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;

namespace WindowsMobile.DAO
{
    public abstract class DB
    {
        string connString;

        public DB()
        {
            ConnString = @"Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\MyDatabase#1.sdf;Persist Security Info=False";
        }

        public string ConnString
        {
            get { return connString; }
            set { connString = value; }
        }

        public abstract DbConnection Connect();

        public abstract string GetCountSql(string table, string where);

        public abstract string GetSequence(string prefix, string name);
        public abstract string GetNextValSql();
        public abstract string GetInsertSql(string table, object o, FieldInfo[] colonnes, bool useSeq);

        public abstract string GetSelectSql(string table, string[] colonnes, int debut, int fin, string where);
        public abstract string GetSelectSql(string sql, string where);

        public abstract string GetUpdateSql(string table, object o, FieldInfo[] cols, string where, List<string> notAdd);

        public abstract string GetDeleteSql(string table, object o, string where);

        public abstract string GetPagination(int debut, int fin);
    }
}
