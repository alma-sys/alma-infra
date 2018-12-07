using Alma.Exemplo.Aplicativo.UsuarioModule.Dto;
using Alma.Exemplo.Dominio.Entidades;
using Alma.Exemplo.Dominio.Repositorios;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Exemplo.Aplicativo.UsuarioModule.Service
{
    class UsuarioService : IUsuarioService
    {
        private IMapper mapper;
        private IRepositorioDeUsuario repositorio;

        public UsuarioService(IMapper mapper, IRepositorioDeUsuario repUsuario)
        {
            this.mapper = mapper;
            this.repositorio = repUsuario;
        }
        public IList<UsuarioDto> Listar()
        {
            var lista = repositorio.Listar();
            var dto = this.mapper.Map<List<UsuarioDto>>(lista);
            return dto;
        }

        public UsuarioDto Salvar(UsuarioDto dto)
        {
            var usuarioPai = repositorio.Listar().FirstOrDefault(); // pegando qualquer um como exemplo


            var usuario = repositorio.GetPorEmailOuUser(dto.Email);
            if (usuario == null)
                usuario = new Usuario(dto.PersonUId, dto.Nome, dto.Sobrenome, dto.Endereco, dto.Email, dto.Telefone, dto.DomainUser);
            else
                usuario.Atualizar(dto.Nome, dto.Sobrenome, dto.Endereco, dto.Telefone, dto.DomainUser);

            usuario.DefinirUsuarioPai(usuarioPai);

            usuario.AtualizarUltimoAcesso(DateTime.Now);

            usuario = this.repositorio.Salvar(usuario);

            var retorno = this.mapper.Map<UsuarioDto>(usuario);
            return retorno;
        }

    }
}
