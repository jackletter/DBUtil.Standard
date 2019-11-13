﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUtil
{
    /// <summary>
    /// 表结构描述类(实验中...)
    /// </summary>
    public class TableStruct
    {
        public string Name { set; get; }
        public string Desc { set; get; }
        public string PrimaryKey { set; get; }
        public List<Column> Columns = new List<Column>();
        public List<Constraint> Constraints = new List<Constraint>();
        public List<Trigger> Triggers = new List<Trigger>();
        public List<Index> Indexs = new List<Index>();
        public class Column
        {
            public string Name { set; get; }
            public string Type { set; get; }
            public string FinalType
            {
                get
                {
                    if ((Type.Contains("binary")
                        || Type.Contains("char")
                        || Type == "datetime2"
                        || Type == "datetimeoffset"
                        || Type == "decimal"
                        || Type == "numeric"
                        || Type == "time")
                        && (!Type.Contains("(")))
                    {
                        //采取的是Type和MaxLength分离的方式
                        return Type + "(" + (MaxLength == -1 ? "max" : MaxLength.ToString()) + ")";
                    }
                    else
                    {
                        return Type;
                    }
                }
            }
            public string IdentityStr { set; get; }
            public string FinalIdentity
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(IdentityStr))
                    {
                        return IdentityStr;
                    }
                    return IsIdentity ? "identity(" + (Start == 0 ? 1 : Start).ToString() + "," + (Incre == 0 ? 1 : Incre).ToString() + ")" : "";
                }
            }
            public string Desc { set; get; }

            public bool IsIdentity { set; get; }

            public bool IsNullable { set; get; }

            public bool IsPrimaryKey { set; get; }

            public string Default { set; get; }

            public int MaxLength { set; get; }

            public int Start { set; get; }

            public int Incre { set; get; }

            public bool IsUnique { set; get; }
        }

        public class Constraint
        {
            public string Name { set; get; }
            public string Type { set; get; }
            public string DelType { set; get; }
            public string UpdateType { set; get; }
            public string Keys { set; get; }
            public string RefStr { set; get; }
            public string Remark { set; get; }
        }

        public class Trigger
        {
            public string Name { set; get; }
            public string Type { set; get; }
        }

        public class Index
        {
            public string Name { set; get; }
            public string Desc { set; get; }
            public string Keys { set; get; }
        }
    }

    public class Proc
    {
        public string Name { set; get; }
        public string LastUpdate { set; get; }
        public string CreateSql { set; get; }
    }

    public class Func
    {
        public string Name { set; get; }
        public string Type { set; get; }
        public string LastUpdate { set; get; }
        public string CreateSql { set; get; }
    }
}
