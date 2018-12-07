using Alma.Exemplo.Aplicativo.UsuarioModule.Dto;
using Alma.Exemplo.Aplicativo.UsuarioModule.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Alma.Exemplo.ApiMSSql.Controllers.v1
{
    [Produces("application/json", "text/xml")]
    [Route("api/v1/[controller]")]
    public class UsuarioController : Controller
    {
        private IUsuarioService service;

        public UsuarioController(IUsuarioService service)
        {
            this.service = service;
        }
        // GET api/values
        [HttpGet]
        public IList<UsuarioDto> Get()
        {
            var lista = service.Listar();

            return lista;
        }

        [HttpGet("random/insert")]
        public IList<UsuarioDto> Random()
        {
            var rnd = new Random();

            for (var i = 0; i < 5; i++)
            {
                var numero = rnd.Next(999, 99999);
                var usr = new UsuarioDto()
                {
                    DomainUser = $"usuario{numero}",
                    Email = $"usuario{numero}@mailinator.com",
                    Endereco = $"Rua do Usuário {numero}, S/N",
                    Nome = $"Fulano {numero}",
                    PersonUId = numero,
                    Sobrenome = "Sobrenome",
                    Telefone = numero.ToString(),
                    UltimoAcesso = DateTime.Now
                };

                service.Salvar(usr);
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
