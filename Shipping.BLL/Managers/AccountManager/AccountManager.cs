
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shipping.BLL.Dtos;
using Shipping.DAL.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Shipping.BLL.Managers
{


    public class AccountManager:IAccountManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AccountManager(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }

        public async Task<string> LoginUser(LoginDtos loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        
            if (user == null || user.IsDeleted == true)
            {
                throw new Exception("User not found.");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                throw new Exception("Invalid email or password.");
            }

            return await GenerateToken(user);

        }

        public async Task LogoutUser()
        {
            await _signInManager.SignOutAsync();
        }


        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("SecretKey").Value ?? string.Empty);
            var Expires = DateTime.Now.AddDays(1);
            var SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var claimsList = await _userManager.GetClaimsAsync(user);
            var token = new JwtSecurityToken(
                claims: claimsList,
                expires: Expires,
                signingCredentials: SigningCredentials);


            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

    }
}
