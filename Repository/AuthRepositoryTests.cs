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
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthorizationServiceTests.Repository
{
    public class Tests
    {
        public string token;
        public string token_null;
        Userdetails user;
        IDictionary<string, string> config;
        List<Userdetails> userdetails = new List<Userdetails>();
        Mock<DbSet<Userdetails>> mockSet;
        Mock<AuditManagementSystemContext> auditManagementSystemContext;
        IQueryable<Userdetails> userdetailsData;

        [SetUp]
        public void Setup()
        {
            //setting up tokens
            token = "xhagssbmfbdmsdjfbkalalasknasncjafh";
            token_null = null;
            //setting up user
            user = new Userdetails { Email = "aaa", Password = "111" };
            //setting up IConfiguration
            config = new Dictionary<string, string>{
                 {"Jwt:Key", "This is my Secret Key"},
                 {"Jwt:Issuer", "Test.com"}
            };
            //setting up AuditManagementSystemContext
            userdetails = new List<Userdetails>()
           {
               new Userdetails{Email="aaa",Password="111"}
            };
            userdetailsData = userdetails.AsQueryable();
            mockSet = new Mock<DbSet<Userdetails>>();

            mockSet.As<IQueryable<Userdetails>>().Setup(m => m.Provider).Returns(userdetailsData.Provider);
            mockSet.As<IQueryable<Userdetails>>().Setup(m => m.Expression).Returns(userdetailsData.Expression);
            mockSet.As<IQueryable<Userdetails>>().Setup(m => m.ElementType).Returns(userdetailsData.ElementType);
            mockSet.As<IQueryable<Userdetails>>().Setup(m => m.GetEnumerator()).Returns(userdetailsData.GetEnumerator());

            var p = new DbContextOptions<AuditManagementSystemContext>();
            auditManagementSystemContext = new Mock<AuditManagementSystemContext>(p);
            auditManagementSystemContext.Setup(x => x.Userdetails).Returns(mockSet.Object);
        }

        [Test]
        public void GenerateJWT_correctInput_tokenNotnull()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var res = new AuthRepository(token, auditManagementSystemContext.Object);
            var data = res.GenerateAccessToken(userdetails[0],configuration);
            Assert.IsNotNull(data);
        }

        [Test]
        public void generateJWT_invalidInput_tokenNull()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var res = new AuthRepository(token_null, auditManagementSystemContext.Object);
            var data = res.GenerateAccessToken(null, configuration);
            Assert.IsNull(data);
        }
    }
}