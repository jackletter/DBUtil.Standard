using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUtil
{
    /// <summary>
    /// 表的ID和编号的自动生成管理接口
    /// </summary>
    public interface IDSNOManager
    {
        /// <summary>
        /// 根据表名和列名生成ID,第一次生成后就不需要再访问数据库,频率高时使用
        /// <para>
        /// 示例：iDb.IDSNOManager.NewID(iDb, "User", "Id");//返回新生成的ID
        /// </para>
        /// </summary>
        /// <param name="iDb">数据库访问对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <returns>新生成的ID</returns>
        int NewID(IDbAccess iDb, string tableName, string colName);

        /// <summary>
        /// 使用程序锁直接从表的字段里面算得递增值,频率低时使用
        /// </summary>
        /// <param name="iDb">数据库访问对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <returns>新生成的ID</returns>
        int NewIDForce(IDbAccess iDb, string tableName, string colName);

        /// <summary>
        /// 重置一个表的ID,在内存中为指定表的指定字段设置ID
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <param name="val">为null时直接从内存中删除这个表和这个列的ID生成控制</param>
        /// <returns></returns>
        void ResetID(string tableName, string colName, int? val);

        /// <summary>
        /// 显示程序运行至当前,程序中缓存的表的ID列表(可以选择指定表名、列名)
        /// </summary>
        /// <param name="tableName">如果指定了tableName就只显示这个表的ID控制情况</param>
        /// <param name="colName">如果指定了colName就显示这个表的这个字段的ID控制情况</param>
        /// <returns></returns>
        List<string[]> ShowCurrentIDs(string tableName, string colName);

        /// <summary>
        /// 添加一个ID控制项,并指定初始值(默认为0,即下一个生成使用的为1)(慎用,必须填写正确的表名和字段名,否则无法在故障修复后恢复ID控制)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <param name="val">初始化值,默认0</param>
        void AddID(string tableName, string colName, int val = 0);


        /// <summary>
        /// 根据表名列名和格式块创建新的自动编号
        /// <para>示例：如果要为某个字段每天自动编号(序列号长度为6,从1开始),那么如下所示:</para>
        /// <para>
        /// iDb.IDSNOManager.NewSNO(iDb, "SysUser", "BankNo", new List&lt;SerialChunk&gt;() { new SerialChunk("FLOWNO", "Text[FLOWNO][6]"), new SerialChunk("DateTime", "DateTime[yyyyMMdd][8][incycle]"), new SerialChunk("SerialNo", "SerialNo[1,1,6,,day]") });
        /// </para>
        /// <para>上面代码返回：FLOWNO20191113000001</para>
        /// </summary>
        /// <param name="iDb">数据库访问对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <param name="chunks">格式块,参照类：<see cref="SerialChunk"/></param>
        /// <returns>生成的自动编号</returns>
        string NewSNO(IDbAccess iDb, string tableName, string colName, List<SerialChunk> chunks);

        /// <summary>
        /// 重置一个序列号控制项的当前编号
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="colName">列名</param>
        /// <param name="trunks">chunk集合(这里的每个chunk只要求Name属性不为空即可)</param>
        /// <param name="sno">指定的编号</param>
        /// <returns></returns>
        void ResetSNO(string tableName, string colName, List<SerialChunk> trunks, string sno);

        /// <summary>
        /// 显示当前环境下的当前序列号情况
        /// </summary>
        /// <param name="tableName">如果指定了tableName就只显示这个表的序列号控制情况</param>
        /// <param name="colName">如果指定了colName就显示这个表的这个字段的序列号控制情况</param>
        /// <param name="trunks">如果指定了trunks就显示当前格式控制下的序列号情况(根据trunk顺序及Name判别唯一)</param>
        /// <returns></returns>
        List<string[]> ShowCurrentSNOs(string tableName, string colName, List<SerialChunk> trunks);
    }
}
