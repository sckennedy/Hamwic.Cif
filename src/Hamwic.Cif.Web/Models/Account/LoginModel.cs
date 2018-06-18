﻿using System.ComponentModel.DataAnnotations;

namespace Hamwic.Cif.Web.Models.Account
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress {get; set;}
        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;}
        public string ReturnUrl {get; set;}
    }
}