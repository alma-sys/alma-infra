using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Infra.Controllers
{
    public class GetArgs
    {
        public GetArgs()
        {
            Pagina = 1;
            TamanhoPagina = 10;
            ResultadoPaginado = true;
        }
        public int Pagina { get; set; }

        public Boolean ResultadoPaginado { get; set; }
        public int TamanhoPagina { get; set; }
    }
}
