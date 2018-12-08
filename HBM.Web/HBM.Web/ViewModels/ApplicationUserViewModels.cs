namespace HBM.Web.ViewModels
{
    public class UserShowViewModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public int Articles { get; set; }
        public int Comments { get; set; }
        public int Banned { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string Avatar { get; set; }       
    }
}