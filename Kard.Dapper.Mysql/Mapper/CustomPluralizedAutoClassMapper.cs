using DapperExtensions.Mapper;

namespace Kard.Dapper.Mysql.Mapper
{
    public class CustomPluralizedAutoClassMapper<T> : AutoClassMapper<T> where T : class
    {
        //private readonly IDictionary<string, IList<string>> _dictionary;

        //public CustomPluralizedClassMapper()
        //{
        //    Type type = typeof(T);
        //    Table(type.Name);

        //    //Map()
        //    AutoMap();
        //}

        public override void Table(string tableName)
        {
            tableName = tableName.Replace("Entity", "");
            tableName = tableName.Remove(1).ToLower() + tableName.Substring(1);
            base.Table(tableName);
        }
    }
}
