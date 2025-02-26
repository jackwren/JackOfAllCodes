using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace JackOfAllCodes.Web.Services
{
    public class EmailSenderService
    {
        private readonly IAmazonSimpleEmailService _sesClient;

        public EmailSenderService(IConfiguration configuration)
        {
            var regionName = configuration["AWS:Region"]; // Reads from AWS:Region in appsettings or environment variables
            var region = RegionEndpoint.GetBySystemName(regionName);

            // Use Default AWS Credentials (IAM Role or ~/.aws/credentials). Please see ReadMe for more information.
            var credentials = FallbackCredentialsFactory.GetCredentials();

            _sesClient = new AmazonSimpleEmailServiceClient(credentials, region);
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var fromAddress = "no-reply@jackofallcodes.co.uk";
            var sendRequest = new SendEmailRequest
            {
                Source = fromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { toEmail }
                },
                Message = new Message
                {
                    Subject = new Content("Password Reset"),
                    Body = new Body
                    {
                        Html = new Content($@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; background-color: #f9f9f9;'>
                            <h2 style='color: #333;'>Password Reset Request</h2>
                            <p style='color: #555;'>Hi there,</p>
                            <p style='color: #555;'>
                                We received a request to reset your password. Click the button below to proceed:
                            </p>

                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 12px 20px; text-decoration: none; border-radius: 5px; display: inline-block; font-size: 16px;'>
                                    Reset Password
                                </a>
                            </div>

                            <p style='color: #555;'>
                                If you didn’t request this password reset, you can safely ignore this email. This link will expire in 30 minutes.
                            </p>

                            <hr style='border: none; border-top: 1px solid #ddd;' />

                            <p style='font-size: 12px; color: #999; text-align: center;'>
                                &copy; {DateTime.Now.Year} JackOfAllCodes. All rights reserved.
                            </p>
                        </div>")
                    }
                }
            };

            try
            {
                var response = await _sesClient.SendEmailAsync(sendRequest);
                Console.WriteLine("Email sent successfully: " + response.MessageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
