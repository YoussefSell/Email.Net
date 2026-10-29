namespace Email.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// a fake <see cref="HttpMessageHandler"/> that records the sent requests and returns a predefined response,
    /// so the channels can be tested without sending any real emails.
    /// </summary>
    internal sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string content = "")
            : this(_ => new HttpResponseMessage(statusCode) { Content = new StringContent(content) }) { }

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public List<RecordedRequest> Requests { get; } = new List<RecordedRequest>();

        public RecordedRequest LastRequest => Requests[^1];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // read the body now, the content may be disposed by the caller once the request is sent
            var body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            Requests.Add(new RecordedRequest(request, body));

            return _responseFactory(request);
        }

        public HttpClient CreateClient() => new HttpClient(this);
    }

    internal sealed class RecordedRequest
    {
        public RecordedRequest(HttpRequestMessage request, string body)
        {
            Request = request;
            Body = body;
        }

        public HttpRequestMessage Request { get; }

        public HttpMethod Method => Request.Method;

        public Uri? Uri => Request.RequestUri;

        public string Body { get; }
    }
}
