using Alma.ExampleProject.Application.SecurityModule.Dto;
using Alma.ExampleProject.Application.SecurityModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.ApiMongo.Controllers.v1
{
    [Produces("application/json", "text/xml")]
    [Route("api/v1/[controller]")]
    public class RoleController : Controller
    {
        private IRoleService service;

        public RoleController(IRoleService service)
        {
            this.service = service;

        }

        [HttpGet]
        public async Task<IList<RoleDto>> Get()
        {
            var lista = await service.List();

            return lista;
        }

        [HttpGet("random/insert")]
        public async Task<IList<RoleDto>> Random()
        {
            var rnd = new Random();

            for (var i = 0; i < 5; i++)
            {
                var number = rnd.Next(999, 99999);
                var p = new RoleDto()
                {
                    Enabled = true,
                    Privado = false,
                    Name = $"ROLE_{number}",
                    Description = $"Role {number}"
                };

                await service.Save(p);
            }

            var list = await service.List();

            return list;
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