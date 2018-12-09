namespace Alma.ExampleProject.Application.SecurityModule.Dto
{
    public class RoleDto
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual bool Privado { get; set; }

    }
}
