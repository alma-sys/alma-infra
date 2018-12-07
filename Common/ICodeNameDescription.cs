namespace Alma.Core
{
    public interface IIdName : IId, IName
    {
    }

    public interface IId : IId<long>
    {
    }

    public interface IIdName<T> : IId<T>, IName where T : struct
    {
    }
    public interface IId<T> where T : struct
    {
        T Id { get; }
    }


    public interface IName
    {
        string Name { get; }
    }
    public interface ICode : ICode<string>
    {
    }
    public interface ICode<T>
    {
        T Code { get; }
    }
    public interface IDescription
    {
        string Description { get; }
    }
}
