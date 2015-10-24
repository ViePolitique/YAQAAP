using System;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using Yaqaap.ServiceModel;
using Yaqaap.ServiceInterface;

namespace Yaqaap.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost _appHost;

        public UnitTests()
        {
            _appHost = new BasicAppHost(typeof(MyServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _appHost.Dispose();
        }

        [Test]
        public void TestMethod1()
        {
            var service = _appHost.Container.Resolve<MyServices>();

            //var response = (HelloResponse)service.Any(new Hello { Name = "World" });

            //Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }
    }
}
