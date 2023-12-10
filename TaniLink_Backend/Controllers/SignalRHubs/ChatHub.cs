using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers.SignalRHubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;

        public ChatHub(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.FindByEmailAsync(Context.User.FindFirst(c => c.Type == ClaimTypes.Email)!.Value);
            user.ConnectionId = Context.ConnectionId;

            await _userManager.UpdateAsync(user);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.Users.Where(u => u.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();
            user.ConnectionId = null;

            await _userManager.UpdateAsync(user);

            await base.OnDisconnectedAsync(exception);
        }
        /*public async Task SendMessage(string conversationId, string message)
        {

            try
            {
                // if konversation
                var SendClient = Clients.Client("").SendAsync("ReceiveMessage", user, message);

                await SendClient;

                if (SendClient.IsCompletedSuccessfully)
                {
                    Console.WriteLine("Send Message Success");
                }
                else
                {
                    Console.WriteLine("Send Message Failed");
                }
            }
            catch (Exception ex)
            {

            }

        }*/
    }
}
