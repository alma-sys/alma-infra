namespace Alma.Core
{
    public interface IIdNome : IId, INome
    {
    }

    public interface IId : IId<long>
    {
    }

    public interface IIdNome<T> : IId<T>, INome where T : struct
    {
    }
    public interface IId<T> where T : struct
    {
        T Id { get; }
    }


    public interface INome
    {
        string Nome { get; }
    }
    public interface ICodigo : ICodigo<string>
    {
    }
    public interface ICodigo<T>
    {
        T Codigo { get; }
    }
    public interface IDescricao
    {
        string Descricao { get; }
    }
}
