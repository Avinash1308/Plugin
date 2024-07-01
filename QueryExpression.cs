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
            

                QueryExpression queryExpression = new QueryExpression("contact");                                               //create object of queryexpression
                queryExpression.Columnset = new Columnset('fullname', 'telephone1', 'parentcustomerid', 'creaditlimit');       //columnset of query, which column u want to fetch
                queryExpression.Criteria.AddCondition(new ConditionExpressionq('parentcustomerid'.ConditionOperator.equal, context.PrimaryEntityId);        // add condition to retrieve data
                EntityCollection contactRecords = _service.RetrieveMultiple(queryExpression);                                 // fetch all the records which satisfy the above condition


            }
        }
    }
}
