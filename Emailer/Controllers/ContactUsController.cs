using System;
using System.Threading.Tasks;
using Emailer.Helpers;
using Emailer.Models;
using Emailer.Services;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Emailer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        #region ctor
        private SendGridClient _smtpClient;
        private readonly ITokenizationService _tokenizationService;
        private readonly EmailSettings _emailSettings;

        public ContactUsController(ITokenizationService tokenizationService, SendGridClient smtpClient, EmailSettings emailSettings)
        {
            _smtpClient = smtpClient;
            _emailSettings = emailSettings;
            _tokenizationService = tokenizationService;
        } 
        #endregion              

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ContactUsRequest contactUsRequest)
        {
            // TODO: it is likely they will want to store contacts in a database
            // TODO: filter duplicate requests
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                try
                {
                    var tokenizedHtml = await _tokenizationService.ReplaceTokens(Constants.CONTACT_US_HTML, contactUsRequest);
                    await _smtpClient.SendEmailAsync(MailHelper.CreateSingleEmail(
                        from: new EmailAddress(_emailSettings.SystemAddress),
                        to: new EmailAddress(_emailSettings.SystemAddress), 
                        subject: Constants.CONTACT_US_SUBJECT,
                        plainTextContent: null,
                        htmlContent: tokenizedHtml));

                    var thankyouHtml = await _tokenizationService.ReplaceTokens(Constants.THANK_YOU_HTML, contactUsRequest);
                    await _smtpClient.SendEmailAsync(MailHelper.CreateSingleEmail(
                        from: new EmailAddress(_emailSettings.SystemAddress),
                        to: new EmailAddress(contactUsRequest.Email),
                        subject: Constants.THANK_YOU_SUBJECT,
                        plainTextContent: null,
                        htmlContent: thankyouHtml));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception!!! " + ex.Message);
                    throw;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // TODO: Handle critical error
                return UnprocessableEntity();
            }
        }
    }
}
