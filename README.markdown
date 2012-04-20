Playing around with a fluent email class in c#

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
var template = "Dear @Model.Name, You are totally @Model.Compliment.";

var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplate(template, new { Name = "Luke", Compliment = "Awesome" });
```

Sending:

```csharp
//send normally
email.Send();

//send asynchronously
email.Sendsync(MailDeliveredCallback);
```

#### Markdown template support!

New support for using Markdown templates has been added. This uses the Markdown Razor support in [ServiceStack](https://github.com/ServiceStack/ServiceStack/wiki/Markdown-Razor) and supports full model binding as available in standard Razor.

Example using a Markdown template:

```csharp
var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingMarkdownTemplateFromFile(@"~/test.md", new { Name = "Luke", Numbers = new string[] { "1", "2", "3" } });
```

test.md:

```markdown
# Heading 1

This is a [Markdown](http://mouapp.com) page

- one
- two
- three

You can also bind to Model

Name: @Model.Name

Numbers:

@foreach i in Model.Numbers {
- Number: @i
}

And do cool stuff like get the current date/time @DateTime.Now
```

This will be the rendered output (Message.Body):

```html
<p>This is a <a href="http://mouapp.com">Markdown</a> page</p>

<ul>
<li>one</li>
<li>two</li>
<li>three</li>
</ul>

<p>You can also bind to Model</p>

<p>Name: LUKE</p>

<p>Numbers:</p>

<ul>
<li>Number: 1</li>
<li>Number: 2</li>
<li>Number: 3</li>
</ul>

<p>And do cool stuff like get the current date/time 20/04/2012 4:52:33 PM</p>
```

<a href="http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/">http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/</a>