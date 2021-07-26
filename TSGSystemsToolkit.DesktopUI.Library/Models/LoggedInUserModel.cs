﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TSGSystemsToolkit.DesktopUI.Library.Models
{
    public class LoggedInUserModel : ILoggedInUserModel
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Roles { get; set; }

        public void ResetUserModel()
        {
            Token = "";
            Id = "";
            EmailAddress = "";
            CreatedDate = DateTime.MinValue;
        }
    }
}