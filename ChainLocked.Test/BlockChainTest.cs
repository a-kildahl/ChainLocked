
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ChainLocked.Test.TestModels;
using Moq;
using ChainLocked.Interfaces;
using ChainLocked.Exceptions;
using ChainLocked.Cryptography;

namespace ChainLocked.Test
{
    public class BlockChainTest
    {

        private readonly Mock<IBlockRepository<Transaction>> _blockRepositoryMock;
        private readonly Mock<ISignerIdentityRepository> _signerIdentityRepositoryMock;

        private readonly BlockChain<Transaction> _sut;
        private readonly KeyPair _keys;
        private readonly string _signerId;

        public BlockChainTest()
        {
            _blockRepositoryMock = new Mock<IBlockRepository<Transaction>>();
            _signerIdentityRepositoryMock = new Mock<ISignerIdentityRepository>();

            _sut = new BlockChain<Transaction>(_blockRepositoryMock.Object, _signerIdentityRepositoryMock.Object);
            _keys = Signer.GenerateNewKeyPair();
            _signerId = "XUNIT";

            _signerIdentityRepositoryMock.Setup(o => o.GetPublicKey(_signerId)).ReturnsAsync(_keys.PublicKey);
        }

        [Fact]
        public async Task VerifyChain_IsValid_IfNoTampering()
        {
            // Arrange
            List<Block<Transaction>> blocks = new List<Block<Transaction>>();
            SetupMock(blocks);

            foreach (int i in Enumerable.Range(0, 10))
            {
                Transaction transaction = new Transaction
                {
                    ID = i,
                    TransactionTotal = 5023.2f,
                    Comment = "This is a comment"
                };
                Block<Transaction> block = await _sut.ConstructBlock(transaction, _signerId, _keys.PublicKey);
                string signature = Signer.Sign(_keys.PrivateKey, block.BlockHash);

                block.ApplySignature(signature);
                await _sut.AcceptNewBlock(block);
            }

            // Act
            await _sut.VerifyChain();

            // Assert - It passes when the action did not produce an exception
            _blockRepositoryMock.Verify(o => o.GetBlockByNumber(It.IsAny<long>()), Times.AtLeast(2));
        }

        [Fact]
        public async Task VerifyChain_IsInvalid_IfTampering()
        {
            // Arrange
            List<Block<Transaction>> blocks = new List<Block<Transaction>>();
            SetupMock(blocks);

            foreach (int i in Enumerable.Range(0, 10))
            {
                Transaction transaction = new Transaction
                {
                    ID = i,
                    TransactionTotal = 5023.2f,
                    Comment = "This is a comment"
                };
                Block<Transaction> block = await _sut.ConstructBlock(transaction, _signerId, _keys.PublicKey);
                string signature = Signer.Sign(_keys.PrivateKey, block.BlockHash);

                block.ApplySignature(signature);
                await _sut.AcceptNewBlock(block);
            }

            // Act
            Transaction tamperedTransaction = (Transaction)blocks[blocks.Count / 2].Data;
            tamperedTransaction.Comment = "This is the tampered comment.";
            Func<Task> action = () => _sut.VerifyChain();

            // Assert
            await Assert.ThrowsAsync<BlockFailedVerificationException<Transaction>>(action);
            _blockRepositoryMock.Verify(o => o.GetBlockByNumber(It.IsAny<long>()), Times.AtLeast(2));
        }

        private void SetupMock(List<Block<Transaction>> backingList)
        {
            _blockRepositoryMock.Setup(o => o.SaveBlock(It.IsAny<Block<Transaction>>())).Callback<Block<Transaction>>(o => backingList.Add(o)).Returns(Task.CompletedTask);
            _blockRepositoryMock.Setup(o => o.GetLatestBlock()).ReturnsAsync(() => backingList.LastOrDefault());
            _blockRepositoryMock.Setup(o => o.GetBlockByNumber(It.IsAny<long>())).ReturnsAsync((long no) => backingList.FirstOrDefault(o => o.BlockNumber == no));
        }
    }
}
