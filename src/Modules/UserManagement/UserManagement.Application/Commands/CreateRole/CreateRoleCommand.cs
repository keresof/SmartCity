using MediatR;
namespace UserManagement.Application.Commands.CreateRole
{
    public class CreateRoleCommand 
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}