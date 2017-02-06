![alt text](https://github.com/lukencode/FluentEmail/blob/master/assets/fluentemail_logo.png?raw=true "FluentEmail")

# FluentEmail - All in one email sender for .NET and .NET Core
Send email from .NET or .NET Core. A bunch of useful extension packages make this dead simple and very powerful.

## Packages

[FluentEmail.Core](src/FluentEmail.Core) - Just the domain model. Includes very basic defaults, but is also included with every other package here.

[FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) - Now we're talking. Send emails via SMTP. At the moment, only works on .NET 4.

[FluentEmail.Razor](src/Renderers/FluentEmail.Razor) - Generate emails using Razor templates. Anything you can do in ASP.NET is possible here. Uses the [RazorLight]() project under the hood. 

[FluentEmail.Mailgun](src/Senders/FluentEmail.Mailgun) - Send emails via MailGun's REST API. Works with .NET Core :)

## Usage

You can choose which renderer and sender you would like to use, or build your own inheriting from ITemplateRenderer and ISender.

Example usage from:

```csharp
var email = Email
    	.From("john@email.com")
    	.To("bob@email.com", "bob")
    	.Subject("hows it going bob")
    	.Body("yo dawg, sup?");
```

Templates usage:

```csharp
// Using Razor templating package
Email.DefaultRenderer = new RazorRenderer();

var template = "Dear @Model.Name, You are totally @Model.Compliment.";

var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplate(template, new { Name = "Luke", Compliment = "Awesome" });
```

Sending:

```csharp
// Using Smtp Sender package
Email.DefaultSender = new SmtpSender();

//send normally
email.Send();

//send asynchronously
await email.SendAsync();
```

<a href="http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/">http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/</a>
