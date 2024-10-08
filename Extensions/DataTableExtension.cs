using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace backend.Extensions
{
  public static class DataTableExtension
  {
    public static IList<T> ToList<T>(this DataTable table) where T : new()
    {
      IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
      IList<T> result = new List<T>();

      foreach (var row in table.Rows)
      {
        var item = MappingItem<T>((DataRow)row, properties);
        result.Add(item);
      }

      return result;
    }

    private static T MappingItem<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
    {
      T item = new T();
      foreach (var property in properties)
      {
        if (row.Table.Columns.Contains(property.Name))
        {
          //針對欄位的型態去轉換
          if (property.PropertyType == typeof(DateTime))
          {
            DateTime dt = new DateTime();
            if (DateTime.TryParse(row[property.Name].ToString(), out dt))
            {
              property.SetValue(item, dt, null);
            }
            else
            {
              property.SetValue(item, null, null);
            }
          }
          else if (property.PropertyType == typeof(decimal))
          {
            decimal val = new decimal();
            decimal.TryParse(row[property.Name].ToString(), out val);
            property.SetValue(item, val, null);
          }
          else if (property.PropertyType == typeof(Single))
          {
            Single val = new Single();
            Single.TryParse(row[property.Name].ToString(), out val);
            property.SetValue(item, val, null);
          }
          else if (property.PropertyType == typeof(double))
          {
            double val = new double();
            double.TryParse(row[property.Name].ToString(), out val);
            property.SetValue(item, val, null);
          }
          else if (property.PropertyType == typeof(int))
          {
            int val = new int();
            int.TryParse(row[property.Name].ToString(), out val);
            property.SetValue(item, val, null);
          }
          else
          {
            if (row[property.Name] == DBNull.Value && property.PropertyType == typeof(string))
            {
              property.SetValue(item, string.Empty, null);
            }

            if (row[property.Name] != DBNull.Value && property.PropertyType == typeof(string))
            {
              property.SetValue(item, row[property.Name], null);
            }

            if (row[property.Name] != DBNull.Value && property.PropertyType == typeof(bool))
            {
              var res = row[property.Name].ToString() == "1" ? true : false;
              property.SetValue(item, res, null);
            }
          }
        }
      }
      return item;
    }
  }
}