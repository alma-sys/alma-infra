using System;

namespace Alma.Exemplo.Aplicativo.UsuarioModule.Dto
{
    public class UsuarioDto
    {
        public UsuarioDto()
        {

        }

        public long Id { get; set; }

        public long PersonUId { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Endereco { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string DomainUser { get; set; }
        public DateTime? UltimoAcesso { get; set; }


    }
}