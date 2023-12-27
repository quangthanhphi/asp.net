using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ElectroMVC.Models;
using ElectroMVC.Data;
using Microsoft.Owin.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace ElectroMVC.Controllers
{
    
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public static ApplicationUserManager Create(IServiceProvider serviceProvider)
        {
            var userStore = serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

            var userManager = new ApplicationUserManager(
                userStore,
                serviceProvider.GetRequiredService<IOptions<IdentityOptions>>(),
                serviceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() },
                new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
                serviceProvider.GetRequiredService<ILookupNormalizer>(),
                serviceProvider.GetRequiredService<IdentityErrorDescriber>(),
                serviceProvider,
                serviceProvider.GetRequiredService<ILogger<UserManager<ApplicationUser>>>()
            );

            // Configure userManager as needed

            // Configure PasswordValidator options
            userManager.Options.Password.RequireDigit = false;
            userManager.Options.Password.RequiredLength = 1;
            userManager.Options.Password.RequireNonAlphanumeric = false;
            userManager.Options.Password.RequireUppercase = false;
            userManager.Options.Password.RequireLowercase = false;

            return userManager;
        }

    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public static ApplicationSignInManager Create(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var claimsFactory = serviceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var optionsAccessor = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
            var confirmation = serviceProvider.GetRequiredService<IUserConfirmation<ApplicationUser>>();

            return new ApplicationSignInManager(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation);
        }
    }
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //public ApplicationSignInManager SignInManager
        //{
        //    get { return _signInManager ?? HttpContext.RequestServices.GetRequiredService<ApplicationSignInManager>(); }
        //    private set { _signInManager = value; }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get { return _userManager ?? HttpContext.RequestServices.GetRequiredService<ApplicationUserManager>(); }
        //    private set { _userManager = value; }
        //}



        // GET: /<controller>/
        public IActionResult Index()
        {
            var item = _context.Users.ToList();
            return View(item);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Role = new SelectList(_context.Roles.ToList(), "Id", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Username: " + model.UserName);
                Console.WriteLine("Password: " + model.Password);
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                };
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                //var roles = await _userManager.GetRolesAsync(user);
                //Console.WriteLine("User roles: " + roles);
                //if (!roles.Contains("customer"))
                //{
                //    await _userManager.AddToRoleAsync(user, "customer");
                //}
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var createdUser = await _userManager.FindByNameAsync(model.UserName);

                    //// Thêm vào role
                    //await _userManager.AddToRoleAsync(createdUser, "customer");
                    await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: null);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(_context.Roles.ToList(), "Id", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(_context.Roles.ToList(), "Id", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Username: " + model.UserName);
                Console.WriteLine("Password: " + model.Password);
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone
                };
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                //if (!await _userManager.IsInRoleAsync(user, "admin"))
                //{
                //    await _userManager.AddToRoleAsync(user, "admin");
                //    Console.WriteLine("----Thành công thêm role admin----");

                //}
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: null);
                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(_context.Roles.ToList(), "Id", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            Console.WriteLine("Đăng xuất thành công");
            return RedirectToAction("Index", "AdminLogin");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
                Console.WriteLine("---- Lỗi ------");
                return View(model);
            }

            Console.WriteLine("Tài khoản " + model.UserName);
            Console.WriteLine("Mật khẩu " + model.Password);
            Console.WriteLine("Remember " + model.RememberMe);
            Console.WriteLine("returnUrl " + returnUrl);


            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            Console.WriteLine("result: " + result);

            var user = await _userManager.FindByNameAsync(model.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            //await _userManager.AddToRoleAsync(user, roles);
            Console.WriteLine($"User Roles: {string.Join(", ", roles)}");
            if (user != null)
            {
                // Kiểm tra xác nhận email
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                // Kiểm tra xác nhận số điện thoại
                var isPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
                Console.WriteLine($"IsEmailConfirmed: {isEmailConfirmed}");
                Console.WriteLine($"IsPhoneNumberConfirmed: {isPhoneNumberConfirmed}");

                if (!isEmailConfirmed)
                {
                    // Xử lý khi email chưa được xác nhận
                    ModelState.AddModelError("", "Email is not confirmed.");
                    return View(model);
                }

                if (!isPhoneNumberConfirmed)
                {
                    // Xử lý khi số điện thoại chưa được xác nhận
                    ModelState.AddModelError("", "Phone number is not confirmed.");
                    return View(model);
                }

                // Tiếp tục quá trình đăng nhập
            }



            if (result.Succeeded )
            {
                Console.WriteLine("----Thành công----");
                string userId = user.Id;
                Console.WriteLine("----Thành công: user id----" + userId);

                if (user.Email != null)
                {
                    // Kiểm tra và gán role "customer" nếu cần thiết
                    if (!await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "admin");
                        Console.WriteLine("----Thành công thêm role admin----");

                    }
                    // Admin login successful
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Kiểm tra và gán role "customer" nếu cần thiết
                    if (!await _userManager.IsInRoleAsync(user, "customer"))
                    {
                        await _userManager.AddToRoleAsync(user, "customer");
                        Console.WriteLine("----Thành công thêm role customer----");

                    }

                    // Regular user login successful
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                
            }
            

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", new { model.RememberMe });
            }
            Console.WriteLine($"SignInResult: {result}");
            Console.WriteLine($"SignInResult.IsNotAllowed: {result.IsNotAllowed}");
            Console.WriteLine($"SignInResult.IsLockedOut: {result.IsLockedOut}");
            Console.WriteLine($"SignInResult.RequiresTwoFactor: {result.RequiresTwoFactor}");



            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }



        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Modify this to match the desired default redirection path after successful login
            return RedirectToAction("Index", "Admin");
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
