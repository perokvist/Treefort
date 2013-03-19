namespace Treefort.WebApi.Security
{
    public interface IAuthenticationService
    {
        bool Authenticate(string username, string password);
    }
}