using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alma.Core;

namespace Alma.Dominio
{
    public class Perfil : Entidade<int>, IIdNome
    {
        protected Perfil() { }

        public Perfil(string nome, string descricao, bool ativo, bool privado)
        {
            this.Nome = nome;
            this.Descricao = descricao;
            this.Ativo = ativo;
            this.Privado = privado;
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

    }
}
