using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoLatex.Database;
using Microsoft.AspNetCore.Mvc;

namespace CoLatex.Authentication
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        private static readonly Regex UsernameRegex = new Regex(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$");
        private UserRepository _userRepository;

        public AuthenticationController(UserRepository userRepository)
        {
            _userRepository = userRepository;
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