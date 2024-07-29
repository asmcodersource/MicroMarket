namespace MicroMarket.Services.AuthAPI.Dtos
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        public UserResponseDto(string id, string name, string surname, string middleName, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            MiddleName = middleName;
            Email = email;
        }
    }
}
