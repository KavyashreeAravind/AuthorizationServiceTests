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

namespace AuthorizationServiceTests.Controllers
{
    public class Tests
    {
        public string token;
        public string token_null;
        List<Userdetails> userdetails = new List<Userdetails>();
        Mock<DbSet<Userdetails>> mockSet;
        Mock<AuditManagementSystemContext> auditManagementSystemContext;
        IQueryable<Userdetails> userdetailsData;
        IDictionary<string, string> config;

        [SetUp]
        public void Setup()
        {
        //setting up IConfiguration
            config = new Dictionary<string, string>{
                 {"Jwt:Key", "This is my Secret Key"},
                 {"Jwt:Issuer", "Test.com"}
            };
        //setting up tokens
            token = "xhagssbmfbdmsdjfbkalalasknasncjafh";
            token_null = null;
        //setting up AuditManagementSystemContext
            userdetails = new List<Userdetails>()
           {
               new Userdetails{Email="kavya@gmail.com",Password="abc@123"}
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
        public void TokenSuccess()
        {
           
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var rmock = new Mock<IAuthRepository>();
            rmock.Setup(p => p.GenerateAccessToken(userdetails[0], configuration)).Returns(token);
            rmock.Setup(p => p.Authenticate(userdetails[0].Email, userdetails[0].Password)).Returns(true);
            var pmock = new Mock<IAuthProvider>();
            pmock.Setup(p => p.GenerateAccessToken(userdetails[0], configuration)).Returns(token);
            pmock.Setup(p => p.Authenticate(userdetails[0].Email, userdetails[0].Password)).Returns(true);
            var login = new LoginController(pmock.Object,configuration);
            var data = login.AuthenticateUser(userdetails[0]) as OkObjectResult;
            Assert.AreEqual(200, data.StatusCode); 
        }

        [Test]
        public void TokenFail()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var rmock = new Mock<IAuthRepository>();
            rmock.Setup(p => p.GenerateAccessToken(userdetails[0], configuration)).Returns(token_null);
            rmock.Setup(p => p.Authenticate(userdetails[0].Email, userdetails[0].Password)).Returns(true);
            var pmock = new Mock<IAuthProvider>();
            pmock.Setup(p => p.GenerateAccessToken(userdetails[0], configuration)).Returns(token_null);
            pmock.Setup(p => p.Authenticate(userdetails[0].Email, userdetails[0].Password)).Returns(true);
            var login = new LoginController(pmock.Object, configuration);
            var data = login.AuthenticateUser(userdetails[0]) as BadRequestObjectResult;
            Assert.AreEqual(400, data.StatusCode);
        }
    }


}