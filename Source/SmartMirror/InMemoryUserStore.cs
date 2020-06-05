using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror
{
    public class InMemoryUserStore : IUserStore<IdentityUser>
    {
        private static Dictionary<string, IdentityUser> Store = new Dictionary<string, IdentityUser>();

        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (!Store.ContainsKey(user.Id))
            {
                Store.Add(user.Id, user);
            }

            return Task.FromResult(new IdentityResult());
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (Store.ContainsKey(user.Id))
            {
                Store.Remove(user.Id);
            }

            return Task.FromResult(new IdentityResult());
        }

        public void Dispose()
        {
        }

        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (Store.ContainsKey(userId))
            {
                return Task.FromResult(Store[userId]);
            }

            return null;
        }

        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToUpper());
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (Store.ContainsKey(user.Id))
            {
                Store[user.Id] = user;
            }

            return Task.FromResult(new IdentityResult());
        }
    }
}