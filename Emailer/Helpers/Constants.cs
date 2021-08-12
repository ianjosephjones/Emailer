namespace Emailer.Helpers
{
    public static class Constants
    {
        public static string CONTACT_US_SUBJECT { get; } = "New Contact Us Request";
        public static string CONTACT_US_HTML { get; } = "HTML\\ContactUs.html";
        public static string THANK_YOU_HTML { get; } = "HTML\\ThankYou.html";
        // TODO: Set ContactUs to email address
        public static string CONTACT_US_TO_EMAIL { get; } = "ianjosephjones@gmail.com";

        public static string EMAIL_RESPONSE_SUCCESS { get; } = "Success";
    }
}
