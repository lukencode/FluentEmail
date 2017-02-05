# Fluent Email - MailGun  

Send email via the MailGun REST API.

## Packages

`FluentEmail.Mailgun`

## Usage

Create a new instance of your sender and add it as the default Fluent Email sender.

    var sender = new MailgunSender(
		"sandboxcf5f41bbf2f84f15a386c60e253b5fe9.mailgun.org", // Mailgun Domain
		"key-8d32c046d7f14ada8d5ba8253e3e30de" // Mailgun API Key
	);
    Email.DefaultSender = sender;

	/*
		You can optionally set the sender per instance like so:
		
		email.Sender = new MailgunSender(...);
	*/

Send the email in the usual Fluent Email way.

    var email = Email
        .From(fromEmail)
        .To(toEmail)
        .Subject(subject)
        .Body(body);

    var response = await email.SendAsync();


## Further Information

Don't forget you can use Razor templating using the [FluentEmail.Razor](../../Renderers/FluentEmail.Razor) package as well.

If you'd like to create your own sender for another service, check out the source code. All you need to do is implement the `ISender` interface :)
