using Microsoft.AspNetCore.Authorization;

namespace Middleware.Utils.Helpers
{
    /// <summary>
    /// Class responsible for creating access policies
    /// </summary>
    public static class Policies
    {
        public static void CreatePolicies(AuthorizationOptions options)
        {
            //Users
            options.AddPolicy(Permission.Users.Viwer, policy => policy.RequireAssertion(context =>
                   context.User.HasClaim(c => Permission.Users.Viwer.Equals(c.Type) || Permission.ADMIN.Equals(c.Value) || Permission.Users.Manager.Equals(c.Type))
               ));
            options.AddPolicy(Permission.Users.Manager, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Users.Manager.Equals(c.Type) || Permission.ADMIN.Equals(c.Value))
            ));

            //Roles
            options.AddPolicy(Permission.Roles.Viwer, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Roles.Viwer.Equals(c.Type) || Permission.ADMIN.Equals(c.Value) || Permission.Roles.Manager.Equals(c.Type))
           ));
            options.AddPolicy(Permission.Roles.Manager, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Roles.Manager.Equals(c.Type) || Permission.ADMIN.Equals(c.Value))
            )); ;

            //Accounts
            options.AddPolicy(Permission.Accounts.Viwer, policy => policy.RequireAssertion(context =>
               context.User.HasClaim(c => Permission.Accounts.Viwer.Equals(c.Type) || Permission.ADMIN.Equals(c.Value) || Permission.Accounts.Manager.Equals(c.Type))
           ));
            options.AddPolicy(Permission.Accounts.Manager, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Accounts.Manager.Equals(c.Type) || Permission.ADMIN.Equals(c.Value))
            ));

            //Requests
            options.AddPolicy(Permission.Requests.Viwer, policy => policy.RequireAssertion(context =>
               context.User.HasClaim(c => Permission.Requests.Viwer.Equals(c.Type) || Permission.ADMIN.Equals(c.Value) || Permission.Requests.Manager.Equals(c.Type))
           ));
            options.AddPolicy(Permission.Requests.Manager, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Requests.Manager.Equals(c.Type) || Permission.ADMIN.Equals(c.Value))
            ));

            //Oders
            options.AddPolicy(Permission.Orders.Viwer, policy => policy.RequireAssertion(context =>
              context.User.HasClaim(c => Permission.Orders.Viwer.Equals(c.Type) || Permission.ADMIN.Equals(c.Value) || Permission.Orders.Manager.Equals(c.Type))
          ));
            options.AddPolicy(Permission.Orders.Manager, policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => Permission.Orders.Manager.Equals(c.Type) || Permission.ADMIN.Equals(c.Value))
            ));
        }
    }
}
