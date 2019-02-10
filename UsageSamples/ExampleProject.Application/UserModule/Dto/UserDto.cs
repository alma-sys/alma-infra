using System;

namespace Alma.ExampleProject.Application.UserModule.Dto
{
    public class UserDto
    {
        public UserDto()
        {

        }

        public long Id { get; set; }

        public long PersonUId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DomainUser { get; set; }
        public DateTime? LastAccess { get; set; }


    }
}