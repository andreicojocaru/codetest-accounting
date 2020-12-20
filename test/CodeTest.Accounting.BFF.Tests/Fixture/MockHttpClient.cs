using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CodeTest.Accounting.BFF.Tests.Fixture
{
    public class MockHttpClient : HttpClient
    {
        public MockHttpClient() : base(new MockHttpClientHandler())
        {

        }
    }

    public class MockHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted));
        }
    }
}
