using System;

namespace Alma.DataAccess.MongoMapping.Generators
{
    public class Int64IdGenerator : MongoDB.Bson.Serialization.IIdGenerator
    {
        public object GenerateId(object container, object document)
        {
            return DateTime.Now.Ticks;
        }

        public bool IsEmpty(object id)
        {
            return (long)id == 0L;
        }
    }
}
