using System;
using NpgsqlTypes;

namespace backend.Sqls.psql
{
  [Serializable]
  public class SQLParameter
  {
    public SQLParameter(object ObjValue, NpgsqlDbType SqlDbType)
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

    NpgsqlDbType _SqlDbType;

    public NpgsqlDbType SqlDbType
    {
      get { return _SqlDbType; }
      set { _SqlDbType = value; }
    }
  }
}