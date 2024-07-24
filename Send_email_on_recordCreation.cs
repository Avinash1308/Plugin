using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ContactCreationPlugin
{
    public class SendEmailOnContactCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "contact")
                    return;

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Retrieve contact's email and other necessary details
                    var contactId = entity.Id;
                    Entity contact = service.Retrieve("contact", contactId, new ColumnSet("emailaddress1", "fullname"));

                    if (contact != null && contact.Contains("emailaddress1"))
                    {
                        // Create the email
                        Entity email = new Entity("email");
                        email["subject"] = "Welcome to Our Service!";
                        email["description"] = $"Hello {contact["fullname"]},\n\nThank you for registering with us.";
                        email["directioncode"] = true; // Outgoing email
                        email["to"] = new EntityCollection(new Entity[] { new Entity("activityparty") { ["partyid"] = contact.ToEntityReference() } });
                        email["from"] = new EntityCollection(new Entity[] { new Entity("activityparty") { ["partyid"] = new EntityReference("systemuser", context.UserId) } });

                        Guid emailId = service.Create(email);

                        // Send the email
                        SendEmailRequest sendEmailRequest = new SendEmailRequest
                        {
                            EmailId = emailId,
                            TrackingToken = "",
                            IssueSend = true
                        };
                        service.Execute(sendEmailRequest);
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace("SendEmailOnContactCreation Plugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
