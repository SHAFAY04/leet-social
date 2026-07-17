public class RolesAttribute : Attribute
{
    public string[] Roles {get;}
    public RolesAttribute(params string[] roles)
    {
        Roles=roles;
    }
}