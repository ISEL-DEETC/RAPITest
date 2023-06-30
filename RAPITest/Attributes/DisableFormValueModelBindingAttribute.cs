using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using System;

namespace RAPITest.Attributes
{
    //Used for large file uploads to prevent the form data from being read into memory
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter 
    {
        private readonly ILogger _logger = Log.Logger;

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                var factories = context.ValueProviderFactories;
                factories.RemoveType<FormValueProviderFactory>();
                factories.RemoveType<FormFileValueProviderFactory>();
                factories.RemoveType<JQueryFormValueProviderFactory>();
            }
            catch (Exception ex)
            {
                _logger.Error("Error during OnResourceExecuting, printing exception...");
                _logger.Error(ex.Message);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
