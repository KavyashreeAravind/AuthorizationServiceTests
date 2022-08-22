using AuthorizationService.Controllers;
using AuthorizationService.DataAccess;
using AuthorizationService.Model;
using AuthorizationService.Providers;
using AuthorizationService.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AuthorizationServiceTests.Providers
{
    public class Tests
    {
        public string token;
        public string token_null;
        Userdetails user;
        IDictionary<string, string> config;

        [SetUp]
        public void Setup()
        {
            //setting up tokens
            token = "xhagssbmfbdmsdjfbkalalasknasncjafh";
            token_null = null;
            //setting up users
            user = new Userdetails { Email = "aaa", Password = "111" };
            //setting up IConfiguration
            config = new Dictionary<string, string>{
                 {"Jwt:Key", "This is my Secret Key"},
                 {"Jwt:Issuer", "Test.com"}
            };
        }

        [Test]
        public void provider_generateToken_correctInput_tokenNotnull()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var mock = new Mock<IAuthRepository>();
            mock.Setup(p => p.GenerateAccessToken(user,configuration)).Returns(token);
            var res = new AuthProvider(mock.Object);
            var data = res.GenerateAccessToken(user,configuration);
            Assert.AreEqual(token, data);
        }
        [Test]
        public void provider_generateJWT_correctInput_tokenNull()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var mock = new Mock<IAuthRepository>();
            mock.Setup(p => p.GenerateAccessToken(user, configuration)).Returns(token_null);
            var res = new AuthProvider(mock.Object);
            var data = res.GenerateAccessToken(user, configuration);
            Assert.AreEqual(null, data);
        }
    }
}