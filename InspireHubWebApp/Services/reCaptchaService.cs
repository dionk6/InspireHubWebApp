using System.Text.Json;

namespace InspireHubWebApp.Services
{
    public class reCaptchaService
    {
        public virtual async Task<reCaptchaRespo> tokenVerify(string token)
        {
            reCaptchaData data = new reCaptchaData
            {
                response = token,
                secret = "6LcHKcclAAAAAH1Dc8fESPZNHzRPzqQz1OVoculd"
            };

            HttpClient client = new HttpClient();
            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
            var response = await client.GetStringAsync($"{googleVerificationUrl}?secret={data.secret}&response={data.response}");
            var reCaptcharesponse = JsonSerializer.Deserialize<reCaptchaRespo>(response);
            return reCaptcharesponse;
        }
    }
    public class reCaptchaData
    {
        public string response { get; set; }
        public string secret { get; set; }
    }
    public class reCaptchaRespo
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public double score { get; set; }
    }
}
