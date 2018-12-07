//using FluentNHibernate.Conventions;

namespace Alma.Dados.OrmNHibernate.Conventions
{
    //public class ManyToManyConvention : IHasManyToManyConvention
    //{
    //    public void Apply(FluentNHibernate.Conventions.Instances.IManyToManyCollectionInstance target)
    //    {
    //        var parentName = target.EntityType.Name;
    //        var childName = target.ChildType.Name;
    //        const string tableNameFmt = "{0}{1}";
    //        //const string keyColumnFmt = "FK_{0}_{1}";
    //        string tableName;

    //        if (parentName.CompareTo(childName) < 0)
    //        {
    //            tableName = String.Format(tableNameFmt, parentName, childName);
    //        }
    //        else
    //        {
    //            tableName = String.Format(tableNameFmt, childName, parentName);
    //        }


    //        //util para mySql, indiferente para mssql.
    //        tableName = tableName.ToLower();

    //        target.Table(tableName);

    //    }
    //}
}
