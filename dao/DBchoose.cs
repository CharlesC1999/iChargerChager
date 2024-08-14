using backend.Sqls.mysql;
using backend.util;

namespace backend.dao
{
    public static class DBchoose
    {
        public static MysqlConnect GetDBConnect(appSettings appSettings, string type){
            switch(type){
                case "db":
                    return new MysqlConnect(appSettings.db);
                default:
                    return null;
            }
        }
    }
}