using Alma.ExampleProject.Application.SecurityModule.Dto;
using Alma.ExampleProject.Application.SecurityModule.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.ApiMSSql.Controllers.v1
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
            var list = await service.List();

            return list;
        }

        [HttpGet("random/insert")]
        public async Task<IList<RoleDto>> Random()
        {
            var rnd = new Random();
            var bunchOfRoles = new List<Task>();
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

                bunchOfRoles.Add(service.Save(p));
            }
            await Task.WhenAll(bunchOfRoles);

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