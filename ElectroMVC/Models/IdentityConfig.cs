//using ElectroMVC.Data;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;

//namespace ElectroMVC.Models
//{
//    public class ApplicationUserManager : UserManager<ApplicationUser>
//    {
//        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
//            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
//        {
//        }

//        public static ApplicationUserManager Create(IServiceProvider serviceProvider)
//        {
//            var userStore = serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

//            var userManager = new ApplicationUserManager(
//                userStore,
//                serviceProvider.GetRequiredService<IOptions<IdentityOptions>>(),
//                serviceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>(),
//                new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() },
//                new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
//                serviceProvider.GetRequiredService<ILookupNormalizer>(),
//                serviceProvider.GetRequiredService<IdentityErrorDescriber>(),
//                serviceProvider,
//                serviceProvider.GetRequiredService<ILogger<UserManager<ApplicationUser>>>()
//            );

//            // Configure userManager as needed

//            // Configure PasswordValidator options
//            userManager.Options.Password.RequireDigit = false;
//            userManager.Options.Password.RequiredLength = 6;
//            userManager.Options.Password.RequireNonAlphanumeric = false;
//            userManager.Options.Password.RequireUppercase = false;
//            userManager.Options.Password.RequireLowercase = false;

//            return userManager;
//        }

//    }

//    public class ApplicationSignInManager : SignInManager<ApplicationUser>
//    {
//        public ApplicationSignInManager(
//            UserManager<ApplicationUser> userManager,
//            IHttpContextAccessor contextAccessor,
//            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
//            IOptions<IdentityOptions> optionsAccessor,
//            ILogger<SignInManager<ApplicationUser>> logger,
//            IAuthenticationSchemeProvider schemes,
//            IUserConfirmation<ApplicationUser> confirmation)
//            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
//        {
//        }

//        public static ApplicationSignInManager Create(IServiceProvider serviceProvider)
//        {
//            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
//            var claimsFactory = serviceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
//            var optionsAccessor = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>();
//            var logger = serviceProvider.GetRequiredService<ILogger<SignInManager<ApplicationUser>>>();
//            var schemes = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
//            var confirmation = serviceProvider.GetRequiredService<IUserConfirmation<ApplicationUser>>();

//            return new ApplicationSignInManager(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation);
//        }
//    }
//}
