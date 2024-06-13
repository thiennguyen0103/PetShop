using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PetShop.Application.Interfaces;
using PetShop.Application.Models.Email;
using PetShop.Domain.Settings;
using MailKit.Security;
using PetShop.Application.Exceptions;
using Mailjet.Client;
using PetShop.Domain;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace PetShop.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        public MailSettings _mailSettings { get; }
        public MailjetSettings _mailjetSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<MailSettings> mailSettings,
            IOptions<MailjetSettings> mailjetSettings,
            ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _mailjetSettings = mailjetSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                // Create message
                var emailMessage = new MimeMessage
                {
                    Sender = new MailboxAddress(_mailSettings.DisplayName, request.From ?? _mailSettings.EmailFrom),
                };
                emailMessage.To.Add(MailboxAddress.Parse(request.To));
                emailMessage.Subject = request.Subject;

                var builder = new BodyBuilder();
                emailMessage.Body = builder.ToMessageBody();

                // Convert MimeMessage to Mailjet format
                var emailContent = emailMessage.HtmlBody ?? emailMessage.TextBody;

                MailjetClient client = new(_mailjetSettings.ApiKey, _mailjetSettings.ApiSecret);
                MailjetRequest mailjetRequest = new MailjetRequest
                {
                    Resource = Send.Resource
                }.Property(Send.FromEmail, _mailSettings.EmailFrom)
                .Property(Send.FromName, _mailSettings.DisplayName)
                .Property(Send.Subject, request.Subject)
                .Property(Send.HtmlPart, emailContent)
                .Property(Send.Recipients, new JArray{
                    new JObject{
                        {"Email", request.To}
                    }
                });

                MailjetResponse response = await client.PostAsync(mailjetRequest);

                if (!response.IsSuccessStatusCode)
                {
                    // Handle error
                    throw new ApiException($"Failed to send email: {response.StatusCode} {response.GetData()}");
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message, error);
                throw new ApiException(error.Message);
            }
        }
    }
}
