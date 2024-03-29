﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysTk.WebApi.Data.Models
{
    public class FtpCredentials : BaseEntity
    {
        private string _stationId;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        [MinLength(5)]
        public string StationId
        {
            get { return _stationId.ToUpper(); }
            set { _stationId = value.ToUpper(); }
        }


        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public Station Station { get; set; }
    }
}
