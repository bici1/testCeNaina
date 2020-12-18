using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;
using WindowsMobile.Utils;
using System.Data.SqlServerCe;
using WindowsMobile.annotation;

namespace WindowsMobile.DAO
{
    class SqlServer : DB
    {

        public SqlServer() : base() { }
        public override DbConnection Connect()
        {
            SqlCeConnection sqlC = null;

            try
            {
                sqlC = new SqlCeConnection(ConnString);
            }
            catch (SqlCeException sqe)
            {
                throw sqe;
            }

            return sqlC;
        }

        public override string GetSequence(string prefix, string name)
        {
            string seq = "";

            if (!Util.IsEmpty(prefix)) seq += " CONCAT('" + prefix + "', ";
            seq += GetNextValSql() + name;
            if (!Util.IsEmpty(prefix)) seq += ")";

            return seq;
        }

        public override string GetInsertSql(string table, object o, FieldInfo[] colonnes, bool useSeq)
        {
            string sql = "INSERT INTO " + table + " (";
            string values = " VALUES (";

            for (int i = 0; i < colonnes.Length; i++)
            {
                if (colonnes[i].GetValue(o) == null) continue;
                if (Tools.checkIfId(colonnes[i], o)) continue; 
                sql += colonnes[i].Name;

                values += "?"; 

                sql += i != colonnes.Length - 1 ? ", " : ")";
                values += i != colonnes.Length - 1 ? ", " : ")";
            }
            return sql + values;
        }

        public override string GetDeleteSql(string table, object o, string where)
        {
            string sql = "DELETE FROM " + table;

            where = GetWhere(where);

            return sql + where;
        }

        public override string GetPagination(int debut, int fin)
        {
            string pagination = "";

            if (debut != -1)
            {
                if (debut < 1 || fin < 1)
                {
                    throw new Exception("Le début et la fin doit être supéreur à 0");
                }

                int offset = debut - 1;

                pagination += " OFFSET " + offset + " ROWS" +
                              " FETCH NEXT " + fin + " ROWS ONLY";
            }

            return pagination;
        }

        public override string GetSelectSql(string table, string[] colonnes, int debut, int fin, string where)
        {
            string cols = GetCols(colonnes);

            string sql = "SELECT ";

            if (debut == -1 && fin >= 1)
            {
                sql += " TOP " + fin;
            }

            sql += cols + " FROM [" + table + "] " + GetWhere(where) + GetPagination(debut, fin);

            return sql;
        }

        public override string GetSelectSql(string sql, string where)
        {
            if (sql != null) {
                sql += GetWhere(where);            
            }

            return sql;
        }


        public override string GetUpdateSql(string table, object o, FieldInfo[] colonnes, string where, List<string> notAdd)
        {
            if (Util.IsEmpty(colonnes))
            {
                throw new Exception("Vous devez spécifier le(s) colonne(s) à mettre à jour!");
            }

            string sql = "UPDATE " + table + " SET ";

            for (int i = 0; i < colonnes.Length; i++)
            {
                if (Tools.checkIfIdUpdate(colonnes[i], o, notAdd)) continue; 
                sql += colonnes[i].Name + "=?";
                sql += i != colonnes.Length - 1 ? ", " : "";
            }

            where = GetWhere(where);

            return sql + where;
        }

        public string GetWhere(string where)
        {
            string wh = !Util.IsEmpty(where) ? " " + where : " ";

            return wh;
        }

        public string GetCols(string[] colonnes)
        {
            string cols = "*";

            if (!Util.IsEmpty(colonnes))
            {
                cols = "";

                for (int i = 0; i < colonnes.Length; i++)
                {

                    cols += "[" + colonnes[i] + "]";
                    cols += i != colonnes.Length - 1 ? "," : "";
                }
            }

            return cols;
        }

        public override string GetNextValSql()
        {
            return " NEXT VALUE FOR ";
        }

        public override string GetCountSql(string table, string where)
        {
            string sql = " SELECT COUNT(*) as __n FROM " + table;
            where = GetWhere(where);

            return sql + where;
        }
    }
}
