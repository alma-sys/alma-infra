using System;

namespace Alma.ApiExtensions.Controllers
{
    public abstract class GetArgs
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
