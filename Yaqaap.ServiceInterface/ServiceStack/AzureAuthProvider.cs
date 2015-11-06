using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using ServiceStack.Auth;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.ServiceStack
{
    public class AzureAuthProvider : IUserAuthRepository
    {
        public int? MaxLoginAttempts { get; set; }

        readonly TableRepository _tableRepository = new TableRepository();

        private void ValidateNewUser(IUserAuth newUser)
        {
            if (newUser.UserName.IsNullOrEmpty() && newUser.Email.IsNullOrEmpty())
                throw new ArgumentNullException(ErrorMessages.UsernameOrEmailRequired);
            if (!newUser.UserName.IsNullOrEmpty() && !HostContext.GetPlugin<AuthFeature>().IsValidUsername(newUser.UserName))
                throw new ArgumentException(ErrorMessages.IllegalUsername, "UserName");
        }

        private void ValidateNewUser(IUserAuth newUser, string password)
        {
            newUser.ThrowIfNull("newUser");
            password.ThrowIfNullOrEmpty("password");

            ValidateNewUser(newUser);
        }

        private void AssertNoExistingUser(IUserAuth newUser, IUserAuth exceptForExistingUser = null)
        {
            if (newUser.UserName != null)
            {
                var userAuthByUserName = GetUserAuthByUserName(newUser.UserName);
                if (userAuthByUserName != null && (exceptForExistingUser == null || userAuthByUserName.Id != exceptForExistingUser.Id))
                    throw new ArgumentException("User {0} already exists".Fmt((object)newUser.UserName));
            }
            if (newUser.Email == null)
                return;
            var userAuthByUserName1 = GetUserAuthByUserName(newUser.Email);
            if (userAuthByUserName1 != null && (exceptForExistingUser == null || userAuthByUserName1.Id != exceptForExistingUser.Id))
                throw new ArgumentException("Email {0} already exists".Fmt((object)newUser.Email));
        }

        protected virtual void RecordInvalidLoginAttempt(IUserAuth userAuth)
        {
            if (!MaxLoginAttempts.HasValue)
                return;
            ++userAuth.InvalidLoginAttempts;
            userAuth.LastLoginAttempt = userAuth.ModifiedDate = DateTime.UtcNow;
            if (userAuth.InvalidLoginAttempts >= MaxLoginAttempts.Value)
                userAuth.LockedDate = userAuth.LastLoginAttempt;
            SaveUserAuth(userAuth);
        }

        protected virtual void RecordSuccessfulLogin(IUserAuth userAuth)
        {
            if (!MaxLoginAttempts.HasValue)
                return;
            userAuth.InvalidLoginAttempts = 0;
            userAuth.LastLoginAttempt = userAuth.ModifiedDate = DateTime.UtcNow;
            SaveUserAuth(userAuth);
        }

        public void LoadUserAuth(IAuthSession session, IAuthTokens tokens)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAuth(IAuthSession authSession)
        {
            throw new NotImplementedException();
        }

        public List<IUserAuthDetails> GetUserAuthDetails(string userAuthId)
        {
            return new List<IUserAuthDetails>();
        }

        public IUserAuthDetails CreateOrMergeAuthSession(IAuthSession authSession, IAuthTokens tokens)
        {
            throw new NotImplementedException();
        }

        public IUserAuth GetUserAuth(IAuthSession authSession, IAuthTokens tokens)
        {
            //if (!authSession.UserAuthId.IsNullOrEmpty())
            //{
            //    var userAuth = GetUserAuth(authSession.UserAuthId);
            //    if (userAuth != null)
            //    {
            //        return userAuth;
            //    }
            //}
            if (!authSession.UserAuthName.IsNullOrEmpty())
            {
                var userAuth = GetUserAuthByUserName(authSession.UserAuthName);
                if (userAuth != null)
                {
                    return userAuth;
                }
            }

            if (tokens == null || tokens.Provider.IsNullOrEmpty() || tokens.UserId.IsNullOrEmpty())
            {
                return null;
            }

            return null;
        }

        public IUserAuth GetUserAuthByUserName(string userNameOrEmail)
        {
            CloudTable userTable = _tableRepository.GetTable(Tables.Users);

            TableQuery<UserEntry> userQuery = userTable.CreateQuery<UserEntry>();

            var username = userNameOrEmail;
            var email = userNameOrEmail;
            username = TableEntityHelper.RemoveDiacritics(username);
            username = TableEntityHelper.ToAzureKeyString(username);

            var result = (from k in userQuery
                          where k.RowKey == username || k.Email == email
                          select k).ToArray();

            return result.FirstOrDefault();
        }

        public void SaveUserAuth(IUserAuth userAuth)
        {
            _tableRepository.InsertOrReplace(userAuth as UserEntry, Tables.Users);
        }

        public bool TryAuthenticate(string userName, string password, out IUserAuth userAuth)
        {
            userAuth = GetUserAuthByUserName(userName);
            if (userAuth == null)
                return false;
            if (HostContext.Resolve<IHashProvider>().VerifyHashString(password, userAuth.PasswordHash, userAuth.Salt))
            {
                RecordSuccessfulLogin(userAuth);
                return true;
            }
            RecordInvalidLoginAttempt(userAuth);
            userAuth = null;
            return false;
        }

        public bool TryAuthenticate(Dictionary<string, string> digestHeaders, string privateKey, int nonceTimeOut, string sequence, out IUserAuth userAuth)
        {
            throw new NotImplementedException();
        }

        public IUserAuth CreateUserAuth(IUserAuth newUser, string password)
        {
            UserEntry user = newUser as UserEntry;

            ValidateNewUser(user, password);
            AssertNoExistingUser(user);

            var saltedHash = HostContext.Resolve<IHashProvider>();
            string salt;
            string hash;
            saltedHash.GetHashAndSaltString(password, out hash, out salt);

            user.PartitionKey = Guid.NewGuid().ToString();
            user.RowKey = newUser.UserName;
            user.RowKey = TableEntityHelper.RemoveDiacritics(user.RowKey);
            user.RowKey = TableEntityHelper.ToAzureKeyString(user.RowKey);

            //user.Id = 0;
            user.PasswordHash = hash;
            user.Salt = salt;
            var digestHelper = new DigestAuthFunctions();
            user.DigestHa1Hash = digestHelper.CreateHa1(user.UserName, DigestAuthProvider.Realm, password);
            user.CreatedDate = DateTime.UtcNow;
            user.ModifiedDate = user.CreatedDate;

            //var userId = user.Id.ToString(CultureInfo.InvariantCulture);
            //if (!newUser.UserName.IsNullOrEmpty())
            //{
            //    redis.SetEntryInHash(IndexUserNameToUserId, newUser.UserName, userId);
            //}
            //if (!newUser.Email.IsNullOrEmpty())
            //{
            //    redis.SetEntryInHash(IndexEmailToUserId, newUser.Email, userId);
            //}
            SaveUserAuth(user);

            return user;
        }

        public IUserAuth UpdateUserAuth(IUserAuth existingUser, IUserAuth newUser, string password)
        {
            throw new NotImplementedException();
        }

        public IUserAuth GetUserAuth(string userAuthId)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserAuth(string userAuthId)
        {
            throw new NotImplementedException();
        }
    }
}
