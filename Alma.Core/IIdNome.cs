namespace Alma.Core
{
    public interface IIdNome : IIdNome<int>
    {
    }

    public interface IId : IId<int>
    {
    }

    public interface IIdNome<T> : IId<T> where T : struct
    {
        string Nome { get; }
    }
    public interface IId<T> where T : struct
    {
        T Id { get; }
    }
}
