using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace UserRoles
{
    public class CalculatedField : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                decimal field1 = 0;
                decimal field2 = 0;

                // Retrieve field1 value, handle if it doesn't exist
                if (entity.Contains("field1") && entity["field1"] != null)
                {
                    field1 = entity.GetAttributeValue<decimal>("field1");
                }
                else
                {
                    tracingService.Trace("field1 does not exist or is null");
                }

                // Retrieve field2 value, handle if it doesn't exist
                if (entity.Contains("field2") && entity["field2"] != null)
                {
                    field2 = entity.GetAttributeValue<decimal>("field2");
                }
                else
                {
                    tracingService.Trace("field2 does not exist or is null");
                }

                decimal field3 = field1 + field2;

                tracingService.Trace($"field1: {field1}, field2: {field2}, field3: {field3}");

                // Setting the calculated field value back to the entity
                entity["field3"] = field3;

                tracingService.Trace("Calculated field3 value has been set");

            }

        }
    }
}
