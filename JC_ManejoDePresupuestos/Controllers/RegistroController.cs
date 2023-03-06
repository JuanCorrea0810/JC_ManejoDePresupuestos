using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using ManejoDePresupuestos.Utilidades.Administracion_DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class RegistroController : Controller
    {
        private readonly UserManager<NewIdentityUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<NewIdentityUser> signInManager;

        public IGetUserInfo GetUserInfo { get; set; }
        private readonly IMapper mapper;
        private readonly IServicioEmail servicioEmail;

        public RegistroController(UserManager<NewIdentityUser> userManager,
            ApplicationDbContext context,
            SignInManager<NewIdentityUser> signInManager,
            IGetUserInfo getUserInfo,
            IMapper mapper,
            IServicioEmail servicioEmail
            )
        {
            this.userManager = userManager;
            this.context = context;
            this.signInManager = signInManager;
            GetUserInfo = getUserInfo;
            this.mapper = mapper;
            this.servicioEmail = servicioEmail;
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Sign_Up()
        {
            
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Sign_Up(InfoUser credentials)
        {
            
            if (!ModelState.IsValid)
            {
                return View(credentials);
            }
            var User = new NewIdentityUser { UserName = credentials.Email, Email = credentials.Email };
            var result = await userManager.CreateAsync(User, credentials.Password);
            if (result.Succeeded)
            {
                //Se agrega el usuario al rol User y se le agregan Claims para que sea más fácil identificarlo
                await userManager.AddToRoleAsync(User, "User");
                await userManager.AddClaimAsync(User, new Claim(ClaimTypes.Role, "User"));

                //Implementación de confirmación de email en el registro
                var code = await userManager.GenerateEmailConfirmationTokenAsync(User);
                var urlRetorno = Url.Action("ConfirmarEmail", "Registro", new { IdUser = User.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await servicioEmail.ConfirmarCuenta(credentials.Email, urlRetorno);

                return RedirectToAction("GraciasPorCrearCuenta", "Registro", new { email = credentials.Email });
            }
            ValidarErrores(result);
            return View(credentials);

        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GraciasPorCrearCuenta([FromQuery] string email)
        {
            return View("GraciasPorCrearCuenta", email);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn(string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]

        public async Task<ActionResult> LogIn(LoginAuth credentials, string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return View(credentials);
            }
            var result = await signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, credentials.RememberMe,
                                                               lockoutOnFailure: true);
            var user = await userManager.FindByEmailAsync(credentials.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(credentials.Email), "El usuario no es válido");
                return View(credentials);
            }
            var LockedOut = await userManager.IsLockedOutAsync(user);
            if (LockedOut)
            {
                ModelState.AddModelError(nameof(credentials.Email), "Cuenta bloqueda temporalmente. Intente más tarde");
                return View(credentials);
            }

            if (!result.Succeeded)
            {
                var FailedCount = await userManager.GetAccessFailedCountAsync(user);
                if (FailedCount < 4)
                {
                    ModelState.AddModelError(nameof(credentials.Password), "Datos Incorrectos. Vuelva a intentarlo.");
                    return View(credentials);
                }
                else if (FailedCount == 4)
                {
                    ModelState.AddModelError(nameof(credentials.Password), "Solo le queda un intento, si vuelve a fallar se bloqueará la cuenta temporalmente");
                    return View(credentials);
                }
                else
                {
                    ModelState.AddModelError(nameof(credentials.Password), "Cuenta bloqueda temporalmente. Intente más tarde");
                    return View(credentials);
                }
                
            }
            await signInManager.SignInAsync(user, isPersistent: credentials.RememberMe);
            return LocalRedirect(ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> CompletarPerfil()
        {
            var IdUser = await GetUserInfo.GetId();
            var User = await userManager.FindByIdAsync(IdUser);
            if (User == null)
            {
                return View("ErrorGenerico");
            }

            var modelo = mapper.Map<PutUsersDTO>(User);
            if (modelo.Country == null)
            {
                modelo.Country = "";
            }
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompletarPerfil(PutUsersDTO putUsersDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(putUsersDTO);
            }
            var IdUser = await GetUserInfo.GetId();
            var User = await userManager.FindByIdAsync(IdUser);

            if (User == null)
            {
                return View("ErrorGenerico");
            }
            //Si el DNI en la base de datos es diferente de  NULL entonces significa que el usuario ya había ingresado datos personales anteriormente
            // Si el DNI es igual a NULL significa que es la primera vez rellenando los campos de datos personales
            if (User.Dni != null)
            {
                //Si es el mismo DNI entonces el usuario se actualiza sin problemas
                if (User.Dni == putUsersDTO.Dni)
                {
                    User = mapper.Map(putUsersDTO, User);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }

                //Sino es el mismo entonces se busca en la base de datos para saber que no haya otro usuario con ese nuevo DNI
                var ExistsDNI = await userManager.Users.AnyAsync(x => x.Dni == putUsersDTO.Dni);
                if (ExistsDNI)
                {
                    ModelState.AddModelError(nameof(putUsersDTO.Dni), "El DNI no es válido");
                    return View(putUsersDTO);
                }
            }
            //Se insertan los campos que nos mandaron
            User = mapper.Map(putUsersDTO, User);
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

            

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult OlvidoContraseña()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> OlvidoContraseña(ForgetPassword emailDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(emailDTO);
            }
            var usuario = await userManager.FindByEmailAsync(emailDTO.Email);
            if (usuario == null)
            {
                return View("ErrorGenerico");
            }

            var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var urlRetorno = Url.Action("ResetearContraseña", "Registro", new { code = codigo }, protocol: HttpContext.Request.Scheme);

            await servicioEmail.RecuperarContraseña(emailDTO.Email, urlRetorno);
            return View("EnvioCorreoOlvidoContraseña", emailDTO.Email);
            

        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetearContraseña([FromQuery] string code)
        {
            ViewBag.Code = code;
            return View();
        }


        [AllowAnonymous]
        [HttpPost]

        public async Task<ActionResult> ResetearContraseña(ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPassword);
            }
            var usuario = await userManager.FindByEmailAsync(resetPassword.Email);
            if (usuario == null)
            {
                return View("ErrorGenerico");
            }

            var resultado = await userManager.ResetPasswordAsync(usuario, resetPassword.Code, resetPassword.NewPassword);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ValidarErrores(resultado);
            return View(resetPassword);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail([FromQuery] string IdUser, [FromQuery] string code)
        {

            if (IdUser == null || code == null)
            {
                return View("ErrorGenerico");
            }

            var usuario = await userManager.FindByIdAsync(IdUser);
            if (usuario == null)
            {
                return View("ErrorGenerico"); ;
            }

            var resultado = await userManager.ConfirmEmailAsync(usuario, code);
            ValidarErrores(resultado);
            return resultado.Succeeded ? View() : View("ErrorGenerico");
        }

        [HttpGet]
        public ActionResult CambiarContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarContraseña(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View(changePassword);
            }
            var IdUser = await GetUserInfo.GetId();
            var usuario = await userManager.FindByIdAsync(IdUser);
            if (usuario == null)
            {
                return View("ErrorGenerico");
            }

            var resultado = await userManager.ChangePasswordAsync(usuario, changePassword.CurrentPassword, changePassword.NewPassword);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ValidarErrores(resultado);
            return View(changePassword);
        }

        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        //Método que sirve para validar de manera remota desde el cliente si el usuario ya existe, se debe utilizar AllowAnonymous para que funcione
        //JavaScript hace una petición HTTPGET a esta ruta antes de entrar a la acción de Sign_Up
        public async Task<IActionResult> CorreoYaExiste(string Email) 
        {
            var ExisteUsuario = await userManager.FindByEmailAsync(Email);
            if (ExisteUsuario != null )
            {
                return Json("Ya existe un usuario con este nombre");
            }
            return Json(true);
        }
    }
}
