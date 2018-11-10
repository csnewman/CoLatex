using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoLatex.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoLatex.Authentication
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        public static readonly string JwtSecret = "ThisIsSomeSpecialSecret";
        private static readonly Regex UsernameRegex = new Regex(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$");
        private UserRepository _userRepository;

        public AuthenticationController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<LoginResponseModel> LoginAsync([FromBody] LoginModel model)
        {
            UserDbModel dbModel = await _userRepository.GetUserByUsername(model.Username);

            if (dbModel == null || !BCrypt.Net.BCrypt.Verify(model.Password, dbModel.Password))
            {
                return new LoginResponseModel
                {
                    Success = false
                };
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(JwtSecret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("username", model.Username),
                    new Claim("name", dbModel.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseModel
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }


        [HttpPost("register")]
        public async Task<RegisterResponseModel> RegisterAsync([FromBody] RegisterModel model)
        {
            if (await _userRepository.GetUserByUsername(model.Username) != null)
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.UsernameInUse
                };
            }

            if (await _userRepository.GetUserByEmail(model.Email) != null)
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.EmailInUse
                };
            }

            if (string.IsNullOrWhiteSpace(model.Username) || !UsernameRegex.IsMatch(model.Username))
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.InvalidUsername
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.InvalidName
                };
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.InvalidEmail
                };
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return new RegisterResponseModel
                {
                    Success = false,
                    Error = RegisterResponseModel.ErrorReason.InvalidPassword
                };
            }

            await _userRepository.AddUser(new UserDbModel
            {
                Username = model.Username,
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });

            return new RegisterResponseModel
            {
                Success = true
            };
        }
    }
}