using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arbor.App.Extensions.ExtensionMethods;
using Arbor.AspNetCore.Host.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Arbor.AspNetCore.Host.Mvc
{
    public class ValidationActionFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
                {
                    var parameters = descriptor.MethodInfo.GetParameters();

                    if (parameters.Length == 1)
                    {
                        var parameter = parameters[0];

                        bool hasNoValidationAttribute = parameter.GetCustomAttributes()
                                                                 .OfType<NoValidationAttribute>().Any();

                        if (hasNoValidationAttribute || parameter.ParameterType.HasAttribute<NoValidationAttribute>())
                        {
                            return Task.CompletedTask;
                        }
                    }
                }

                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            return Task.CompletedTask;
        }
    }
}