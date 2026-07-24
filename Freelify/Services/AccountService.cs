using Freelify.Data;
using Freelify.Models.Entities.Users;
using Freelify.Models.Results;
using Freelify.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Freelify.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AccountService(
     UserManager<ApplicationUser> userManager,
     SignInManager<ApplicationUser> signInManager,
     AppDbContext context,
     IConfiguration configuration,
     EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return result;
         

            result = await _userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
                return result;

            if (model.Role == "Client")
            {
                _context.ClientProfiles.Add(new ClientProfile
                {
                    UserId = user.Id,
                    CompanyName = "",
                    CompanyDescription = "",
                    CompanyLogoUrl = null,
                    AverageRating = 0,
                    ReviewCount = 0
                });
            }
            else if (model.Role == "Freelancer")
            {
                _context.FreelancerProfiles.Add(new FreelancerProfile
                {
                    UserId = user.Id,
                    Bio = "",
                    Experience = "",
                    AverageRating = 0,
                    ReviewCount = 0
                });
            }

            await _context.SaveChangesAsync();
         
           
            await SendConfirmationEmailAsync(user);

            return IdentityResult.Success;
        }
        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var confirmationLink =
                $"{_configuration["BaseUrl"]}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email!,
                "Confirm your email",
                $"<h3>Welcome!</h3>" +
                $"<p>Please confirm your email by clicking the link below:</p>" +
                $"<a href='{confirmationLink}'>Confirm Email</a>");
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "User not found."
                    });
            }
            token = Encoding.UTF8.GetString(
                     WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result;
        }
        public async Task<IdentityResult> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            if (user.EmailConfirmed)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Email is already confirmed."
                });
            }

            await SendConfirmationEmailAsync(user);

            return IdentityResult.Success;
        }

        //_____________login_____________________________________

        public async Task<LoginResult> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "The email or password is incorrect."
                };
            }

            var resultOfCheckPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!resultOfCheckPassword)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "The email or password is incorrect."
                };
            }
             if (!user.EmailConfirmed)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Please confirm your email first."
                };
            }
            if (!user.IsActive)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Your account has been suspended. Please contact the administrator."
                };
            }
           

            await _signInManager.SignInAsync(user, model.RememberMe);

            return new LoginResult
            {
                Success = true,
                Role = (await _userManager.GetRolesAsync(user))[0]
            };
        }



        public async Task LogOutAsync()
        {
            
          await  _signInManager.SignOutAsync();
                        
        }

    }
}