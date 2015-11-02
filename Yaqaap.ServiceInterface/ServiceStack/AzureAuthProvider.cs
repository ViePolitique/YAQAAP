using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Web;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.ServiceStack
{
    public class AzureAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(IServiceBase authService,
            string userName, string password)
        {
            //Add here your custom auth logic (database calls etc)
            //Return true if credentials are valid, otherwise false

            TableRepository tableRepository = new TableRepository();

            CloudTable userTable = tableRepository.GetTable(Tables.Users);

            TableQuery<UserEntry> userQuery = userTable.CreateQuery<UserEntry>();

            userName = TableEntityHelper.RemoveDiacritics(userName);
            userName = TableEntityHelper.ToAzureKeyString(userName);

            var result =    (from k in userQuery
                            where k.RowKey == userName
                            select k).ToArray();

            var user = result.FirstOrDefault();
            if (user == null)
                return false;

            return user.Password == password;
        }

        public override IHttpResult OnAuthenticated(IServiceBase authService,
            IAuthSession session, IAuthTokens tokens,
            Dictionary<string, string> authInfo)
        {
            //Fill IAuthSession with data you want to retrieve in the app eg:
            //session.FirstName = "some_firstname_from_db";
            //...

            //Call base method to Save Session and fire Auth/Session callbacks:
            return base.OnAuthenticated(authService, session, tokens, authInfo);

            //Alternatively avoid built-in behavior and explicitly save session with
            //authService.SaveSession(session, SessionExpiry);
            //return null;
        }
    }
}
