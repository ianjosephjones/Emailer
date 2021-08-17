using Emailer.Helpers;
using System.Reflection;
using System.Threading.Tasks;

namespace Emailer.Services
{
    public interface ITokenizationService
    {
        Task<string> ReplaceTokens(string emailTemplatePath, object emailModel);
    }

    public class TokenizationService : ITokenizationService
    {
        private readonly EmailSettings _emailSettings;

        public TokenizationService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public async Task<string> ReplaceTokens(string emailTemplatePath, object emailModel)
        {
            // TODO: this will likely come from a database
            var emailHtml = string.Join(string.Empty, await System.IO.File.ReadAllTextAsync(emailTemplatePath));

            // model replacement
            var emailModelProperties = emailModel.GetType().GetProperties();
            foreach (var emailModelProperty in emailModelProperties)
            {
                // TODO: throws if null, fix
                var matchingEmailvalueProperty = GetPropertyValue(emailModel, emailModelProperty.Name).ToString();
                emailHtml = emailHtml.Replace("{{" + emailModelProperty.Name + "}}", matchingEmailvalueProperty.ToString());
            }

            // environment variable replacement
            emailHtml = emailHtml.Replace("{{BaseUrl}}", _emailSettings.BaseUrl);

            return emailHtml;
        }

        private static object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            return property.GetValue(source, null);
        }
    }
}
