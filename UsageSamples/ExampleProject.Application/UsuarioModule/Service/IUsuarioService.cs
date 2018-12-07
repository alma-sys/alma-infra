using Alma.Exemplo.Aplicativo.UsuarioModule.Dto;
using System.Collections.Generic;

namespace Alma.Exemplo.Aplicativo.UsuarioModule.Service
{
    public interface IUsuarioService
    {
        UsuarioDto Salvar(UsuarioDto user);
        IList<UsuarioDto> Listar();
    }
}