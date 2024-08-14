using System;
using System.Data;



namespace backend.Sqls.mssql
{
  [Serializable]
  public class SQLParameter
  {
    public SQLParameter(object ObjValue, SqlDbType SqlDbType)
    {
      this.ObjValue = ObjValue;
      this.SqlDbType = SqlDbType;
    }

    object _objValue;

    public object ObjValue
    {
      get { return _objValue; }
      set
      {
        if ((value == null))
          _objValue = DBNull.Value;
        else _objValue = value;
      }
    }

    SqlDbType _SqlDbType;

    public SqlDbType SqlDbType
    {
      get { return _SqlDbType; }
      set { _SqlDbType = value; }
    }
  }
}