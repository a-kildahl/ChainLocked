using ChainLocked.Cryptography;
using ChainLocked.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked
{
    public class Block<T>
    {
        public T Data { get; }
        public long BlockNumber { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public string BlockHash { get; private set; }
        public string PreviousBlockHash { get; private set; }
        public DigitalSignature Signature { get; private set; }

        internal Block(T data, long blockNumber, string previousHash, string signerId, string signersPublicKey)
            : this(data, blockNumber, previousHash, signerId, signersPublicKey, null, DateTime.UtcNow, null)
        {
            this.BlockHash = CalculateBlockHash(this.PreviousBlockHash);
        }

        internal Block(T data, long blockNumber, string previouskHash, string signerId, string signersPublicKey, string signature, DateTime createdDate, string blockHash)
        {
            this.BlockNumber = blockNumber;
            this.Data = data;
            this.CreatedDate = createdDate;
            this.PreviousBlockHash = previouskHash;
            this.BlockHash = blockHash;
            this.Signature = new DigitalSignature
            {
                SignerId = signerId,
                PublicKey = signersPublicKey
            };
            ApplySignature(signature);
        }
        

        public string CalculateBlockHash(string previousBlockHash)
        {
            string data = JsonConvert.SerializeObject(this.Data);
            string blockHeader = this.BlockNumber + CreatedDate.ToBinary().ToString() + this.Signature.SignerId + this.Signature.PublicKey + previousBlockHash;
            return Hasher.ComputeHash(data + blockHeader);
        }

        public void ApplySignature(string signature)
        {
            this.Signature.Signature = signature;
        }
    }
}
