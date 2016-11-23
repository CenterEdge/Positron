using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Positron.Server.Hosting
{
    class PositronModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var constraint = new AssemblyActionConstraintAttribute();

            foreach (var controller in application.Controllers)
            {
                controller.Selectors[0].ActionConstraints.Add(constraint);
            }
        }
    }
}
