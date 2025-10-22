namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; } = Role.None;

        public Client(int id, string name, string emailAddress, string password) : base(id, name)
        {
            Id = id;
            Name = name;
            EmailAddress=emailAddress;
            Password=password;
        }
    }
}