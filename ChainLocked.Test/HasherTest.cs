using ChainLocked.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ChainLocked.Test
{
    public class HasherTest
    {
        [Fact]
        public void TestHasher()
        {
            string expected = "F6B51A03DEBF680BDCC215F429EED499EF5933E759614DA25ED44513E918561D";
            string actual = Hasher.ComputeHash("THIS IS A TEST");
            Assert.Equal(expected, actual);
        }
    }
}
