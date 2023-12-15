using Microsoft.AspNetCore.Identity;
using RestSharp;
using System.Net;
using System.Text;
using System.Text.Json;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class SendMailRepository : ISendMailRepository
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public SendMailRepository(IConfiguration configuration,
            UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<bool> SendResetPasswordEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var data = new Dictionary<string, object>
            {
                { "from",
                    new {
                        email = "reset.tanilink@bantuin.me",
                        name = "TaniLink"
                    }
                },
                { "to",
                    new [] {
                        new {
                        email = email,
                        },
                    }
                },
                { "template_uuid", "e335c690-89a2-472b-9c6a-17210a0194c4" },
                { "template_variables",
                    new {
                        user_email = user.Email,
                        pass_reset_link = "https://tanilink.bantuin.me/User/ResetPassword?token=" + token + "&id=" + user.Id
                    }
                },
            };

            RestClient client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri("https://send.api.mailtrap.io/api/send"),
            });


            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", "Bearer " + _configuration["MailTrap:Key"]);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonSerializer.Serialize(data), ParameterType.RequestBody);
            request.Method = Method.Post;

            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;

            return true;
        }

        public async Task<bool> SendVerificationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            if (token == null)
                return false;

            var data = new Dictionary<string, object>
            {
                { "from",
                    new {
                        email = "verify.tanilink@bantuin.me",
                        name = "TaniLink"
                    }
                },
                { "to",
                    new [] {
                        new {
                        email = email,
                        },
                    }
                },
                { "template_uuid", "57543064-f4f9-4a47-b897-cc36a8bef689" },
                { "template_variables",
                    new {
                        name = user.FullName,
                        verify_link = "https://tanilink.bantuin.me/User/Verify?token=" + token + "&id=" + user.Id
                    }
                },
            };

            RestClient client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri("https://send.api.mailtrap.io/api/send"),
            });


            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", "Bearer " + _configuration["MailTrap:Key"]);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonSerializer.Serialize(data), ParameterType.RequestBody);
            request.Method = Method.Post;

            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;

            return true;

        }
    }
}
