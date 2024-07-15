using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text.RegularExpressions;


namespace Emailvalidation
{
    public class emailValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService _service = factory.CreateOrganizationService(context.UserId);

            try
            {
                // Log entry point
                tracingService.Trace("EmailValidation Plugin: Entering Execute method.");

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];

                    // Check if the entity has an email attribute and it's not null
                    if (entity.LogicalName == "account" && entity.Attributes.Contains("emailaddress1"))
                    {
                        string email = entity.GetAttributeValue<string>("emailaddress1");

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            throw new InvalidPluginExecutionException("The email address cannot be empty.");
                        }

                        if (!IsValidEmail(email))
                        {
                            throw new InvalidPluginExecutionException("The email address format is invalid.");
                        }
                    }
                }

                // Log exit point
                tracingService.Trace("EmailValidation Plugin: Exiting Execute method.");
            }
            catch (Exception ex)
            {
                tracingService.Trace("EmailValidation Plugin: {0}", ex.ToString());
                throw new InvalidPluginExecutionException($"An error occurred in the EmailValidation plug-in: {ex.Message}", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Regular expression for validating an Email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
