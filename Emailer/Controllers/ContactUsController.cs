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
                        new EmailAddress(_emailSettings.SystemAddress),
                        new EmailAddress(Constants.CONTACT_US_TO_EMAIL), 
                        Constants.CONTACT_US_SUBJECT, 
                        null, 
                        tokenizedHtml));

                    var thankyouHtml = await _tokenizationService.ReplaceTokens(Constants.THANK_YOU_HTML, contactUsRequest);
                    await _smtpClient.SendEmailAsync(MailHelper.CreateSingleEmail(
                        new EmailAddress(_emailSettings.SystemAddress),
                        new EmailAddress(contactUsRequest.Email),
                        Constants.CONTACT_US_SUBJECT,
                        null,
                        thankyouHtml));
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
