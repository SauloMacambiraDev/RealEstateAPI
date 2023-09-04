using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using RealStateAPI.DTOs;
using RealStateAPI.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;

namespace RealStateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private RealEStateDbContext _dbContext;
        private SecurityHelper _securityHelper;
        private IConfiguration _configuration;

        public UsersController(RealEStateDbContext dbContext, SecurityHelper securityHelper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _securityHelper = securityHelper;
            _configuration = configuration;
        }

        // DEPRECATED
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetEncryptedUserPassword(int id)
        {
            var userExists = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(userExists == null)
            {
                return NotFound();
            }

            var userPassword = _securityHelper.DecryptPassword(userExists.Password);

            return Ok(userPassword);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var userExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (userExists != null)
                {
                    return BadRequest("User with same already exists. Please, use another one");
                }

                byte[] hash;
                byte[] salt;
                _securityHelper.CreateHashPassword(user.Password, out hash, out salt);

                user.Password = Convert.ToBase64String(hash);
                user.PasswordSalt = Convert.ToBase64String(salt);
                Role? defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Description.Equals("Default"));
                if(defaultRole != null) user.RoleId = defaultRole.Id;


                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();

            } catch (DbUpdateException ex)
            {

                foreach (var error in ex.Entries)
                {
                    if(ex.InnerException is SqlException sqlException)
                    {
                        switch (sqlException.Number)
                        {
                            case 2627:
                                return BadRequest("User with same already exists.Please, use another one");
                            default:
                                return BadRequest("An unknown error ocurred while trying to add user. Contact admin for more information");
                        }
                    }
                }

            }


            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SignIn(LoginCredentialsDTO loginCredentials)
        {
            // Check User Credentials
            var user = await AreUserCredentialsValid(loginCredentials);
            if(user == null) return BadRequest("Email or password didn't match");

            // Generate bearer token to end user

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (user.Role != null) claims.Add(new Claim(ClaimTypes.Role, user.Role.Description));

            var token = new JwtSecurityToken(issuer: _configuration["JWT:Issuer"],
                                             audience: _configuration["JWT:Audience"],
                                             claims: claims,
                                             expires: DateTime.Now.AddMinutes(60),
                                             signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(jwt);
        }

        #region Private Methods
        private async Task<User> AreUserCredentialsValid(LoginCredentialsDTO loginCredentials)
        {
            var userExists = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Email == loginCredentials.Email);

            if (userExists == null)
            {
                return null;
            }

            var passwordMatched = _securityHelper.VerifyPassword(loginCredentials.Password,
                                                                 Convert.FromBase64String(userExists.Password),
                                                                 Convert.FromBase64String(userExists.PasswordSalt));

            if (!passwordMatched)
            {
                return null;
            }

            return userExists;
        }

        #endregion
    }
}
