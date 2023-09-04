using Castle.Core.Configuration;
using Moq;
using RealStateAPI.Utils;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RealStateAPI.UnitTests.RealStateAPI.Utils
{
    public class SecurityHelperTests
    {
        private SecurityHelper _securityHelper;
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;
        private string fakeUserPassword;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            _mockConfiguration.Setup(c => c["Security:SecretKey"]).Returns("5v2Zu1JL-b[T.UP[");
            _mockConfiguration.Setup(c => c["Security:IV"]).Returns(".[+H1oq.(+;13d>T");

            _securityHelper = new SecurityHelper(_mockConfiguration.Object);

            fakeUserPassword = "123456";
        }

        [Test]
        public void EncryptPassword_ProvidingValidPassword_ReturnString()
        {
            // Arrange

            // Act 
            var encryptedPassword = _securityHelper.EncryptPassword(fakeUserPassword);

            // Assert
            Assert.That(encryptedPassword, Is.Not.Null);
            Assert.That(encryptedPassword, Is.Not.Empty);
            Assert.That(IsBase64String(encryptedPassword), Is.EqualTo(true));
        }

        [TestCase("")]
        [TestCase(null)]
        public void EncryptPassword_ProvidingEmptyOrNullPassword_ThrowsArgumentNullException(string nullOrEmptyPassword)
        {
            Assert.That(() => _securityHelper.EncryptPassword(nullOrEmptyPassword), Throws.ArgumentNullException);
        }

        [Test]
        public void DecryptPassword_WithValidCipherText_ReturnsString()
        {
            // Arrange 
            var cipherText = _securityHelper.EncryptPassword(fakeUserPassword); 

            // Act
            var storedPassword = _securityHelper.DecryptPassword(cipherText);

            // Assert
            Assert.That(storedPassword, Is.Not.Null);
            Assert.That(storedPassword, Is.Not.Empty);
            Assert.That(storedPassword, Is.EqualTo(fakeUserPassword));

        }


        [Test]
        public void DecryptPassword_WithEmptyCipherText_ReturnsEmptyString()
        {
            var somePassword = _securityHelper.DecryptPassword(string.Empty);
            Assert.That(somePassword, Is.Empty);
        }

        [Test]
        public void DecryptPassword_WithNullForCipherText_ThrowsArgumentNullException()
        {
            Assert.That(() => _securityHelper.DecryptPassword(null), Throws.ArgumentNullException);
        }

        [TestCase("123456")]
        [TestCase("")]
        [TestCase("ASDOFKJNSIHBFHBDI98372Y4329Y4H327")]
        [TestCase("JFODNSA@&*#(¨#98hn jfo@#&(*")]
        public void CreateHashPassword_WithValidInputPassword_FillUpArrayOfBytesForHashAndSalt(string validInputPassword)
        {
            byte[] hash = null; 
            byte[] salt = null; 

            _securityHelper.CreateHashPassword(validInputPassword, out hash, out salt);

            string hashedPassword = Convert.ToBase64String(hash);
            string saltPassword = Convert.ToBase64String(salt);

            Assert.That(hash, Is.Not.Null);
            Assert.That(salt, Is.Not.Null);
            Assert.That(hashedPassword, Is.Not.Null);
            Assert.That(hashedPassword, Is.Not.Empty);
            Assert.That(saltPassword, Is.Not.Null);
            Assert.That(saltPassword, Is.Not.Empty);
        }

        [Test]
        public void CreateHashPassword_WithNullValueForPassword_ThrowArgumentNullException()
        {
            byte[] hash = null; 
            byte[] salt = null; 

            Assert.That(() => _securityHelper.CreateHashPassword(null, out hash, out salt),
                        Throws.ArgumentNullException);

        }

        [TestCase("123456")]
        [TestCase("")]
        [TestCase("ASDOFKJNSIHBFHBDI98372Y4329Y4H327")]
        [TestCase("JFODNSA@&*#(¨#98hn jfo@#&(*")]
        public void VerifyPassword_WithValidInputPassword_ReturnsTrue(string validInputPassword)
        {
            byte[] hash = null;
            byte[] salt = null;

            _securityHelper.CreateHashPassword(validInputPassword, out hash, out salt);

            string hashedPassword = Convert.ToBase64String(hash);
            string saltPassword = Convert.ToBase64String(salt);

            var result = _securityHelper.VerifyPassword(validInputPassword, hash, salt);

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void VerifyPassword_WithNullValueForPassword_ThrowArgumentNullException()
        {
            byte[] hash = null;
            byte[] salt = null;

            _securityHelper.CreateHashPassword(fakeUserPassword, out hash, out salt);

            string hashedPassword = Convert.ToBase64String(hash);
            string saltPassword = Convert.ToBase64String(salt);

            Assert.That(() => _securityHelper.VerifyPassword(null, hash, salt), Throws.ArgumentNullException);
        }

        [TestCase("123")]
        [TestCase("")]
        [TestCase("#*&(@#HUJSDhsdjhi")]
        [TestCase("#$)(&*#$(*&@89749823")]
        public void VerifyPassword_WithNonMatchingPassword_ReturnsFalse(string nonMatchingPassword)
        {
            byte[] hash = null;
            byte[] salt = null;

            _securityHelper.CreateHashPassword(fakeUserPassword, out hash, out salt);

            string hashedPassword = Convert.ToBase64String(hash);
            string saltPassword = Convert.ToBase64String(salt);

            var result = _securityHelper.VerifyPassword(nonMatchingPassword, hash, salt);

            Assert.That(result, Is.EqualTo(false));
        }

        #region Private Methods

        private bool IsBase64String(string input)
        {
            string base64Pattern = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";

            return Regex.IsMatch(input, base64Pattern);
        }

        #endregion
    }
}
