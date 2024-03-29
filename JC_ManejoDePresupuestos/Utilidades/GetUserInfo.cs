﻿using ManejoDePresupuestos.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ManejoDePresupuestos.Utilidades
{
    public class GetUserInfo : IGetUserInfo
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<NewIdentityUser> userManager;

        public GetUserInfo(IHttpContextAccessor httpContextAccessor,UserManager<NewIdentityUser> userManager )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public  async Task<string> GetId()
        {
            var email_Claim = httpContextAccessor.HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();
            var email = email_Claim.Value;
            var User = await userManager.FindByEmailAsync(email);
            var IdUser = User.Id;

            return IdUser;
        }

    }
}
