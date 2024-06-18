using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OutOfOffice.Web
{
    public class Client
    {
        private readonly string Address;
        private readonly HttpClient HttpClient;
        private Token? Token;
        private bool TokenInvalid;

        public Client(string Address)
        {
            this.Address = Address;
            this.HttpClient = new();
        }

        public HttpResponseMessage SetToken(string Login, string Password)
        {
            var LoginDetails = new Dictionary<string, string>
            {
                { "login", Login },
                { "password", Password }
            };

            HttpResponseMessage Response = SendRequest(HttpMethod.Post, "/user/login", false, LoginDetails);
            if (Response.IsSuccessStatusCode)
            {
                if (GetData<Token>(Response) is Token Token)
                {
                    RefreshTokenTask(Token);
                    this.Token = Token;
                }
            }

            return Response;
        }

        public HttpResponseMessage GetToken(string RefreshToken)
        {
            var LoginDetails = new Dictionary<string, string> { { "token", RefreshToken } };

            HttpResponseMessage Response = SendRequest(HttpMethod.Post, "/user/refreshtoken", false, LoginDetails);
            if (Response.IsSuccessStatusCode)
            {
                Token = new Token
                {
                    RefreshToken = RefreshToken,
                    AccessToken = Task.Run(Response.Content.ReadAsStringAsync).Result
                };
                RefreshTokenTask(Token);
            }

            return Response;
        }

        public HttpResponseMessage SendRequest(HttpMethod Method, string Page, bool RequiresToken, IDictionary<string, string>? Parameters = null, string? FilePath = null)
        {
            var Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(Address + Page),
                Method = Method
            };

            if (Parameters != null)
                Request.Content = new StringContent(JsonSerializer.Serialize(Parameters), Encoding.UTF8, "application/json");

            if (FilePath != null)
                Request.Content = new MultipartFormDataContent { { new StreamContent(File.OpenRead(FilePath)), "file", FilePath[(FilePath.LastIndexOf('/') + 1)..] } };

            if (RequiresToken)
                Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token?.AccessToken);

            try { return HttpClient.Send(Request); }
            catch { return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError }; }
        }

        public static T? GetData<T>(HttpResponseMessage Response)
        {
            try { return JsonSerializer.Deserialize<T>(Response.Content.ReadAsStream(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); }
            catch { return default; }
        }

        public void SaveToken() => File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "Token.txt"), Token?.RefreshToken);

        public void InvalidateToken() => TokenInvalid = true;

        private void RefreshTokenTask(Token Token)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Thread.Sleep(600000); //Refresh token every 10 minutes

                    if (TokenInvalid)
                        break;

                    HttpResponseMessage Response = SendRequest(HttpMethod.Post, "/user/refreshtoken", false, new Dictionary<string, string> { { "token", Token.RefreshToken } });

                    if (Response.IsSuccessStatusCode)
                        Token.AccessToken = await Response.Content.ReadAsStringAsync();
                }
            });
        }
    }
}
