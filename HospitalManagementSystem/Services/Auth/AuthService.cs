using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.Auth;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.Auth.Token;
using HospitalManagementSystem.Services.Interfaces.Auth;
using Serilog;
using HospitalManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Models;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;






namespace HospitalManagementSystem.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRespository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserManagementRespository _userManagementRespository;

        public AuthService(
            IAuthRespository authRepository,
            ITokenService tokenService,
            IUserManagementRespository userManagementRespository)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _userManagementRespository = userManagementRespository;
        }

        /// <summary>
        /// Authenticates a user and generates access and refresh tokens
        /// </summary>
        /// <param name="loginRequestDto">Login credentials</param>
        /// <returns>Token response with authentication details</returns>
        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            Log.Information("Login attempt for email: {Email}", loginRequestDto.Email);

            var user = await _userManagementRespository.ValidateCredentialsAsync(
                loginRequestDto.Email,
                loginRequestDto.Password);

            if (user == null)
            {
                Log.Warning("Login failed - Invalid credentials for email: {Email}", loginRequestDto.Email);
                return new TokenResponseDto
                {
                    Message = "Email or Password is incorrect!",
                    IsAuthenticated = false
                };
            }

            if (!user.IsActive)
            {
                Log.Warning("Login failed - Inactive user account: {UserId}", user.UserId);
                return new TokenResponseDto
                {
                    Message = "User Is Inactive!",
                    IsAuthenticated = false
                };
            }

            Log.Debug("User authenticated successfully: {UserId}", user.UserId);
            var jwtSecurityToken = await _tokenService.CreateJwtToken(user);

            var refreshToken = await _authRepository.GetActiveRefreshTokenAsync(user.UserId);
            if (refreshToken != null)
            {
                Log.Debug("Using existing refresh token for user: {UserId}", user.UserId);
                return CreateSuccessfulTokenResponse(
                    jwtSecurityToken,
                    refreshToken.Token,
                    refreshToken.ExpiresOn,
                    user,
                    "Welcome");
            }

            Log.Debug("Generating new refresh token for user: {UserId}", user.UserId);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _authRepository.SaveRefreshTokenAsync(newRefreshToken, user.UserId);

            return CreateSuccessfulTokenResponse(
                jwtSecurityToken,
                newRefreshToken.Token,
                newRefreshToken.ExpiresOn,
                user,
                "Welcome");
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token

        
        /// </summary>
        /// <param name="token">Refresh token</param>
        /// <returns>New token response with refreshed tokens</returns>
        public async Task<TokenResponseDto> RefreshTokenAsync(string token)
        {
            Log.Information("Refresh token attempt for token: {Token}", token);

            var userId = await _userManagementRespository.GetUserIdByTokenAsync(token);
            if (userId == -1)
            {
                Log.Warning("Refresh token failed - User ID not found for token: {Token}", token);
                return new TokenResponseDto
                {
                    Message = "User ID not found",
                    IsAuthenticated = false
                };
            }

            var user = await _userManagementRespository.GetUserByIdAsync(userId);
            if (user == null)
            {
                Log.Warning("Refresh token failed - User not found for ID: {UserId}", userId);
                return new TokenResponseDto
                {
                    Message = $"User with ID {userId} not found",
                    IsAuthenticated = false
                };
            }

            if (!await _authRepository.RevokeTokenAsync(token))
            {
                Log.Warning("Refresh token failed - Invalid or inactive token: {Token}", token);
                return new TokenResponseDto
                {
                    Message = "Invalid token or Inactive token",
                    IsAuthenticated = false
                };
            }

            Log.Debug("Generating new tokens for user: {UserId}", userId);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _authRepository.SaveRefreshTokenAsync(newRefreshToken, userId);

            var jwtSecurityToken = await _tokenService.CreateJwtToken(user);

            return CreateSuccessfulTokenResponse(
                jwtSecurityToken,
                newRefreshToken.Token,
                newRefreshToken.ExpiresOn,
                user,
                "RefreshToken completed successfully");
        }

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        /// <param name="token">Token to revoke</param>
        /// <returns>True if revocation was successful</returns>
        public Task<bool> RevokeTokenAsync(string token)
        {
            Log.Information("Revoking token: {Token}", token);
            return _authRepository.RevokeTokenAsync(token);
        }

        /// <summary>
        /// Creates a successful token response DTO
        /// </summary>
        private TokenResponseDto CreateSuccessfulTokenResponse(
            JwtSecurityToken jwtToken,
            string refreshToken,
            DateTime refreshTokenExpiration,
            User user,
            string message)
        {
            return new TokenResponseDto
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Email = user.Email,
                Username = user.Username,
                ExpiresOn = jwtToken.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                Roles = jwtToken.Claims
                    .Where(c => c.Type == "role")
                    .Select(c => c.Value)
                    .ToList(),
                Message = message
            };
        }
    }
}