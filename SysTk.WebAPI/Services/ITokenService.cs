﻿namespace SysTk.WebAPI.Services
{
    public interface ITokenService
    {
        Task<dynamic> GenerateToken(string username);
        Task<bool> IsValidUsernameAndPassword(string userName, string password);
    }
}