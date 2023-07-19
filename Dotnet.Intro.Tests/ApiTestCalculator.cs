using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotnet.Intro.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Intro.Tests
{
    public class ApiTestCalculator : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient client;

        public ApiTestCalculator(WebApplicationFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task MiddlewareTest()
        {
            HttpResponseMessage response = await client.GetAsync("middleware/hello-world");

            response.EnsureSuccessStatusCode();
            
            string result =  await response.Content.ReadAsStringAsync();

            Assert.Equal("hello world!", result);
        }

        [Fact]
        public async Task DefaultMiddlewareTest()
        {
            HttpResponseMessage response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello World!", result);
        }

        [Theory]
        [InlineData("add", 2, 3, 5)]
        [InlineData("sub", 8, 3, 5)]
        [InlineData("mul", 16, 16, 256)]
        [InlineData("div", 23 ,23, 1)]
        [InlineData("div", 5, 0, -1)]
        public async Task CalculatorWebTest(string operation, float x, float y, float result)
        {
            HttpResponseMessage response = await client.GetAsync("calculator/" + operation
                + "?x=" + x.ToString() + "&y=" 
                + y.ToString());

            response.EnsureSuccessStatusCode();

            string resultActual = await response.Content.ReadAsStringAsync();

            Assert.Equal(result.ToString(), resultActual);
        }
    }
}
