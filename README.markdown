Playing around with a fluent email class in c#

Example usage from:

	var email = Email
            	.From("john@email.com")
            	.To("bob@email.com", "bob")
            	.Subject("hows it going bob")
            	.Body("yo dawg, sup?");


Templates usage:

	var template = "Dear @Model.Name, You are totally @Model.Compliment.";
 
	var email = Email
            .From("bob@hotmail.com")
            .To("somedude@gmail.com")
            .Subject("woo nuget")
            .UsingTemplate(template, new { Name = "Luke", Compliment = "Awesome" });


Sending:
 
	//send normally
	email.Send();
 
	//send asynchronously
	email.SendAsync(MailDeliveredCallback);


<a href="http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/">http://lukencode.com/2011/04/30/fluent-email-now-supporting-razor-syntax-for-templates/</a>