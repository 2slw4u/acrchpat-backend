﻿using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Account
{
    public class OpenNewAccountRequest
    {
        [FromBody]
        [Required]
        public AccountCreateModel NewAccount { get; set; }
    }
}
