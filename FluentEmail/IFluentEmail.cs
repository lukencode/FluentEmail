namespace FluentEmail
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net.Mail;
	using System.Reflection;
	using System.Threading;

	public interface IFluentEmail: IHideObjectMembers, IDisposable
	{		
		MailMessage Message { get; set; }
		IFluentEmail UsingClient(SmtpClient client);
		IFluentEmail To(string emailAddress, string name);
		IFluentEmail To(string emailAddress);
		IFluentEmail To(IList<MailAddress> mailAddresses);
		IFluentEmail CC(string emailAddress, string name = "");
		IFluentEmail CC(IList<MailAddress> mailAddresses);
		IFluentEmail BCC(string emailAddress, string name = "");
		IFluentEmail BCC(IList<MailAddress> mailAddresses);
		IFluentEmail ReplyTo(string address);
		IFluentEmail ReplyTo(string address, string name);
		IFluentEmail Subject(string subject);
		IFluentEmail Body(string body);
		IFluentEmail HighPriority();
		IFluentEmail LowPriority();
		IFluentEmail Attach(Attachment attachment);
		IFluentEmail Attach(IList<Attachment> attachments);
		IFluentEmail BodyAsHtml();
		IFluentEmail BodyAsPlainText();
		IFluentEmail UsingTemplateEngine(ITemplateRenderer renderer);
		IFluentEmail UsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly = null);
		IFluentEmail UsingTemplateFromFile<T>(string filename, T model);
		IFluentEmail UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture = null);
		IFluentEmail UsingTemplate<T>(string template, T model, bool isHtml = true);		
		IFluentEmail Send();
		IFluentEmail SendAsync(SendCompletedEventHandler callback, object token = null);
		IFluentEmail Cancel();
		
	}
}
