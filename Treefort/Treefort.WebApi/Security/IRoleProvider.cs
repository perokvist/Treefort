namespace Treefort.WebApi.Security
{
    public interface IRoleProvider
    {
        string[] GetRoles(string userName);
    }
}