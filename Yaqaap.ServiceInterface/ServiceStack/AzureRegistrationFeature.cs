using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.FluentValidation;

namespace Yaqaap.ServiceInterface.ServiceStack
{
    /// <summary>
    /// Enable the Registration feature and configure the RegistrationService.
    /// </summary>
    public class AzureRegistrationFeature : IPlugin
    {
        public string AtRestPath { get; set; }

        public AzureRegistrationFeature()
        {
            this.AtRestPath = "/api/register";
        }

        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<AzureRegisterService>(AtRestPath);
            appHost.RegisterAs<RegistrationValidator, IValidator<Register>>();
        }
    }
}