using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Json;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace final_project_be_Application.Service.GoogleMeetService
{
    public class GoogleMeetService : IGoogleMeetService
    {
        private readonly GoogleSettings _googleSettings;
        private readonly ILogger<GoogleMeetService> _logger;
        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "Phronesis Meeting Scheduler";

        public GoogleMeetService(IOptions<GoogleSettings> googleSettings, ILogger<GoogleMeetService> logger)
        {
            _googleSettings = googleSettings.Value;
            _logger = logger;
        }

        public async Task<string> CreateGoogleMeetLinkAsync(string meetingTitle, DateTime startTime, DateTime endTime, string description = "")
        {
            try
            {
                // Lấy UserCredential sử dụng OAuth client credentials
                UserCredential credential;
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GetClientCredentialJson())))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user", // Dùng một user ID cố định cho toàn hệ thống
                        CancellationToken.None,
                        new FileDataStore("Google.Calendar.Auth.Store"));
                }

                // Tạo Calendar service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });

                var calendarEvent = new Event()
                {
                    Summary = meetingTitle,
                    Description = description,
                    Start = new EventDateTime()
                    {
                        DateTime = startTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    End = new EventDateTime()
                    {
                        DateTime = endTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    ConferenceData = new ConferenceData()
                    {
                        CreateRequest = new CreateConferenceRequest()
                        {
                            RequestId = Guid.NewGuid().ToString()
                        }
                    }
                };

                var request = service.Events.Insert(calendarEvent, "primary");
                request.ConferenceDataVersion = 1;
                var createdEvent = await request.ExecuteAsync();

                return createdEvent.HangoutLink;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Google Meet link");
                return null;
            }
        }

        private string GetClientCredentialJson()
        {
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var projectId = Environment.GetEnvironmentVariable("PROJECT_ID");
            var authUri = Environment.GetEnvironmentVariable("AUTH_URI");
            var tokenUri = Environment.GetEnvironmentVariable("TOKEN_URI");
            var authProviderCertUrl = Environment.GetEnvironmentVariable("AUTH_PROVIDER_X509_CERT_URL");
            var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI");

            return $@"{{
        ""installed"": {{
            ""client_id"": ""{clientId}"",
            ""project_id"": ""{projectId}"",
            ""auth_uri"": ""{authUri}"",
            ""token_uri"": ""{tokenUri}"",
            ""auth_provider_x509_cert_url"": ""{authProviderCertUrl}"",
            ""client_secret"": ""{clientSecret}"",
            ""redirect_uris"": [""{redirectUri}""]
        }}
    }}";
        }
    }
} 