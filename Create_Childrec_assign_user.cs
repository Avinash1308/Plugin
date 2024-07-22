using System;
using Microsoft.Xrm.Sdk;

namespace UserRoles
{
    public class TaskCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // Check if the plugin is executing in response to an account creation message.
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity && context.MessageName.ToLower() == "create")
            {
                Entity account = (Entity)context.InputParameters["Target"];

                // Check if the account has an owner.
                if (account.Attributes.Contains("ownerid") && account["ownerid"] is EntityReference)
                {
                    EntityReference ownerRef = (EntityReference)account["ownerid"];

                    // Create a new contact entity.
                    Entity contact = new Entity("contact");
                    contact["firstname"] = "Avinash";
                    contact["lastname"] = "Kedar";
                    contact["parentcustomerid"] = new EntityReference(account.LogicalName, account.Id); // Set parentcustomerid to the account

                    // Assign the contact to the owner of the account.
                    contact["ownerid"] = new EntityReference(ownerRef.LogicalName, ownerRef.Id);

                    // Create the contact record.
                    Guid contactId = service.Create(contact);
                }
            }
        }
    }
}
