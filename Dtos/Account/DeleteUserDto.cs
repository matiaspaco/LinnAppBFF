using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class DeleteUserDto
    {
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        public string? CurrentPassword { get; set; }
    }
}