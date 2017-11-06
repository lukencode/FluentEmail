![alt text](https://github.com/lukencode/FluentEmail/raw/master/assets/fluentemail_logo_64x64.png "FluentEmail")

# FluentEmail - All in one email sender for .NET and .NET Core
Send email from .NET or .NET Core. A bunch of useful extension packages make this dead simple and very powerful.

## Packages

[FluentEmail.Core](src/FluentEmail.Core) - Just the domain model. Includes very basic defaults, but is also included with every other package here.

[FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) - Now we're talking. Send emails via SMTP.

[FluentEmail.Razor](src/Renderers/FluentEmail.Razor) - Generate emails using Razor templates. Anything you can do in ASP.NET is possible here. Uses the [RazorLight]() project under the hood. 

[FluentEmail.Mailgun](src/Senders/FluentEmail.Mailgun) - Send emails via MailGun's REST API.

[FluentEmail.SendGrid](src/Senders/FluentEmail.Sendgrid) - Send email via the SendGrid API.

## Usage

You can choose which renderer and sender you would like to use, or build your own inheriting from ITemplateRenderer and ISender.

**Basic Usage**

```csharp
var email = Email
    	.From("john@email.com")
    	.To("bob@email.com", "bob")
    	.Subject("hows it going bob")
    	.Body("yo dawg, sup?")
		.Send();
```

**Using a template**

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

**Sending Emails**

```csharp
// Using Smtp Sender package
Email.DefaultSender = new SmtpSender();

//send normally
email.Send();

//send asynchronously
await email.SendAsync();
```

**Template File from Disk**  

```csharp
var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
	.UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Mytemplate.cshtml", new { Name = "Rad Dude" });
```

**Embedded Template File**  

**Note for .NET Core 2 users:** You'll need to add the following line to the project containing any embedded razor views. See [this issue for more details](https://github.com/aspnet/Mvc/issues/6021).

```xml
<MvcRazorExcludeRefAssembliesFromPublish>false</MvcRazorExcludeRefAssembliesFromPublish>
```

```csharp
var email = new Email("bob@hotmail.com")
	.To("benwholikesbeer@twitter.com")
	.Subject("Hey cool name!")
	.UsingTemplateFromEmbedded("Example.Project.Namespace.template-name.cshtml", 
		new { Name = "Bob" }, 
		TypeFromYourEmbeddedAssembly.GetType().GetTypeInfo().Assembly);
```

**More Info**

<a href="http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/">http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/</a>


## Development and Beta Packages

If you need a pre-release version, you can add the MyGet feed to your nuget package sources.  
<https://www.myget.org/F/fluentemail/api/v3/index.json>
