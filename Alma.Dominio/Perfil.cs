using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Core;

namespace Alma.Dominio
{
    public class Perfil : Entidade, IIdNome
    {
        protected Perfil() { }
        public Perfil(string nome, string descricao, bool ativo = true)
        {
            this.Ativo = ativo;

            this.DefinirNome(nome);
            this.DefinirDescricao(descricao);
        }

        public virtual string Nome { get; protected set; }
        public virtual string Descricao { get; protected set; }
        public virtual bool Ativo { get; protected set; }
        public virtual bool Privado { get; protected set; }

        protected IList<Permissao> _permissoes = new List<Permissao>();
        public virtual IReadOnlyList<Permissao> Permissoes => new ReadOnlyCollection<Permissao>(_permissoes);

        public virtual void AssociarPermissao(params Permissao[] permissao)
        {
            if (permissao == null)
                throw new ArgumentNullException(nameof(permissao));

            lock (_permissoes)
            {
                foreach (var p in permissao)
                {
                    if (!_permissoes.Contains(p))
                        _permissoes.Add(p);
                }
            }
        }

        public virtual void RemoverPermissao(params Permissao[] permissao)
        {
            if (permissao == null)
                throw new ArgumentNullException(nameof(permissao));

            lock (_permissoes)
            {
                foreach (var p in permissao)
                {
                    if (_permissoes.Contains(p))
                        _permissoes.Remove(p);
                }
            }

        }

        public virtual void Ativar()
        {
            this.Ativo = true;
        }

        public virtual void Desativar()
        {
            this.Ativo = false;
        }

        public virtual void DefinirNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException(nameof(nome));

            int tamanho = 50;
            if (nome.Length > tamanho)
                throw new ArgumentException($"Nome deve ter no máximo {tamanho} caracteres.");

            this.Nome = nome;
        }

        public virtual void DefinirDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException(nameof(descricao));

            int tamanho = 500;
            if (descricao.Length > tamanho)
                throw new ArgumentException($"Descrição deve ter no máximo {tamanho} caracteres.");

            this.Descricao = descricao;
        }
    }
}
