using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;
using static TaniLink_Backend.LoginRes.Types;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class AccountService : Accounts.AccountsBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAddressRepository _addressRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ISendMailRepository _sendMailRepository;

        public AccountService(IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IAreaRepository areaRepository,
            ITokenRepository tokenRepository,
            ISendMailRepository sendMailRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _addressRepository = addressRepository;
            _areaRepository = areaRepository;
            _tokenRepository = tokenRepository;
            _sendMailRepository = sendMailRepository;
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
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = DateOnly.Parse(request.DateOfBirth),
                    Gender = request.Gender,
                    Picture = "https://firebasestorage.googleapis.com/v0/b/topibatu-2a076.appspot.com/o/assets%2Fdefault_profile_picture.png?alt=media&token=0135a63e-af90-4c35-8109-d48a9efaf3be"
                };

                if (request.Password != request.ConfirmPassword)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Password and Confirm Password do not match."));

                var result = await _userManager.CreateAsync(userAdd, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userAdd, "User");
                    var sendMail = await _sendMailRepository.SendVerificationEmail(request.Email);
                    if (!sendMail)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to send verification email"));

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
                    Tokens = new Token
                    {
                        AccessToken = tokenString,
                        Expires = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:DurationInDay"])))
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllAreaDetails> GetAllArea(Empty request, ServerCallContext context)
        {
            try
            {
                var areas = await _areaRepository.GetAllAreas();
                var areaMap = _mapper.Map<IEnumerable<AreaDetail>>(areas);
                var allAreas = new AllAreaDetails();
                allAreas.Area.AddRange(areaMap);
                return allAreas;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AccountDetail> GetProfile(Empty request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                var accountDetail = _mapper.Map<AccountDetail>(user);
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    accountDetail.Role.Add(role);
                }

                return accountDetail;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AccountDetail> EditProfile(EditProfileReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                var accountDetail = _mapper.Map(request, user);
                if (request.DateOfBirth != null && !string.IsNullOrEmpty(request.DateOfBirth.ToString()))
                    accountDetail.DateOfBirth = DateOnly.Parse(request.DateOfBirth);

                var result = await _userManager.UpdateAsync(accountDetail);
                if (result.Succeeded)
                {
                    var account = _mapper.Map<AccountDetail>(accountDetail);
                    foreach (var role in await _userManager.GetRolesAsync(accountDetail))
                    {
                        account.Role.Add(role);
                    }
                    return account;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllAddressDetails> GetAddress(Empty request, ServerCallContext context)
        {
            try
            {
                var addresses = await _addressRepository.GetAllAddressesByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var addressMap = _mapper.Map<IEnumerable<AddressDetail>>(addresses);
                var addressDetails = new AllAddressDetails();
                addressDetails.Address.AddRange(addressMap);
                return addressDetails;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllAddressDetails> AddAddress(BatchAddAddressReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                foreach (var address in request.Address)
                {
                    var area = await _areaRepository.GetAreaById(address.AreaId);
                    if (area == null)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Area not found"));

                    var addressAdd = new Address
                    {
                        Detail = address.Detail,
                        Area = area,
                        User = user
                    };

                    var createAddress = await _addressRepository.CreateAddress(addressAdd);
                    if (createAddress == null)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to create address"));
                }

                return await GetAddress(new Empty(), context);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllAddressDetails> EditAddress(BatchEditAddressReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                foreach (var address in request.Address)
                {
                    var area = await _areaRepository.GetAreaById(address.AreaId);
                    if (area == null)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Area not found"));

                    var addressEdit = await _addressRepository.GetAddressById(address.Id);
                    if (addressEdit == null)
                    {
                        var addressAdd = new Address
                        {
                            Detail = address.Detail,
                            Area = area,
                            User = user
                        };

                        var createAddress = await _addressRepository.CreateAddress(addressAdd);
                        if (createAddress == null)
                            throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to create address"));
                    } 
                    else
                    {
                        addressEdit.Detail = address.Detail;
                        addressEdit.Area = area;

                        var updateAddress = await _addressRepository.UpdateAddress(addressEdit);
                        if (updateAddress == null)
                            throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to update address"));
                    }

                }

                return await GetAddress(new Empty(), context);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllAddressDetails> DeleteAddress(AddressIdReq request, ServerCallContext context)
        {
            try
            {
                var address = await _addressRepository.GetAddressById(request.AddressId);
                if (address == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Address not found"));

                var deleteAddress = await _addressRepository.DeleteAddress(request.AddressId);
                if (deleteAddress == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to delete address"));

                return await GetAddress(new Empty(), context);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<Empty> ChangeEmail(EmailReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                if (user.EmailConfirmed == false)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Please check your inbox or spam email to confirm your email."));

                var result = await _userManager.SetEmailAsync(user, request.Email);
                if (result.Succeeded)
                {
                    var changeUsername = await _userManager.SetUserNameAsync(user, request.Email);
                    if (!changeUsername.Succeeded)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, changeUsername.Errors.FirstOrDefault()?.Description.ToString()!));

                    var sendMail = await _sendMailRepository.SendVerificationEmail(request.Email);
                    if (!sendMail)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to send verification email"));

                    return new Empty { };
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

        public override async Task<Empty> IsEmailConfirmed(EmailReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                if (user.EmailConfirmed == false)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Please check your inbox or spam email to confirm your email."));

                return new Empty { };
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

        public override async Task<Empty> ResendVerificationMail(EmailReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                if(user.EmailConfirmed == true)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Email already confirmed"));

                var sendMail = await _sendMailRepository.SendVerificationEmail(request.Email);
                if (!sendMail)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to send verification email"));

                return new Empty { };
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

        public override async Task<Empty> ForgotPassword(EmailReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (token == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to generate reset password token"));

                token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var sendResetMail = await _sendMailRepository.SendResetPasswordEmail(request.Email, token);
                if (!sendResetMail)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to send reset password email"));

                return new Empty { };
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


        public override async Task<Empty> ResetPassword(ResetPasswordReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                if (request.NewPassword != request.ConfirmNewPassword)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Password and Confirm Password do not match."));

                var token = Encoding.UTF8.GetString(Convert.FromBase64String(request.Token));
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
                if (!result.Succeeded)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.FirstOrDefault()?.Description.ToString()!));

                return new Empty { };
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<Empty> ChangePassword(ChangePasswordReq request, ServerCallContext context)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not found"));

                if (request.NewPassword != request.ConfirmNewPassword)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Password and Confirm Password do not match."));

                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (!result.Succeeded)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.FirstOrDefault()?.Description.ToString()!));

                return new Empty { };
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
    }
}
