using Alma.ExampleProject.Application.UserModule.Dto;
using Alma.ExampleProject.Application.UserModule.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alma.ExampleProject.ApiMongo.Controllers.v1
{
    [Produces("application/json", "text/xml")]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }
        // GET api/values
        [HttpGet]
        public async Task<IList<UserDto>> Get()
        {
            var lista = await service.List();

            return lista;
        }

        [HttpGet("random/insert")]
        public async Task<IList<UserDto>> Random()
        {
            var rnd = new Random();

            var bunchOfUsers = new List<Task>();
            for (var i = 0; i < 5; i++)
            {
                var number = rnd.Next(999, 99999);
                var usr = new UserDto()
                {
                    DomainUser = $"user{number}",
                    Email = $"user{number}@mailinator.com",
                    Address = $"00 User {number} Street",
                    Name = $"Fulano {number}",
                    PersonUId = number,
                    FamilyName = "Family Name",
                    Telephone = number.ToString(),
                    LastAccess = DateTime.Now
                };

                bunchOfUsers.Add(service.Save(usr));
            }

            await Task.WhenAll(bunchOfUsers);

            var lista = await service.List();

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
