namespace Alma.Infra.Dominio.Entidades
{
    public interface IIdNome : IIdNome<int>
    {
    }

    public interface IId : IId<int>
    {
    }

    public interface IIdNome<T> : IId<T> where T : struct
    {
        string Nome { get; set; }
    }
    public interface IId<T> where T : struct
    {
        T Id { get; set; }
    }

    public class IdNome : IIdNome
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
