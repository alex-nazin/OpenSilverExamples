using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.HttpClientUsage
{
    public partial class HttpClientUsage : Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private const string uri = "https://doc.opensilver.net/documentation/general/overview.html";

        private class WebRequestState
        {
            public ManualResetEvent Event { get; set; }
            public WebRequest Request { get; set; }
            public byte[] Content { get; set; }
        }

        public HttpClientUsage()
        {
            this.InitializeComponent();
        }

        private async void TestHttpClient_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsByteArrayAsync();
            stopwatch.Stop();
            HttpClientResult.Text = $"The content was downloaded in {stopwatch.ElapsedMilliseconds} ms, length is {content.Length} bytes";
        }

        private void TestWebRequest_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((_) =>
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    WebRequest request = HttpWebRequest.Create(uri);
                    request.Method = "GET";

                    var evt = new ManualResetEvent(false);

                    var state = new WebRequestState
                    {
                        Event = evt,
                        Request = request,
                    };

                    request.BeginGetResponse(GetResponseCallback, state);

                    evt.WaitOne();

                    stopwatch.Stop();
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        WebRequestResult.Text = $"The content was downloaded in {stopwatch.ElapsedMilliseconds} ms, length is {state.Content.Length} bytes";
                    });
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        WebRequestResult.Text = $"Error during usage of HttpWebRequest: {ex.GetType()}";
                    });
                }
            });
        }

        private static void GetResponseCallback(IAsyncResult result)
        {
            WebRequestState state = result.AsyncState as WebRequestState;

            WebResponse response = state.Request.EndGetResponse(result);

            using (MemoryStream ms = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(ms);
                state.Content = ms.ToArray();
            }

            state.Event.Set();
        }
    }
}
