Playing around with a fluent email class in c#

Example usage from:

	var email = Email
            	.From("john@email.com")
            	.To("bob@email.com", "bob")
            	.Subject("hows it going bob")
            	.Body("yo dawg, sup?");


Templates usage:
    var email = Email
            	.From("john@email.com")
            	.To("bob@email.com", "bob")
            	.Subject("hows it going bob")
                .UsingTemplate(@"C:\Emailer\TransactionTemplate.htm")
                .Replace("<%CurrentDate%>", DateTime.Now.ToShortDateString())
                .Replace("<%FullName%>", fullName)
                .Replace("<%SaleDate%>", saleDate)
 
	//send normally
	email.Send();
 
	//send asynchronously
	email.SendAsync(MailDeliveredCallback);


<a href="http://lukencode.com/2010/04/11/fluent-email-in-net">http://lukencode.com/2010/04/11/fluent-email-in-net</a>