using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Emailer.Helpers;
using Emailer.Models;
using Emailer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emailer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private SmtpClient _smtpClient;
        private readonly ITokenizationService _tokenizationService;
        private readonly EmailSettings _emailSettings;

        public ContactUsController(ITokenizationService tokenizationService, SmtpClient smtpClient, EmailSettings emailSettings)
        {
            _smtpClient = smtpClient;
            _emailSettings = emailSettings;
            _tokenizationService = tokenizationService;
        }

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
                    _smtpClient.Send(new MailMessage(_emailSettings.SystemAddress, Constants.CONTACT_US_TO_EMAIL, Constants.CONTACT_US_SUBJECT, tokenizedHtml) { IsBodyHtml = true });
                }
                catch (Exception ex)
                {
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
