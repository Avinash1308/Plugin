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
    {   //Fetch xml query is assigned to string variable
        public const string GetContactRecord =
            @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                <entity name=""contact"">
                    <attribute name=""fullname"" />
                    <attribute name=""telephone1"" />
                    <attribute name=""contactid"" />
                    <order attribute=""fullname"" descending=""false"" />
                    <filter type=""and"">
                        <condition attribute=""parentcustomerid"" operator=""eq"" uiname=""Bajaj Finance"" uitype=""account"" value=""{0}"" />
                     </filter>
                </entity>
            </fetch>";

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
            // Expresion to pass dynamic parameter into FetchXML Query
                EntityCollection ContactRecordFetchXML = _service.RetrieveMultiple(new FetchExpression(string.Format(GetContactRecord, context.PrimaryEntityId)));

            }
        }
    }
}
