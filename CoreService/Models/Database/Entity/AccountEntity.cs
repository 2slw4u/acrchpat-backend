﻿using CoreService.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Database.Entity
{
    public class AccountEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [DefaultValue(0)]
        public double Balance { get; set; }
        [Required]
        [DefaultValue(AccountStatus.Opened)]
        public AccountStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
