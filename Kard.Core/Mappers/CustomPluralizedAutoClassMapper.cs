using DapperExtensions.Core20.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Mappers
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
            base.Table(tableName);
        }
    }
}
