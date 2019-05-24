using ANCommon.Sql;
using System.Data;

namespace ANCommon.DataAccess
{
    public interface IDataSetAdapter
    {
        int GetCount();
        int GetCount(ICondition criteria);
        int GetCount(string data_member);
        int GetCount(string data_member, ICondition criteria);

        void LoadData();
        void LoadData(ICondition criteria);
        void LoadData(ICondition criteria, IOrderByExpression order_by);

        void LoadData(string data_member);
        void LoadData(string data_member, ICondition criteria);
        void LoadData(string data_member, ICondition criteria, IOrderByExpression order_by);

        void LoadData(string data_member, ICondition criteria, IOrderByExpression order_by, int start_record, int max_records);
        void LoadDataById(object id);
        void LoadDataById(object id, bool with_children);
        void LoadDataById(object[] ids, bool with_children);
        void LoadDataChildrens(object id);
        void LoadDataChildrens(object[] ids);


        int Insert();
        int Insert(string data_member);
        int Update();
        int Update(string data_member);
        int Delete();
        int Delete(string data_member);

        DataSet GetDataSet();

        string WriteToDisk(string data_dir);

    }
}
