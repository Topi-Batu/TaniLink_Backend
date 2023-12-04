using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class AccountService : Accounts.AccountsBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;

        public AccountService(IMapper mapper, 
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ITokenRepository tokenRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenRepository = tokenRepository;
        }
        public override async Task<RegisterRes> Register(RegisterReq request, ServerCallContext context)
        {
            try
            {
                var userAdd = new User
                {
                    FullName = request.FullName,
                    UserName = request.Email,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync(userAdd, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userAdd, "User");/*
                    var sendMail = await _sendMailRepository.SendVerificationEmail(request.Email);
                    if (!sendMail)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to send verification email"));*/
                    var accountDetail = _mapper.Map<AccountDetail>(userAdd);
                    foreach (var role in await _userManager.GetRolesAsync(userAdd))
                    {
                        accountDetail.Role.Add(role);
                    }
                    return new RegisterRes
                    {
                        Account = accountDetail
                    };
                }

                throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.FirstOrDefault()?.Description.ToString()!));
            }
            catch (RpcException ex)
            {
                throw new RpcException(ex.Status);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<LoginRes> Login(LoginReq request, ServerCallContext context)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(request.Email!);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                var result = await _userManager.CheckPasswordAsync(user, request.Password!);
                if (!result)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Password invalid"));

                if (user.EmailConfirmed == false)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Please check your inbox or spam email to confirm your email."));

                var tokenString = await _tokenRepository.CreateAccessToken(user);

                var accountDetail = _mapper.Map<AccountDetail>(user);
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    accountDetail.Role.Add(role);
                }

                return new LoginRes
                {
                    Tokens = new LoginRes.Types.Token
                    {
                        AccessToken = tokenString,
                        Expires = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + Convert.ToInt32(_configuration["Jwt:DurationInDay"])
                    },
                    Account = accountDetail
                };
            }
            catch (RpcException ex)
            {
                throw new RpcException(ex.Status);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        [Authorize]
        public override async Task<Test> Testing(Empty request, ServerCallContext context)
        {
            return new Test
            {
                Test_ = "Hello World"
            };
        }



    }
}
