using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ForkEat.Web.Tests.Integration
{
    public abstract class AuthenticatedTests : IntegrationTest
    {
        protected AuthenticatedTests(WebApplicationFactory<Startup> factory, string[] tableToClear) : base(factory, tableToClear.Append("Users").ToList())
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var token = await RegisterAndLogin();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}