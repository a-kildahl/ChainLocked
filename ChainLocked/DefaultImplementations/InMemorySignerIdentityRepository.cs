using ChainLocked.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChainLocked.DefaultImplementations
{
    public class InMemorySignerIdentityRepository : ISignerIdentityRepository
    {
        private readonly IDictionary<string, string> _dict = new Dictionary<string, string>();

        public Task<string> GetPublicKey(string signerId)
        {
            return Task.FromResult(_dict[signerId]);
        }

        public Task Save(string signerId, string publicKey)
        {
            _dict.Add(signerId, publicKey);
            return Task.CompletedTask;
        }
    }
}
