using System.Security.Principal;

namespace Treefort.WebApi.Security
{
    public interface IPrincipalProvider
    {
        IPrincipal CreatePrincipal(string username, string password);
    }
}