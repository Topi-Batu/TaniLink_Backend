using Grpc.Core;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class AccountService : Accounts.AccountsBase
    {
        public override async Task<RegisterRes> Register(RegisterReq request, ServerCallContext context)
        {
            return new RegisterRes
            {
                Name = "hokas",
                Status = "sukses"
            };
        }

    }
}
