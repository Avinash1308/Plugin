using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Webdev
{
    class AssociateDissociate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService _service = factory.CreateOrganizationService(context.UserId);
            EntityReference targetentity = null;
            EntityReference relatedentity = null;
            string relationshipname = string.Empty;
            Entity student = null;
            Entity classentity = null;
            Entity courseentity = null;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
            {
                targetentity = context.InputParameters["Target"] as EntityReference;
                if (context.InputParameters.Contains("Relationship"))
                {
                    relationshipname = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                }
                if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                {
                    EntityReferenceCollection relatedEntityCol = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                    relatedentity = relatedEntityCol[0]; 
                }

                student = _service.Retrieve(targetentity.LogicalName, targetentity.Id, new ColumnSet("dnlb_courseappliedfor", "dnlb_subsidy", "dnlb_tax", "dnlb_totalfee", "dnlb_universityfee"));
                classentity = _service.Retrieve(targetentity.LogicalName, targetentity.Id, new ColumnSet("dnlb_courseid"));
                courseentity = _service.Retrieve("dnlb_course", classentity.GetAttributeValue<EntityReference>("dnlb_courseid").Id, new ColumnSet("dnlb_basicfee");

                // check if the plugin is register for associate
                if(context.MessageName.ToLower()=="associate")
                {
                    if(student.GetAttributeValue<EntityReference>("dnlb_courseappliedfor").Id != classentity.GetAttributeValue<EntityReference>("dnlb_courseid").Id)
                    {
                        throw new Exception("The student not applied for course");// Only throw error without context msg
                        throw new InvalidPluginExecutionException("The course is invalid"); // throw error with msg
                    }

                   Money subsidy = student.GetAttributeValue<Money>("dnlb_subsidy") == null ? new Money(0) :
                        student.GetAttributeValue<Money>("dnlb_subsidy");


                    Money tax = student.GetAttributeValue<Money>("dnlb_tax") == null ? new Money(0) 
                        : student.GetAttributeValue<Money>("dnlb_tax");


                    Money universityfee= student.GetAttributeValue<Money>("dnlb_universityfee") == null ? new Money(0)  
                        : student.GetAttributeValue<Money>("dnlb_universityfee");

                    Money basefee = courseentity.GetAttributeValue<Money>("dnlb_basicfee") == null ? new Money(0)
                       : student.GetAttributeValue<Money>("dnlb_basicfee");


                    Entity updatestudent = new Entity(targetentity.LogicalName);
                    updatestudent.Id = targetentity.Id;
                    updatestudent["dnlb_basicfee"] = new Money(basefee.Value);
                    updatestudent["dnlb_subsidy"] = new Money(subsidy.Value);
                    updatestudent["dnlb_tax"] = new Money(tax.Value);
                    updatestudent["dnlb_universityfee"] = new Money(universityfee.Value);
                    updatestudent["dnlb_totalfee"] = new Money(basefee.Value + universityfee.Value + tax.Value - subsidy.Value);
                    _service.Update(updatestudent);

                }
                else if (context.MessageName.ToLower()=="disassociate")
                {
                    Money basefee = courseentity.GetAttributeValue<Money>("dnlb_basicfee") == null ? new Money(0)
                      : student.GetAttributeValue<Money>("dnlb_basicfee");
                    Entity updatestudent = new Entity(targetentity.LogicalName);
                    updatestudent.Id = targetentity.Id;
                    updatestudent["dnlb_basicfee"] = new Money(0);

                }
            }

        }

    }
}
