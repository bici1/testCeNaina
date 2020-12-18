using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;
using WindowsMobile.Utils;

namespace WindowsMobile.DAO
{
    public class Dao
    {
        DB db;

        public DB Db
        {
            get { return db; }
            set { db = value; }
        }

        public Dao(DB database){
            Db = database == null ? database = new SqlServer() : database;
        }

        public int Count(DbConnection c, DbTransaction trans, string table, string where, params object[] values)
        {
            DbCommand command = null;
            DbDataReader reader = null;
            int count = 0;
            int con = 0;

            try
            {
                con = CheckConnection(c);

                string sql = db.GetCountSql(table, where);

                command = c.CreateCommand();
                command.CommandText = sql;
                command.Transaction = trans;

                SetWhere(where, command, values);

                reader = command.ExecuteReader();
                reader.Read();

                count = reader.GetInt32(0);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Dispose(); reader.Close(); }
                if (command != null) { command.Dispose(); }
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };
            }

            return count;
        }

        private int CheckConnection(DbConnection c)
        {
            if (c == null)
            {
                c = db.Connect();
            }
            if (c.State == System.Data.ConnectionState.Closed)
            {
                c.Open();
                return 1;
            }
            return 0;
        }

        private void CheckDateTime(object o)
        {
            if (o != null && o.GetType() == typeof(DateTime))
            {
                o = ((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string GetNextVal(DbConnection c, DbTransaction trans, string name, string prefix)
        {
            DbCommand command = null;
            DbDataReader reader = null;
            string seq = null;
            int con = 0;

            try
            {
                con = CheckConnection(c);
                command = c.CreateCommand();
                command.CommandText = "SELECT " + db.GetSequence(prefix, name);
                command.Transaction = trans;
                reader = command.ExecuteReader();
                reader.Read();

                seq = reader.IsDBNull(0) ? null : reader.GetString(0);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Dispose(); reader.Close(); }
                if (command != null) { command.Dispose(); }
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };
            }

            return seq;
        }

        public int Insert<T>(DbConnection c, DbTransaction trans, T instance, string table, string[] cols, bool useSeq)
        {
            int nb = 0;
            table = GetTable<T>(table, instance);
            Type t = instance.GetType();

            FieldInfo[] fields = Tools.GetAllFields(t, cols);

            string sql = db.GetInsertSql(table, instance, fields, useSeq);
            int con = 0;
            DbCommand command = null;

            try
            {
                con = CheckConnection(c);
                command = c.CreateCommand();
                command.CommandText = sql;

                foreach (FieldInfo f in fields)
                {
                    object value = f.GetValue(instance);

                    if (value != null && !Tools.checkIfId(f,instance))
                    {
                        CheckDateTime(value);

                        AddParameter(command, f.Name, value);
                    }
                }

                command.Transaction = trans;
                nb = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (command != null) command.Dispose();
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };
                //if (c != null && opened) c.Close();
            }

            return nb;
        }

        public int Update<T>(DbConnection c, DbTransaction trans, T instance, string table, string[] cols, List<string> notAdd, string where, params object[] values)
        {
            int nb = 0;
            table = GetTable<T>(table, instance);
            Type t = instance.GetType();

            FieldInfo[] fields = Tools.GetAllFields(t, cols);

            string sql = db.GetUpdateSql(table, instance, fields, where, notAdd);
            int con = 0;
            DbCommand command = null;

            try
            {
                con = CheckConnection(c);
                command = c.CreateCommand();
                command.CommandText = sql;

                foreach(FieldInfo f in fields)
                {
                    object value = f.GetValue(instance);

                    if (value != null && !Tools.checkIfIdUpdate(f,instance,notAdd))
                    {
                        CheckDateTime(value);

                        AddParameter(command, f.Name, value);
                    }
                }

                SetWhere(where, command, values);

                command.Transaction = trans;
                nb = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (command != null) command.Dispose();
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };
                //if (c != null && opened) c.Close();
            }

            return nb;
        }

        public int Delete<T>(DbConnection c, DbTransaction trans, T instance, string table, string where, params object[] values) {
            int nb = 0;
            int con = 0;
            table = GetTable<T>(table, instance);
            Type t = instance.GetType();

            string sql = db.GetDeleteSql(table, instance, where);
           
            DbCommand command = null;

            try
            {
                con = CheckConnection(c);
                command = c.CreateCommand();
                command.CommandText = sql;


                SetWhere(where, command, values);

                command.Transaction = trans;
                nb = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (command != null) command.Dispose();
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };
                //if (c != null && opened) c.Close();
            }

            return nb;
        }

        public static string GetTableFrom(object o)
        {
            string cl = o.ToString();
            return cl.Substring(cl.LastIndexOf('.') + 1);
        }

        public DbCommand CreateCommand(DbConnection c, DbTransaction trans, string sql)
        {
            DbCommand command = c.CreateCommand();
            command.CommandText = sql;
            command.Transaction = trans;

            return command;
        }

        private void SetWhere(string where, DbCommand command, params object[] valeurs)
        {
            if (!Util.IsEmpty(where))
            {
                for (int i = 0; valeurs != null && i < valeurs.Length; i++)
                {
                    AddParameter(command, ""+i, valeurs[i]);
                }
            }
        }

        private void AddParameter(DbCommand command, string nom, object val)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = nom;
            dbParameter.Value = val;
            command.Parameters.Add(dbParameter);
        }

        public void SetValues<T>(DbDataReader sdr, T obj, FieldInfo[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                //PropertyInfo pI = objT.GetProperty(Connection.FName(fields[i].Name));

                try
                {
                    //if (!sdr.IsDBNull(i))
                    //{
                        object val = sdr[fields[i].Name];

                        if (val.GetType().FullName.Equals("System.DBNull")) val = null;

                        fields[i].SetValue(obj, val);
                        //pI.SetValue(record, val, null);
                    //}
                }
                catch (NullReferenceException exp)
                {
                    continue;
                }
                catch (IndexOutOfRangeException io_exp)
                {
                    continue;
                }
            }
        }

        public void GetResult<T>(DbDataReader sdr, Type objT, FieldInfo[] fields, List<T> res)
        {
            while (sdr.Read())
            {
                T record = (T)Activator.CreateInstance(objT);

                SetValues(sdr, record, fields);

                res.Add(record);
            }
        }

        private string GetTable<T>(string table, T obj)
        {
            return table == null ? GetTableFrom(obj) : table;
        }

        public DbDataReader Execute(DbConnection c, DbCommand command, DbTransaction transac, string sql, string where, params object[] valeurs)
        {
            command = CreateCommand(c, transac, sql);
            SetWhere(where, command, valeurs);

            return command.ExecuteReader();
        }

        public T[] Select<T>(DbConnection c, DbTransaction transaction, T obj, string sql,string where, params object[] valeurs)
        {
            List<T> res = new List<T>();

            Type objT = obj.GetType();
            FieldInfo[] fields = Tools.GetAllFields(objT, null);

            sql = db.GetSelectSql(sql, where);
            int con = 0;
            DbCommand command = null;
            DbDataReader sdr = null;

            try
            {
                con = CheckConnection(c);

                sdr = Execute(c, command, transaction, sql, where, valeurs);

                GetResult<T>(sdr, objT, fields, res);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sdr != null && !sdr.IsClosed) { sdr.Dispose(); sdr.Close(); }
                if (command != null) command.Dispose();
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };

                //if (c != null && opened) c.Close();
            }

            return res.ToArray<T>();
        }

        public T[] Select<T>(DbConnection c, DbTransaction transaction, T obj, string table, string[] cols, int debut, int fin, string where, params object[] valeurs)
        {
            List<T> res = new List<T>();
            table = GetTable<T>(table, obj);

            Type objT = obj.GetType();
            FieldInfo[] fields = Tools.GetAllFields(objT, cols);

            string sql = db.GetSelectSql(table, cols, debut, fin, where);
            int con = 0;
            DbCommand command = null;
            DbDataReader sdr = null;

            try
            {
                con = CheckConnection(c);

                sdr = Execute(c, command, transaction, sql, where, valeurs);

                GetResult<T>(sdr, objT, fields, res);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sdr != null && !sdr.IsClosed) { sdr.Dispose(); sdr.Close(); }
                if (command != null) command.Dispose();
                if (c.State == System.Data.ConnectionState.Open && con == 1) { c.Close(); };

                //if (c != null && opened) c.Close();
            }

            return res.ToArray<T>();
        }

    }
}
