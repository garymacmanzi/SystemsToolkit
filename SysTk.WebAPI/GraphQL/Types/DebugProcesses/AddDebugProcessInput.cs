﻿using System.ComponentModel.DataAnnotations;

namespace SysTk.WebAPI.GraphQL.Types.DebugProcesses
{
    public class AddDebugProcessInput
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
