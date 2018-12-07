using Alma.Dominio;
using Alma.Exemplo.Aplicativo.SegurancaModule.Dto;
using Alma.Exemplo.Dominio.Repositorios;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace Alma.Exemplo.Aplicativo.SegurancaModule.Services
{
    class PerfilService : Alma.Application.ServiceBase, IPerfilService
    {
        private IMapper mapper;
        private IRepositorioDePerfil repositorio;
        private IRepositorioDePermissao repPermissoes;

        public PerfilService(IMapper mapper, IRepositorioDePerfil repositorio, IRepositorioDePermissao repPermissoes)
        {
            this.mapper = mapper;
            this.repositorio = repositorio;
            this.repPermissoes = repPermissoes;

            VerificarPermissoesSistema();
        }

        private const string ROOT = nameof(ROOT);
        private const string PERMISSAO_USUARIOS = nameof(PERMISSAO_USUARIOS);
        private const string PERMISSAO_PERFIS = nameof(PERMISSAO_PERFIS);

        /// <summary>
        /// Verifica se as permissões estão cadastradas e cadastra. 
        /// </summary>
        private void VerificarPermissoesSistema()
        {


            this.repPermissoes.AtualizarPermissoes(new string[] {
                ROOT,
                PERMISSAO_PERFIS,
                PERMISSAO_USUARIOS
            });

        }

        public IList<PerfilDto> Listar()
        {
            var lista = repositorio.Listar();
            var dto = this.mapper.Map<List<PerfilDto>>(lista);
            return dto;
        }

        public void Salvar(PerfilDto dto)
        {
            var perfil = repositorio.ObterPorNome(dto.Nome);
            if (perfil == null)
                perfil = new Perfil(dto.Nome, dto.Descricao, dto.Ativo);
            else
            {
                perfil.DefinirDescricao(dto.Descricao);
                perfil.DefinirNome(dto.Nome);


                if (dto.Ativo)
                    perfil.Ativar();
                else
                    perfil.Desativar();
            }


            //como este é um exemplo, associar aleatoriamente permissões
            var permissoes = repPermissoes.Listar();
            var rnd = new Random();
            foreach (var p in permissoes)
            {
                if (rnd.Next(0, 2) == 1)
                {
                    perfil.AssociarPermissao(p);
                }
            }

            this.repositorio.Salvar(perfil);

            var retorno = this.mapper.Map<PerfilDto>(perfil);
            //return retorno;
        }
    }
}
