using Freelify.Data;
using Freelify.Models.Entities.Users;
using Freelify.Models.Results;
using Freelify.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelify.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly AppDbContext _context;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            return await _userManager.AddToRoleAsync(user, model.Role);
        }
        public async Task<LoginResult> LoginAsync(LoginViewModel model)
        {
          var user=  await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                return new LoginResult { Success = false , Message = "The email or password is incorrect." };
            }

          var ResultofCheckPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if(!ResultofCheckPassword)
            {
                return new LoginResult { Success = false, Message = "The email or password is incorrect." };

             
            }
            
            
            await _signInManager.SignInAsync(user, model.RememberMe);
            return new LoginResult { Success = true , Role= (await _userManager.GetRolesAsync(user))[0] };



        }

        //public async Task<bool> CompleteClientProfileAsync(string userId, CompleteClientProfileViewModel model)
        //{
        //    if (await _context.ClientProfiles.AnyAsync(cp => cp.UserId == userId))
        //        return false; // already completed — don't create a duplicate row

        //    _context.ClientProfiles.Add(new ClientProfile
        //    {
        //        UserId = userId,
        //        CompanyName = model.CompanyName,
        //        CompanyLogoUrl = model.CompanyLogoUrl,
        //        CompanyDescription = model.CompanyDescription
        //    });
        //    await _context.SaveChangesAsync();
        //    return true;
        //}


        public async Task LogOutAsync()
        {
            
          await  _signInManager.SignOutAsync();
                        
        }

    }
}