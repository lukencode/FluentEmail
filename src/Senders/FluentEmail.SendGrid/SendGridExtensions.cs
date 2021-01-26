using FluentEmail.Core;

namespace FluentEmail.SendGrid
{
    public static class SendGridExtensions
    {
        public static IFluentEmail DisableClickTracking(this IFluentEmail email)
        {
            email.Context["ClickTrackingDisabled"] = true;
            return email;
        }

        internal static bool ClickTrackingDisabled(this IFluentEmail email)
        {
            if (!email.Context.TryGetValue("ClickTrackingDisabled", out var value))
            {
                return false;
            }

            return (bool) value;
        }
    }
}