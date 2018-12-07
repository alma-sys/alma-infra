namespace Alma.Exemplo.Aplicativo.SegurancaModule.Dto
{
    public class PerfilDto
    {
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual bool Privado { get; set; }

    }
}
