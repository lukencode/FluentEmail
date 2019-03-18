using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core
{
    public interface IFluentEmail: IHideObjectMembers
	{
	    EmailData Data { get; set; }
	    ITemplateRenderer Renderer { get; set; }
	    ISender Sender { get; set; }

	    /// <summary>
	    /// Adds a reciepient to the email, Splits name and address on ';'
	    /// </summary>
	    /// <param name="emailAddress">Email address of recipeient</param>
	    /// <param name="name">Name of recipient</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail To(string emailAddress, string name = null);

        /// <summary>
        /// Set the send from email address
        /// </summary>
        /// <param name="emailAddress">Email address of sender</param>
        /// <param name="name">Name of sender</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail SetFrom(string emailAddress, string name = null);

        /// <summary>
        /// Adds a reciepient to the email
        /// </summary>
        /// <param name="emailAddress">Email address of recipeient (allows multiple splitting on ';')</param>
        /// <returns></returns>
        IFluentEmail To(string emailAddress);

	    /// <summary>
	    /// Adds all reciepients in list to email
	    /// </summary>
	    /// <param name="mailAddresses">List of recipients</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail To(IList<Address> mailAddresses);

	    /// <summary>
	    /// Adds a Carbon Copy to the email
	    /// </summary>
	    /// <param name="emailAddress">Email address to cc</param>
	    /// <param name="name">Name to cc</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail CC(string emailAddress, string name = "");

	    /// <summary>
	    /// Adds all Carbon Copy in list to an email
	    /// </summary>
	    /// <param name="mailAddresses">List of recipients to CC</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail CC(IList<Address> mailAddresses);

	    /// <summary>
	    /// Adds a blind carbon copy to the email
	    /// </summary>
	    /// <param name="emailAddress">Email address of bcc</param>
	    /// <param name="name">Name of bcc</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail BCC(string emailAddress, string name = "");

	    /// <summary>
	    /// Adds all blind carbon copy in list to an email
	    /// </summary>
	    /// <param name="mailAddresses">List of recipients to BCC</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail BCC(IList<Address> mailAddresses);

	    /// <summary>
	    /// Sets the ReplyTo address on the email
	    /// </summary>
	    /// <param name="address">The ReplyTo Address</param>
	    /// <returns></returns>
	    IFluentEmail ReplyTo(string address);

	    /// <summary>
	    /// Sets the ReplyTo address on the email
	    /// </summary>
	    /// <param name="address">The ReplyTo Address</param>
	    /// <param name="name">The Display Name of the ReplyTo</param>
	    /// <returns></returns>
	    IFluentEmail ReplyTo(string address, string name);

	    /// <summary>
	    /// Sets the subject of the email
	    /// </summary>
	    /// <param name="subject">email subject</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail Subject(string subject);

	    /// <summary>
	    /// Adds a Body to the Email
	    /// </summary>
	    /// <param name="body">The content of the body</param>
	    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
	    IFluentEmail Body(string body, bool isHtml = false);

	    /// <summary>
	    /// Marks the email as High Priority
	    /// </summary>
	    IFluentEmail HighPriority();

	    /// <summary>
	    /// Marks the email as Low Priority
	    /// </summary>
	    IFluentEmail LowPriority();

	    /// <summary>
	    /// Set the template rendering engine to use, defaults to RazorEngine
	    /// </summary>
	    IFluentEmail UsingTemplateEngine(ITemplateRenderer renderer);

	    /// <summary>
	    /// Adds template to email from embedded resource
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="path">Path the the embedded resource eg [YourAssembly].[YourResourceFolder].[YourFilename.txt]</param>
	    /// <param name="model">Model for the template</param>
	    /// <param name="assembly">The assembly your resource is in. Defaults to calling assembly.</param>
	    /// <returns></returns>
	    IFluentEmail UsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly, bool isHtml = true);

        /// <summary>
        /// Adds the template file to the email
        /// </summary>
        /// <param name="filename">The path to the file to load</param>
        /// <param name="model">Model for the template</param>
	    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail UsingTemplateFromFile<T>(string filename, T model, bool isHtml = true);

	    /// <summary>
	    /// Adds a culture specific template file to the email
	    /// </summary>
	    /// <param name="filename">The path to the file to load</param>
	    /// /// <param name="model">The razor model</param>
	    /// <param name="culture">The culture of the template (Default is the current culture)</param>
	    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture, bool isHtml = true);

        /// <summary>
        /// Adds razor template to the email
        /// </summary>
        /// <param name="template">The razor template</param>
        /// <param name="model">Model for the template</param>
	    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail UsingTemplate<T>(string template, T model, bool isHtml = true);

	    /// <summary>
	    /// Adds an Attachment to the Email
	    /// </summary>
	    /// <param name="attachment">The Attachment to add</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail Attach(Attachment attachment);

	    /// <summary>
	    /// Adds Multiple Attachments to the Email
	    /// </summary>
	    /// <param name="attachments">The List of Attachments to add</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail Attach(IList<Attachment> attachments);

	    /// <summary>
	    /// Sends email synchronously
	    /// </summary>
	    /// <returns>Instance of the Email class</returns>
	    SendResponse Send(CancellationToken? token = null);

        /// <summary>
        /// Sends email asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
	    Task<SendResponse> SendAsync(CancellationToken? token = null);

	    IFluentEmail AttachFromFilename(string filename,  string contentType = null, string attachmentName = null);

	    /// <summary>
	    /// Adds a Plaintext alternative Body to the Email. Used in conjunction with an HTML email,
	    /// this allows for email readers without html capability, and also helps avoid spam filters.
	    /// </summary>
	    /// <param name="body">The content of the body</param>
	    IFluentEmail PlaintextAlternativeBody(string body);

	    /// <summary>
	    /// Adds template to email from embedded resource
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="path">Path the the embedded resource eg [YourAssembly].[YourResourceFolder].[YourFilename.txt]</param>
	    /// <param name="model">Model for the template</param>
	    /// <param name="assembly">The assembly your resource is in. Defaults to calling assembly.</param>
	    /// <returns></returns>
	    IFluentEmail PlaintextAlternativeUsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly);

	    /// <summary>
	    /// Adds the template file to the email
	    /// </summary>
	    /// <param name="filename">The path to the file to load</param>
	    /// <param name="model">Model for the template</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail PlaintextAlternativeUsingTemplateFromFile<T>(string filename, T model);

	    /// <summary>
	    /// Adds a culture specific template file to the email
	    /// </summary>
	    /// <param name="filename">The path to the file to load</param>
	    /// /// <param name="model">The razor model</param>
	    /// <param name="culture">The culture of the template (Default is the current culture)</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail PlaintextAlternativeUsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture);

	    /// <summary>
	    /// Adds razor template to the email
	    /// </summary>
	    /// <param name="template">The razor template</param>
	    /// <param name="model">Model for the template</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail PlaintextAlternativeUsingTemplate<T>(string template, T model);

	    /// <summary>
	    /// Adds tag to the Email. This is currently only supported by the Mailgun provider. <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
	    /// </summary>
	    /// <param name="tag">Tag name, max 128 characters, ASCII only</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail Tag(string tag);

	    /// <summary>
	    /// Adds header to the Email.
	    /// </summary>
	    /// <param name="header">Header name, only printable ASCII allowed.</param>
	    /// <param name="body">value of the header</param>
	    /// <returns>Instance of the Email class</returns>
	    IFluentEmail Header(string header, string body);
    }
}
