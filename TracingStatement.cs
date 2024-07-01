using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Webdev
{
    public class Program:IPlugin
    {
        public void Execute(IServiceProvider serviceProvider) {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService _service = factory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService)); // tracing statement 
            if (context.PrimaryEntityName == "account")
            {
                tracingService.Trace("Enter the method");
                Entity accountrecord = _service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("name", "telephone1"));
                string accountname = accountrecord.GetAttributeValue<string>("name");
                string accounttelephone = accountrecord.GetAttributeValue<string>("telephone1");
                tracingService.Trace("Retrieve data from account");


                //Create Operation
                Entity contactrecord = new Entity("contact");
                contactrecord["fullname"] = accountname;
                contactrecord["telephone1"] = accounttelephone;
                contactrecord["primarycontactid"] = new EntityReference("account",context.PrimaryEntityId);
                contactrecord["familystatuscode"] = new OptionSetValue(2);
                contactrecord["creditlimit"] = new Money(2000);
                contactrecord["lastonholdtime"] = new DateTime(2023, 01, 01);
                contactrecord["donotphone"] = true;
                contactrecord["numberofchilren"] = 0;
                Guid ContactId =_service.Create(contactrecord);
                tracingService.Trace("Record is successfully created");
                
                
            }
        }
    }
}
