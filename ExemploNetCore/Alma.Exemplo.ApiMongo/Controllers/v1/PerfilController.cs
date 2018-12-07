using Alma.Exemplo.Aplicativo.SegurancaModule.Dto;
using Alma.Exemplo.Aplicativo.SegurancaModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Alma.Exemplo.ApiMongo.Controllers.v1
{
    [Produces("application/json", "text/xml")]
    [Route("api/v1/[controller]")]
    public class PerfilController : Controller
    {
        private IPerfilService service;

        public PerfilController(IPerfilService service)
        {
            this.service = service;

        }

        [HttpGet]
        public IList<PerfilDto> Get()
        {
            var lista = service.Listar();

            return lista;
        }

        [HttpGet("random/insert")]
        public IList<PerfilDto> Random()
        {
            var rnd = new Random();

            for (var i = 0; i < 5; i++)
            {
                var numero = rnd.Next(999, 99999);
                var p = new PerfilDto()
                {
                    Ativo = true,
                    Privado = false,
                    Nome = $"PERFIL_{numero}",
                    Descricao = $"Perfil {numero}"
                };

                service.Salvar(p);
            }

            var lista = service.Listar();

            return lista;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}