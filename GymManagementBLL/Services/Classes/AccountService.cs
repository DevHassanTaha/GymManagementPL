﻿using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public ApplicationUser? ValidateUser(LoginViewModel input)
        {
            var user = _userManager.FindByEmailAsync(input.Email).Result;
            if (user != null)
            {
                var isPasswordValid = _userManager.CheckPasswordAsync(user, input.Password).Result;
                if (isPasswordValid)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
