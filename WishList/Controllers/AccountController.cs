﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Models;
using WishList.Models.AccountViewModels;

namespace WishList.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet,AllowAnonymous]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost,AllowAnonymous]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", registerViewModel);
            }
            var newUser = new ApplicationUser
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email
            };

            var result = _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (!result.Result.Succeeded)
            {
                foreach (var item in result.Result.Errors)
                {
                    ModelState.AddModelError("Password",item.Description);
                }
                return View("Register", registerViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet,AllowAnonymous]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost,AllowAnonymous,ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", loginViewModel);
            }
            Task<Microsoft.AspNetCore.Identity.SignInResult> r = _signInManager.PasswordSignInAsync(loginViewModel.Email,
                loginViewModel.Password,
                false,
                false);
            if (!r.Result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Login",loginViewModel);
            }
            return RedirectToAction("Index", "Item");
        }

        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
