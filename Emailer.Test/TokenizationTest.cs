using Emailer.Helpers;
using Emailer.Models;
using Emailer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Emailer.Test
{
    [TestClass]
    public class TokenizationTest
    {
        [TestMethod]
        public async Task WillReplaceTokens()
        {
            var tokenizationService = new TokenizationService();
            var result = await tokenizationService.ReplaceTokens(Constants.CONTACT_US_HTML, new ContactUsRequest { 
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Email = "test@test.com",
                Message = "This is a test message"
            });

            Assert.IsFalse(result.Contains("{{FirstName}}"));
            Assert.IsFalse(result.Contains("{{LastName}}"));
            Assert.IsFalse(result.Contains("{{Email}}"));
            Assert.IsFalse(result.Contains("{{Message}}"));

            Assert.IsTrue(result.Contains("TestFirstName"));
            Assert.IsTrue(result.Contains("TestLastName"));
            Assert.IsTrue(result.Contains("test@test.com"));
            Assert.IsTrue(result.Contains("This is a test message"));
        }
    }
}
