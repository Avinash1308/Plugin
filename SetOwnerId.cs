using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace UserRoles
{
    public class SetOwner : IPlugin
    {
       public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity contact = (Entity)context.InputParameters["Target"];
                if(contact.Attributes.Contains("parentCustomerId") && contact["parentCustomerId"] is EntityReference)
                {
                    EntityReference AccountRef = (EntityReference)contact["parentCustomerId"];
                    Entity ParentAccount = service.Retrieve("account", AccountRef.Id, new ColumnSet("ownerid"));
                    if(ParentAccount != null && ParentAccount.Attributes.Contains("ownerid"))
                    {
                        EntityReference Ownerid = (EntityReference)ParentAccount["ownerid"];
                        contact["ownerid"] = Ownerid;
                    }

                }
                service.Update(contact);
            }
            


        }
    }
}

