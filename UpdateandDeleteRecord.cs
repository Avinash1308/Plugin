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
    public class UpdateRecord : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {   // fetch  context of environment
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            // metadata data of environemnt
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // access the metadata of the environemnt
            IOrganizationService _service = factory.CreateOrganizationService(context.UserId);

            // check the primary entity is account 
            // it is used to access trigger entity
            if (context.PrimaryEntityName == "account") { 
            // access entity record
                Entity accountrecord = _service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("accountratingcode", "numberofemployee"));
            // get the field value
                int accountrating = accountrecord.Contains("accountratingcode") ? accountrecord.GetAttributeValue<OptionSetValue>("accountratingcode").Value : 100;
                int numberofemp = accountrecord.Contains("numberofemployee") ? accountrecord.GetAttributeValue<int>("numberofemployee").Value : 100;
             // create new object for account for updating
                Entity accountToUpdate = new Entity("account");
            // Guid of account
                accountToUpdate.Id = context.PrimaryEntityId;
            // condition
                if(accountrating ==1 && numberofemp < 50)
                {
                    // update the field value
                    accountToUpdate["revenue"] = new Money(50);
                }
             // update the record 
                _service.Update(accountToUpdate);
                // delete the record
                _service.Delete("account",context.PrimaryEntityId);
            }
        }
    }
}
