using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CafeMgmtSystem.Services
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;

        public SmsService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _twilioPhoneNumber = config["Twilio:PhoneNumber"];
        }
        public async Task SendOtpAsync(string phoneNumber, string otp)
        {
            TwilioClient.Init(_accountSid, _authToken);

            var messageOptions = new CreateMessageOptions(new Twilio.Types.PhoneNumber(phoneNumber))
            {
                From = new Twilio.Types.PhoneNumber(_twilioPhoneNumber),
                Body = $"Your OTP is: {otp}. This OTP will expire in 5 minutes."
            };

            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
