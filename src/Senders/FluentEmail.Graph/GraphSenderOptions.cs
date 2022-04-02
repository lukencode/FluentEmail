namespace FluentEmail.Graph
{
    /// <summary>
    /// Contains settings needed for the <see cref="GraphSender"/>.
    /// </summary>
    public class GraphSenderOptions
    {
        /// <summary>
        /// Gets or sets the Client ID (also known as App ID) of the application as registered in the application registration portal.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Tenant Id of the organization from which the application will let users sign-in. This is classically a GUID or a domain name.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the secret string previously shared with AAD at application registration to prove the identity of the application (the client) requesting the tokens.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to save the message in Sent Items. Default is <c>true</c>.
        /// </summary>
        public bool? SaveSentItems { get; set; }
    }
}
