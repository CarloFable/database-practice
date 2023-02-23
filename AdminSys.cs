using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace LabSoftwareAdminSys
{
    public class AdminSys
    {
        private static readonly string server = "127.0.0.1";
        private static readonly string port = "3306";
        private static readonly string user = "root";
        private static readonly string password = "mysql";
        private static readonly string database = "labsftadminsys";
        private static readonly string access
            = $"server={server};port={port};user={user};password={password};database={database}";
        private static readonly MySqlConnection connection = new(access);

        public AdminSys()
        {
            connection.Open();
        }

        private static MySqlDataReader Search(string querytext)
        {
            MySqlCommand query = new(querytext, connection);
            return query.ExecuteReader();
        }

        private static int Update(string querytext)
        {
            MySqlCommand query = new(querytext, connection);
            int row;
            try
            {
                row = query.ExecuteNonQuery();
            }
            catch(MySqlException)
            {
                return -1;
            }
            return row;
        }

        public static bool LoginCheck(string id, string password)
        {
            MySqlDataReader result
                = Search($"select * from userinfo where id = \"{id}\" and keyword = \"{password}\" and state = 0");
            bool check = result.Read();
            result.Close();
            return check;
        }

        public static void SetOnState(string id)
        {
            _ = Update($"update userinfo set state = 1 where id = \"{id}\"");
        }

        public static void Logout(string id)
        {
            _ = Update($"update userinfo set state = 0 where id = \"{id}\"");
        }

        public static List<string[]> SoftView()
        {
            MySqlDataReader result
                = Search($"select * from software");
            List<string[]> software = new();
            while (result.Read())
            {
                string[] soft = new string[8];
                for(int i = 0; i < 8; ++i)
                {
                    soft[i] = result[i].ToString();
                }
                software.Add(soft);
            }
            result.Close();
            return software;
        }

        private static string PreString(string str)
        {
            if (str is null)
                return str;
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"'", @"\'");
            str = str.Replace("\"", @"\" + "\"");
            str = str.Replace(@"%", @"\%");
            str = str.Replace(@"_", @"\_");
            return str;
        }

        private static string SearchKeyword(string table, string[] where)
        {
            MySqlDataReader result
                = Search($"select group_concat(column_name) from information_schema.COLUMNS " +
                $"where table_name = \"{PreString(table)}\" and table_schema = \"{PreString(database)}\"");
            string cols = string.Empty;
            while(result.Read())
            {
                cols = result[0].ToString();
            }
            result.Close();
            string[] col = cols.Split(",");
            string query = $"select * from {PreString(table)} where ";
            int len = where.Length - 1;
            for(int i = 0; i < len; ++i)
            {
                if (where[i] == "")
                {
                    query += $"({PreString(col[i])} like \"%{PreString(where[i])}%\" or {PreString(col[i])} is null) and ";
                }
                else
                {
                    query += $"{PreString(col[i])} like \"%{PreString(where[i])}%\" and ";
                }
            }
            if (where[len] == "")
            {
                query += $"({PreString(col[len])} like \"%{PreString(where[len])}%\" or {PreString(col[len])} is null)";
            }
            else
            {
                query += $"{PreString(col[len])} like \"%{PreString(where[len])}%\"";
            }
            return query;
        }

        public static List<string[]> SoftView(string[] where)
        {
            MySqlDataReader result = Search(SearchKeyword("software", where));
            List<string[]> software = new();
            while (result.Read())
            {
                string[] soft = new string[8];
                for (int i = 0; i < 8; ++i)
                {
                    soft[i] = result[i].ToString();
                }
                software.Add(soft);
            }
            result.Close();
            return software;
        }

        public static void SoftUpdate(List<string[]> insertdata, List<string[]> deletedata, List<string[]> updatedata, List<SoftAdminPage.Op> history)
        {
            int i = 0, d = 0, u = 0;
            foreach(SoftAdminPage.Op op in history)
            {
                if (op == SoftAdminPage.Op.insert)
                {
                    string query = $"insert into software values({insertdata[i][0]}";
                    for(int j = 1; j < 8; ++j)
                    {
                        if (insertdata[i][j] is null || insertdata[i][j] == "")
                        {
                            query += $", null";
                            continue;
                        }
                        query += $", \"{PreString(insertdata[i][j])}\"";
                    }
                    query += ")";
                    _ = Update(query);
                    ++i;
                }
                else if (op == SoftAdminPage.Op.delete)
                {
                    string query = $"delete from software where id = {deletedata[d][0]}";
                    _ = Update(query);
                    ++d;
                }
                else
                {
                    string query = $"update software set ";
                    if (updatedata[u][1] is null || updatedata[u][1] == "")
                        query += $"sname = null, ";
                    else
                        query += $"sname = \"{PreString(updatedata[u][1])}\", ";
                    if (updatedata[u][2] is null || updatedata[u][2] == "")
                        query += $"category = null, ";
                    else
                        query += $"category = \"{PreString(updatedata[u][2])}\", ";
                    if (updatedata[u][3] is null || updatedata[u][3] == "")
                        query += $"version = null, ";
                    else
                        query += $"version = \"{PreString(updatedata[u][3])}\", ";
                    if (updatedata[u][4] is null || updatedata[u][4] == "")
                        query += $"cap = null, ";
                    else
                        query += $"cap = \"{PreString(updatedata[u][4])}\", ";
                    if (updatedata[u][5] is null || updatedata[u][5] == "")
                        query += $"config = null, ";
                    else
                    query += $"config = \"{PreString(updatedata[u][5])}\", ";
                    if (updatedata[u][6] is null || updatedata[u][6] == "")
                        query += $"envir = null, ";
                    else
                        query += $"envir = \"{PreString(updatedata[u][6])}\", ";
                    if (updatedata[u][7] is null || updatedata[u][7] == "")
                        query += $"intro = null ";
                    else
                        query += $"intro = \"{PreString(updatedata[u][7])}\" ";
                    query += $"where id = {updatedata[u][0]}";
                    _ = Update(query);
                    ++u;
                }
            }
        }

        public static List<string[]> LabView()
        {
            MySqlDataReader result
                = Search("select lab.addr, lname, concat(ifnull(aname,\"\"),\" (\",admin.id,\")\") aname, " +
                "lab.cap, concat(computer.id,\" (\",computer.config,\")\") conf, " +
                "group_concat(concat(sname,\" \",ifnull(version,\"\")) order by category separator \", \") sname "
                + "from lab, lab_computer, computer, maintain_lab, admin, lab_software, software "
                + "where lab.addr = lab_computer.addr "
                + "and lab_computer.id = computer.id "
                + "and lab.addr = maintain_lab.addr "
                + "and maintain_lab.id = admin.id "
                + "and lab.addr = lab_software.addr "
                + "and software.id = lab_software.id "
                + "group by lab.addr "
                + "order by lab.addr");
            List<string[]> labs = new();
            while (result.Read())
            {
                string[] lab = new string[6];
                for (int i = 0; i < 6; ++i)
                {
                    lab[i] = result[i].ToString();
                }
                labs.Add(lab);
            }
            result.Close();
            result = Search("select * from lab");
            while (result.Read())
            {
                bool flag = true;
                foreach(string[] l in labs)
                {
                    if (l[0] == result[0].ToString())
                        flag = false;
                }
                if(flag)
                {
                    string[] lab = new string[6];
                    lab[0] = result[0].ToString();
                    lab[1] = result[1].ToString();
                    lab[3] = result[2].ToString();
                    labs.Add(lab);
                }
            }
            result.Close();
            return labs;
        }

        public static void AddLabSoftware(string id, string addr)
        {
            if(addr is not null || addr != "")
                Update($"insert into lab_software values(\"{PreString(addr)}\", {id})");
        }

        public static void AddCourseSoftware(string id, string cname)
        {
            if (cname is not null || cname != "")
            {
                string cid = string.Empty;
                MySqlDataReader result = Search($"select id from course where cname = \"{PreString(cname)}\"");
                while (result.Read())
                {
                    cid = result[0].ToString();
                }
                result.Close();
                if(cid is not null || cid != "")
                    _ = Update($"insert into course_software values(\"{PreString(cid)}\", {id})");
            }
        }

        public static Stack<string> SearchLab(string addr)
        {
            Stack<string> addrs = new();
            if (addr is not null || addr != "")
            {
                MySqlDataReader result = Search($"select addr from lab where addr like \"%{PreString(addr)}%\"");
                while (result.Read())
                {
                    addrs.Push(result[0].ToString());
                }
                result.Close();
            }
            return addrs;
        }

        public static Stack<string> SearchCourse(string cname)
        {
            Stack<string> cnames = new();
            if (cname is not null || cname != "")
            {
                MySqlDataReader result = Search($"select cname from course where cname like \"%{PreString(cname)}%\"");
                while (result.Read())
                {
                    cnames.Push(result[0].ToString());
                }
                result.Close();
            }
            return cnames;
        }

        private static string SearchAid(string aname)
        {
            string aid = string.Empty;
            if (aname is not null || aname != "")
            {
                MySqlDataReader result = Search($"select id from admin where aname = \"{PreString(aname)}\"");
                while (result.Read())
                {
                    aid = result[0].ToString();
                }
                result.Close();
            }
            return aid;
        }

        public static void LabUpdate(List<string[]> insertdata, List<string[]> deletedata, List<string[]> updatedata, List<LabAdminPage.Op> history)
        {
            int i = 0, d = 0, u = 0;
            foreach (LabAdminPage.Op op in history)
            {
                if (op == LabAdminPage.Op.insert)
                {
                    string query = $"insert into lab values(\"{PreString(insertdata[i][0])}\",\"{PreString(insertdata[i][1])}\",{insertdata[i][3]})";
                    _ = Update(query);
                    string aid = Regex.Match(insertdata[i][2] ?? "", "[0-9]{9}").Value;
                    if(aid is null || aid == "")
                    {
                        aid = SearchAid(insertdata[i][2]);
                    }
                    query = $"insert into maintain_lab values({aid},\"{PreString(insertdata[i][0])}\",null,null,null)";
                    _ = Update(query);
                    query = $"insert into lab_computer values(\"{PreString(insertdata[i][0])}\",\"{PreString(insertdata[i][4])}\")";
                    _ = Update(query);
                    ++i;
                }
                else if (op == LabAdminPage.Op.delete)
                {
                    string query = $"delete from maintain_lab where addr = \"{PreString(deletedata[d][0])}\"";
                    _ = Update(query);
                    query = $"delete from lab_computer where addr = \"{PreString(deletedata[d][0])}\"";
                    _ = Update(query);
                    query = $"delete from lab_software where addr = \"{PreString(deletedata[d][0])}\"";
                    _ = Update(query);
                    query = $"delete from lab where addr = \"{PreString(deletedata[d][0])}\"";
                    _ = Update(query);
                    ++d;
                }
                else
                {
                    string aid = Regex.Match(updatedata[u][2] ?? "", "[0-9]{9}").Value;
                    if (aid is null || aid == "")
                    {
                        aid = SearchAid(updatedata[u][2]);
                    }
                    string query = $"update maintain_lab set id = {aid} where addr = \"{PreString(updatedata[u][0])}\"";
                    _ = Update(query);
                    query = $"update lab_computer set id = {updatedata[u][4]} where addr = \"{PreString(updatedata[u][0])}\"";
                    _ = Update(query);
                    query = $"update lab set lname = \"{PreString(updatedata[u][1])}\", cap = {updatedata[u][3]} where addr = \"{PreString(updatedata[u][0])}\"";
                    _ = Update(query);
                    ++u;
                }
            }
        }

        public static List<string[]> LabSoftView(string addr)
        {
            MySqlDataReader result
                = Search($"select software.id, category, sname from software, lab_software where lab_software.id = software.id and " +
                $"lab_software.addr = \"{PreString(addr)}\" order by category");
            List<string[]> software = new();
            while (result.Read())
            {
                string[] soft = new string[3];
                for (int i = 0; i < 3; ++i)
                {
                    soft[i] = result[i].ToString();
                }
                software.Add(soft);
            }
            result.Close();
            return software;
        }

        public static void LabSoftUpdate(List<string[]> deletedata, List<LabSoftList.Op> history)
        {
            int d = 0;
            foreach (LabSoftList.Op op in history)
            {
                if (op == LabSoftList.Op.delete)
                {
                    string query = $"delete from lab_software where id = {deletedata[d][0]}";
                    _ = Update(query);
                    ++d;
                }
            }
        }
    }
}
