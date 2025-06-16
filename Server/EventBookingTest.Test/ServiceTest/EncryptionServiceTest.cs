using System;
using System.Threading.Tasks;
using EventBookingApi.Model;
using EventBookingApi.Service;
using NUnit.Framework;



namespace EventBookingApi.Tests.Service
{
    [TestFixture]
    public class EncryptionServiceTests
    {
        private EncryptionService _encryptionService;

        [SetUp]
        public void SetUp()
        {
            _encryptionService = new EncryptionService();
        }

        [Test]
        public async Task EncryptData_ValidInput_ReturnsHashedData()
        {
            // Arrange
            var model = new EncryptModel
            {
                Data = "TestPassword123"
            };

            // Act
            var result = await _encryptionService.EncryptData(model);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
