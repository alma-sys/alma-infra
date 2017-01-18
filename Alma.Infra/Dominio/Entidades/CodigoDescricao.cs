using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Infra.Dominio.Entidades
{
    public class CodigoDescricao
    {
        public CodigoDescricao(String codigo, String descricao)
        {
            this.Codigo = codigo;
            this.Descricao = descricao;
        }
        public String Codigo { get; set; }
        public String Descricao { get; set; }
        public override string ToString()
        {
            return Descricao;
        }
    }
}
