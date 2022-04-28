using Context.DAL;

namespace API.ResponseModels
{
    public class LoginResponse
    {
        public User User { get; set; }

        public AuthenticationInformation Authentication { get; set; }

    }
}
