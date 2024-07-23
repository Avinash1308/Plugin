using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;


namespace UserRoles
{
    public class AssignSecurityRole : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName == "systemuser")
                {
                    Guid roleid = new Guid("e5af511e-4b50-ea11-a811-000d3af05a1b");
                    AssignRequest assignRequest = new AssignRequest();
                    assignRequest.Assignee = new EntityReference("systemuser", entity.Id);
                    assignRequest.Target = new EntityReference("role", roleid);
                    
                    service.Execute(assignRequest);
                }
            }

        } 
    }
}
