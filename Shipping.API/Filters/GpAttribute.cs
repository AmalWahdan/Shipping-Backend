using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shipping.BLL.Managers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shipping.DAL.Data.Models;
using Shipping.BLL.Dtos;

namespace Shipping.API.Filters
{
    public class GpAttribute: ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly IGroupPermissionManager _groupPermissionManager ;
        public GpAttribute(IConfiguration configuration,
        IGroupPermissionManager groupPermissionManager)
        {
            _configuration = configuration;
            _groupPermissionManager = groupPermissionManager;
            
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.HttpContext.Request.Method.ToLower();
            var controllerName = context.Controller.GetType().Name.ToLower();
            string operation = string.Empty;
            var token = getToken(context);
            var groupId = GetGroupId(token);
            bool isValid = true;
            List<GroupPermissionDto> groupPermissions = new List<GroupPermissionDto>();
            if (groupId !=null)
            {
                int id = int.Parse(groupId);
                var permissions = _groupPermissionManager.HasPermission(id);
                if (permissions != null)
                {
                    groupPermissions = permissions.Result.Where(p => controllerName.Contains(p.Name.ToLower())).ToList();
                }
            }

            if (groupPermissions != null)
            {
                switch (actionName)
                {
                    case "post":
                        isValid = groupPermissions.FirstOrDefault(gp => gp.Action == "Add") == null ? false : true;


                        break;
                    case "get":
                        isValid = groupPermissions.FirstOrDefault(gp => gp.Action == "Show") == null ? false : true;

                        break;
                    case "put":
                        isValid = groupPermissions.FirstOrDefault(gp => gp.Action == "Edit") == null ? false : true;

                        break;
                    case "delete":
                        isValid = groupPermissions.FirstOrDefault(gp => gp.Action == "Delete") == null ? false : true;
                        break;
                }
            }
            else
            {
                isValid = false;
            }
            if (isValid == false)
            {
                context.ModelState.AddModelError(controllerName, "employee not authorized ");
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

        }
        string GetGroupId(string token)
        {
            var secret = _configuration["SecretKey"] ?? string.Empty;
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var groupId = handler.ValidateToken(token, validations, out var tokenSecure).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ElementAt(0);
            return groupId;
        }
        string getToken(ActionExecutingContext context)
        {
            return context.HttpContext.Request.Headers["Authorization"][0].Split(" ")[1];
        }
    }
}
