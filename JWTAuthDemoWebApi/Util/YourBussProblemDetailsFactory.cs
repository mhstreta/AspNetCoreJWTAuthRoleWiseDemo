using JWTAuthDemoWebApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Util
{
    public class YourBussProblemDetailsFactory : Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;

        /// <inheritdoc />
        public YourBussProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            statusCode ??= 500; // <-- Microsoft hard codes the value? Why aren't they using StatusCodes.Status500InternalServerError?

            ProblemDetails problemDetails = null;

            var context = httpContext.Features.Get<IExceptionHandlerFeature>();

            if (context?.Error != null)
            {
                if (context.Error is YourBussException ybe)
                {
                    statusCode = 400;
                    httpContext.Response.StatusCode = statusCode.Value; // <-- The result serializer doesn't use the status from the 
                                                                        //	ProblemDetails object to set this code. You have to set
                                                                        //	it by hand.

                    problemDetails = new ProblemDetails
                    {
                        Status = statusCode,
                        Title = ybe.Title,
                        Type = ybe.Type,
                        Detail = ybe.Message,
                        Instance = instance,
                        Extensions =
                        {
                            { "extinfo", ybe.ExtendedInfo }
                        }
                    };
                }
            }

            if (problemDetails == null)
            {
                //	default exception handler
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Type = type,
                    Detail = detail,
                    Instance = instance,
                };
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        /// <inheritdoc />
        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            var problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            //problemDetails.Status ??= statusCode;
            problemDetails.Status = problemDetails.Status ?? statusCode;

            if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }
        }
    }
}
