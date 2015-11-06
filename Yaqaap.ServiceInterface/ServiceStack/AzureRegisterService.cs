using ServiceStack;
using ServiceStack.Auth;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.ServiceStack
{
    [DefaultRequest(typeof(Register))]
    public class AzureRegisterService : RegisterService<UserEntry>
    {
    }
}