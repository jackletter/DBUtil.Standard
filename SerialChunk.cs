using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBUtil
{
    /// <summary>
    /// 字段的编号格式控制块
    /// <para>一个表的一个字段可以有多个编号格式,它们之间互不影响</para>
    /// <para>在程序中会对"指定的表+指定的字段+指定的编号格式"加锁用来并发控制</para>
    /// <para>如何指定字段的编号格式是否是同一个: List集合中所有SerialChunk的name相加值是否相等</para>
    /// <para>SerialChunk有两个重要的属性:name,formatStr,name表示名称可以用来唯一性判别,formatStr控制实际的编号格式,参照<see cref="FormatStr"/></para>
    /// <para>示例：根据前缀FLOWNO+日期+6位序列码生成编号的格式控制如下(编号如"FLOWNO20160203000001"):
    /// new List&lt;SerialChunk&gt;() { new SerialChunk("FLOWNO", "Text[FLOWNO][6]"),new SerialChunk("DateTime", "DateTime[yyyyMMdd][8][incycle]"),new SerialChunk("SerialNo", "SerialNo[1,1,6,,day]") };</para>
    /// </summary>
    public class SerialChunk
    {
        /// <summary>
        /// 序列块的唯一构造函数
        /// </summary>
        /// <param name="name">序列块的名称,程序锁的组成部分(表名+列名+所有的序列块名)</param>
        /// <param name="formatStr">
        /// 序列号块的格式控制(不允许出现多余的空格等字符,严格按格式填写,注意大小写),有三种格式:
        /// <para>格式1: SerialNo[start,incr,len,end,cyclemodel][varlen]</para>
        /// <para>格式2: Text[TextVal][len]</para>
        /// <para>格式3: DateTime[dateformate][len][incycle]</para>
        /// </param>
        public SerialChunk(string name, string formatStr)
        {
            this._name = name;
            this._formatstr = formatStr;
        }

        private string _name;

        /// <summary>
        /// 每个chunk都要有一个名字,这个名字作为编号生成锁的一部分
        /// <para>io</para>
        /// </summary>
        public string Name { get { return this._name; } }

        private string _formatstr;

        /// <summary>
        /// 序列号块的格式控制(不允许出现多余的空格等字符,严格按格式填写,注意大小写),有三种格式:
        /// 格式1: SerialNo[start,incr,len,end,cyclemodel][varlen]
        /// 格式2: Text[TextVal][len]
        /// 格式3: DateTime[dateformate][len][incycle]
        ///     <para>格式1: SerialNo[start,incr,len,end,cyclemodel][varlen]</para>
        ///     <para>  SerialNo:表示这是一个序列码</para>
        ///     <para>  start:序列码起始值(可选,默认为1)</para>
        ///     <para>  incr:序列码递增值(可选,默认为1)</para>
        ///     <para>  len:序列码占位长度(必须有)</para>
        ///     <para>  end:序列码结束值(可选,默认为无限大)</para>
        ///     <para>  cyclemodel:序列码循环模式:day-每天循环,month-每月循环,year-每年循环,hour-每小时循环,minute-每分钟循环,none-不循环(可选,默认为day)</para>
        ///     <para>  varlen:表示长度是变长的,该属性只在最后一个trunck中有效并自动忽略掉len属性</para>
        ///     <para>格式2: Text[TextVal][len]</para>
        ///     <para>   Text:表示这是一个文本</para>
        ///     <para>   TextVal:文本的具体值(必须)</para>
        ///     <para>   len:文本的长度(必须)</para>
        ///     <para>格式3: DateTime[dateformate][len][incycle]</para>
        ///     <para>   DateTime:表示这是一个日期输出项(必须)</para>
        ///     <para>   dateformate:日期的输出格式,使用形式:DateTime.Now.ToString(dateformate),例如:yyyyMMdd</para>
        ///     <para>   len:输出日期的占位长度(必须)</para>
        ///     <para>   incycle:是否在循环中(incyle表示在循环中,不填或填写其他的都是不在循环中)</para>
        ///     <para> 示例1."SerialNo[1,1,5,,day]":表示序列码从1开始每次增长1直至无限大,占5个字符位(不够长度的左侧补0,超过长度抛异常),每天重置</para>
        ///     <para> 示例2."Text[GWFW][4]":表示文本块本次使用的值是GWFW,占用了4个字符位</para>
        ///     <para> 示例3.DateTime[yyyyMMdd][8][incycle]:表示日期输出,格式化字符串为"yyyyMMdd",占位8个字符,在序列码的循环中</para>
        ///     <para> 示例4.DateTime[yyyyMMdd][8]:表示日期输出,格式化字符串为"yyyyMMdd",占位8个字符,不属于循环</para>
        /// </summary>
        public string FormatStr { get { return _formatstr; } }

        private string _type;
        private int _len;
        private bool _varlen;
        public string Type { get { return _type; } }
        public int Len { get { return _len; } }
        public bool Varlen { get { return _varlen; } }
        //SerialNo的属性
        private int _start;
        private int _incr;
        private int _end;
        private string _cyclemodel;
        public int Start { get { return _start; } }
        public int Incr { get { return _incr; } }
        public int End { get { return _end; } }
        public string CycleModel { get { return _cyclemodel; } }
        //Text的属性
        private string _textval;
        public string TextVal { get { return _textval; } }
        //DateTime的属性
        private string _dateformate;
        public string DateFormate { get { return _dateformate; } }
        private bool _incyle;
        public bool Incyle { get { return _incyle; } }

        public void PraseSelf()
        {
            Regex reg;
            if (FormatStr.StartsWith
                ("SerialNo["))
            {
                _type = "SerialNo";
                reg = new Regex(@"SerialNo\[(\d{0,})\,(\d{0,})\,(\d{0,})\,(\d{0,})\,([a-zA-Z]{0,})\](\[(varlen)\]){0,}");
                Match ma = reg.Match(FormatStr);
                if (ma.Success)
                {
                    _start = int.Parse((ma.Groups[1].Value ?? "1").ToString() == "" ? "1" : (ma.Groups[1].Value ?? "1").ToString());
                    _incr = int.Parse((ma.Groups[2].Value ?? "1").ToString() == "" ? "1" : (ma.Groups[2].Value ?? "1").ToString());
                    object obj = ma.Groups[3].Value ?? "";
                    if (string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        _len = -1;
                    }
                    else
                    {
                        _len = int.Parse(obj.ToString());
                    }
                    obj = ma.Groups[4].Value ?? "";
                    if (string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        _end = -1;
                    }
                    else
                    {
                        _end = int.Parse(obj.ToString());
                    }
                    _cyclemodel = (ma.Groups[5].Value ?? "").ToString();
                    if ((ma.Groups[7].Value ?? "").ToString() == "varlen")
                    {
                        _varlen = true;
                    }
                    else
                    {
                        _varlen = false;
                    }

                }
                else
                {
                    throw new Exception("当前未匹配成功,请检查语法:" + FormatStr);
                }
            }
            else if (FormatStr.StartsWith("Text["))
            {
                _type = "Text";
                reg = new Regex(@"Text\[([^\]]+)\]\[(\d+)\]");
                Match ma = reg.Match(FormatStr);
                if (ma.Success)
                {
                    _textval = ma.Groups[1].Value.ToString();
                    _len = int.Parse(ma.Groups[2].Value.ToString());
                }
                else
                {
                    throw new Exception("当前未匹配成功,请检查语法:" + FormatStr);
                }
            }
            else if (FormatStr.StartsWith("DateTime["))
            {
                _type = "DateTime";
                reg = new Regex(@"DateTime\[([^\]]+)\]\[(\d+)\](\[(incycle)\]){0,}");
                Match ma = reg.Match(FormatStr);
                if (ma.Success)
                {
                    _dateformate = ma.Groups[1].Value.ToString();
                    _len = int.Parse(ma.Groups[2].Value.ToString());
                    object obj = ma.Groups[4].Value ?? "";
                    if (obj.ToString() == "incycle")
                    {
                        _incyle = true;
                    }
                    else
                    {
                        _incyle = false;
                    }
                }
                else
                {
                    throw new Exception("当前未匹配成功,请检查语法:" + FormatStr);
                }
            }
        }
    }
}
