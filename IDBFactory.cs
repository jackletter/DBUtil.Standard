﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace DBUtil
{
    public class IDBFactory
    {
        /// <summary>
        /// 创建IDB对象,注意.netcore中不支持oledb，这个组件中不再支持
        /// </summary>
        /// <param name="connStr">
        /// <para>连接字符串:</para>
        /// <para>SQLSERVER:   Data Source=.;Initial Catalog=JACKOA;User ID=sa;Password=xx;</para>
        /// <para>ORACLE:   Data Source=ORCLmyvm2;Password=sys123;User ID=sys;DBA Privilege=SYSDBA;</para>
        /// <para>MYSQL:   Data Source=localhost;Initial Catalog=test;User ID=root;Password=xxxx;</para>
        /// <para>POSTGRESQL:   Server=localhost;Port=5432;UserId=postgres;Password=xxxx;Database=test</para>
        /// <para>SQLITE:   Data Source=f:\demo.db;</para>
        /// </param>
        /// <param name="DBType">数据库类型:SQLSERVER、ORACLE、MYSQL、SQLITE、ACCESS、POSTGRESQL</param>
        /// <returns></returns>
        public static IDbAccess CreateIDB(string connStr, string DBType)
        {
            DBType = (DBType ?? "").ToUpper();
            if (DBType == "SQLSERVER")
            {
                SqlConnection conn = new SqlConnection(connStr);
                IDbAccess iDb = new SqlServerIDbAccess()
                {
                    conn = conn,
                    ConnectionString = connStr,
                    DataBaseType = DataBaseType.SQLSERVER
                };
                return iDb;
            }
            else if (DBType == "MYSQL")
            {
                //使用单独一个方法,防止在下面代码访问不到的情况下仍会因没有mysql组件而报错
                return CreateMySql(connStr);
            }
            else if (DBType == "ORACLE")
            {
                //使用单独一个方法,防止在下面代码访问不到的情况下仍会因没有oracle组件而报错
                return CreateOracle(connStr);
            }
            else if (DBType == "SQLITE")
            {
                //使用单独一个方法,防止在下面代码访问不到的情况下仍会因没有sqlite组件而报错
                return CreateSQLite(connStr);
            }
            else if (DBType == "POSTGRESQL")
            {
                //使用单独一个方法,防止在下面代码访问不到的情况下仍会因没有postgresql组件而报错
                return CreatePostgreSql(connStr);
            }
            else
            {
                throw new Exception("暂不支持这种(" + DBType + ")数据库!");
            }
        }

        /// <summary>
        /// 创建IDB对象
        /// </summary>
        /// <param name="connStr">
        /// <para>连接字符串:</para>
        /// <para>SQLSERVER:   Data Source=.;Initial Catalog=JACKOA;User ID=sa;Password=xx;</para>
        /// <para>ORACLE:   Data Source=ORCLmyvm2;Password=sys123;User ID=sys;DBA Privilege=SYSDBA;</para>
        /// <para>MYSQL:   Data Source=localhost;Initial Catalog=test;User ID=root;Password=xxxx;</para>
        /// <para>POSTGRESQL:   Server=localhost;Port=5432;UserId=postgres;Password=xxxx;Database=test</para>
        /// <para>ACCESS:   Provider=Microsoft.Jet.OLEDB.4.0;Data Source=G:\work\Multiplan.mdb;</para>
        /// <para>ACCESS:   Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Administrator\Desktop\demo.accdb;</para>
        /// <para>SQLITE:   Data Source=f:\demo.db;</para>
        /// </param>
        /// <param name="DBType">数据库类型:SQLSERVER、ORACLE、MYSQL、SQLITE、ACCESS、POSTGRESQL</param>
        /// <returns></returns>
        public static IDbAccess CreateIDB(string connStr, DataBaseType DBType)
        {
            string dbtype = DBType.ToString();
            return CreateIDB(connStr, dbtype);
        }


        private static IDbAccess CreateOracle(string connStr)
        {
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(connStr);
            IDbAccess iDb = new OracleIDbAccess()
            {
                conn = conn,
                ConnectionString = connStr,
                DataBaseType = DataBaseType.ORACLE
            };
            return iDb;
        }

        private static IDbAccess CreateMySql(string connStr)
        {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connStr);
            IDbAccess iDb = new MySqlIDbAccess()
            {
                conn = conn,
                ConnectionString = connStr,
                DataBaseType = DataBaseType.MYSQL
            };
            return iDb;
        }

        private static IDbAccess CreatePostgreSql(string connStr)
        {
            Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connStr);
            IDbAccess iDb = new PostgreSqlIDbAccess()
            {
                conn = conn,
                ConnectionString = connStr,
                DataBaseType = DataBaseType.PostgreSql
            };
            return iDb;
        }

        public static void CreateSQLiteDB(string absPath)
        {
            if (File.Exists(absPath))
            {
                throw new Exception("要创建的数据库文件已存在，请核对：" + absPath);
            }
            System.Data.SQLite.SQLiteConnection.CreateFile(absPath);
        }

        public static string GetSQLiteConnectionString(string absPath)
        {
            return GetSQLiteConnectionString(absPath, null);
        }

        public static string GetSQLiteConnectionString(string absPath, string pwd)
        {
            string str;
            if (string.IsNullOrWhiteSpace(pwd))
            {
                str = "Data Source=" + absPath;
            }
            else
            {
                str = "Data Source=" + absPath + ";Password=" + pwd;
            }
            return str;
        }

        private static IDbAccess CreateSQLite(string connStr)
        {
            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(connStr);
            IDbAccess iDb = new SQLiteIDbAccess()
            {
                conn = conn,
                ConnectionString = connStr,
                DataBaseType = DataBaseType.SQLITE
            };
            return iDb;
        }

        /// <summary>不要在程序运行环境中修改此值,但可以在应用程序启动时进行赋值
        /// </summary>
        public static IDSNOManager IDSNOManage = new SimpleIDSNOManager();

    }
}
