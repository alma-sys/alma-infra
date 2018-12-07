using Alma.Exemplo.Aplicativo.SegurancaModule.Dto;
using System.Collections.Generic;

namespace Alma.Exemplo.Aplicativo.SegurancaModule.Services
{
    public interface IPerfilService
    {
        void Salvar(PerfilDto user);
        IList<PerfilDto> Listar();

    }
}