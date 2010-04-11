Playing around with a fluent email class in c#

Example usage from:

	var email = Email
            	.From("john@email.com")
            	.To("bob@email.com", "bob")
            	.Subject("hows it going bob")
            	.Body("yo dawg, sup?");
 
	//send normally
	email.Send();
 
	//send asynchronously
	email.SendAsync(MailDeliveredCallback);

http://lukencode.com/2010/04/11/fluent-email-in-net