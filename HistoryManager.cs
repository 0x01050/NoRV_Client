using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRV
{
    class HistoryManager
    {
        private static HistoryManager _instance = null;
        public static HistoryManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new HistoryManager();
            }
            return _instance;
        }

        SqliteConnection sqlite_conn = null;
        Random random = null;

        HistoryManager()
        {
            try
            {
                random = new Random();
                sqlite_conn = new SqliteConnection("Data Source=depos.db");
                sqlite_conn.Open();
                string checkSql = "SELECT COUNT(*) FROM History";
                SqliteCommand checkCommand = sqlite_conn.CreateCommand();
                checkCommand.CommandText = checkSql;
                checkCommand.ExecuteNonQuery();
            }
            catch(Exception)
            {
                sqlite_conn = null;
            }
        }
        public bool isOpen()
        {
            return sqlite_conn != null;
        }

        public string generateID()
        {
            while(true)
            {
                string id = DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 14)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                if (!checkIDExist(id))
                    return id;
            }
        }
        public bool checkIDExist(string id)
        {
            if (!isOpen())
                return false;
            try
            {

                string checkIDSql = "SELECT COUNT(*) FROM History WHERE depo_id=@id";
                SqliteCommand checkIDCommand = sqlite_conn.CreateCommand();
                checkIDCommand.CommandText = checkIDSql;
                checkIDCommand.Parameters.Add(new SqliteParameter("@id", id));
                long checkResult = (long)checkIDCommand.ExecuteScalar();
                return (checkResult != 0);
            }
            catch(Exception) { }
            return false;
        }

        public void saveDepo(string depoid, string witness, string casename, string videopath, string logpath)
        {
            if (!isOpen())
                return;
            try
            {
                string saveSql = "INSERT INTO History(depo_id, witness_name, case_name, video_path, log_path) VALUES(@depoid, @witness, @casename, @videopath, @logpath)";
                SqliteCommand saveCommand = sqlite_conn.CreateCommand();
                saveCommand.CommandText = saveSql;
                saveCommand.Parameters.Add(new SqliteParameter("@depoid", depoid));
                saveCommand.Parameters.Add(new SqliteParameter("@witness", witness));
                saveCommand.Parameters.Add(new SqliteParameter("@casename", casename));
                saveCommand.Parameters.Add(new SqliteParameter("@videopath", videopath));
                saveCommand.Parameters.Add(new SqliteParameter("@logpath", logpath));
                saveCommand.ExecuteNonQuery();
            }
            catch (Exception) { }
        }

        internal string getByID(string type, string id)
        {
            if (!isOpen())
                return "";
            try
            {
                SqliteCommand getCommand = sqlite_conn.CreateCommand();
                if (type == "video")
                    getCommand.CommandText = "SELECT video_path FROM History WHERE depo_id=@id";
                else if (type == "log")
                    getCommand.CommandText = "SELECT log_path FROM History WHERE depo_id=@id";
                else
                    return "";
                getCommand.Parameters.Add(new SqliteParameter("@id", id));
                var getResult = getCommand.ExecuteScalar().ToString();
                return getResult;
            }
            catch (Exception) { }
            return "";
        }

        private string addQuery(string key, bool whereStarted)
        {
            string result = " ";
            if (whereStarted)
                result += "AND ";
            else
                result += "WHERE ";
            result += (key + " LIKE @" + key + " ");
            return result;
        }

        public int getTotalCount(string case_name, string witness_name)
        {
            if (!isOpen())
                return 0;
            try
            {
                SqliteCommand coundCommand = sqlite_conn.CreateCommand();
                string countSql = "SELECT COUNT(*) FROM History";
                bool whereStarted = false;
                if(!String.IsNullOrEmpty(case_name))
                {
                    countSql += addQuery("case_name", whereStarted);
                    coundCommand.Parameters.Add(new SqliteParameter("@case_name", '%' + case_name + '%'));
                    whereStarted = true;
                }
                if (!String.IsNullOrEmpty(witness_name))
                {
                    countSql += addQuery("witness_name", whereStarted);
                    coundCommand.Parameters.Add(new SqliteParameter("@witness_name", '%' + witness_name + '%'));
                    whereStarted = true;
                }
                coundCommand.CommandText = countSql;
                long countResult = (long)coundCommand.ExecuteScalar();
                return Convert.ToInt32(countResult);
            }
            catch (Exception) { }
            return 0;
        }

        public object getHistory(int page, int limit, string case_name, string witness_name)
        {
            List<Dictionary<string, object>> getResult = new List<Dictionary<string, object>>();
            if (!isOpen())
                return getResult;
            try
            {
                SqliteCommand getCommand = sqlite_conn.CreateCommand();
                string getSql = "SELECT * FROM History";
                bool whereStarted = false;
                if (!String.IsNullOrEmpty(case_name))
                {
                    getSql += addQuery("case_name", whereStarted);
                    getCommand.Parameters.Add(new SqliteParameter("@case_name", '%' + case_name + '%'));
                    whereStarted = true;
                }
                if (!String.IsNullOrEmpty(witness_name))
                {
                    getSql += addQuery("witness_name", whereStarted);
                    getCommand.Parameters.Add(new SqliteParameter("@witness_name", '%' + witness_name + '%'));
                    whereStarted = true;
                }
                getSql += " ORDER BY depo_id LIMIT @offset, @limit";
                getCommand.CommandText = getSql;
                getCommand.Parameters.Add(new SqliteParameter("@offset", (page - 1) * limit));
                getCommand.Parameters.Add(new SqliteParameter("@limit", limit));
                SqliteDataReader getReader = getCommand.ExecuteReader();

                while(getReader.Read())
                {
                    getResult.Add(new Dictionary<string, object>
                    {
                        { "depo_id", getReader["depo_id"] },
                        { "case_name", getReader["case_name"] },
                        { "witness_name", getReader["witness_name"] }
                    });
                }
            }
            catch (Exception) { }
            return getResult;
        }
    }
}
