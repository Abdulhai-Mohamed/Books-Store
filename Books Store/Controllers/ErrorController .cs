using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Books_Store.Controllers
{
    public class ErrorController : Controller
    {

        private readonly ILogger<ErrorController> ILogger_ErrorController_;

        //Inject ASP.NET Core ILogger service. Specify the Controller
        //Type as the generic parameter. This helps us identify later
        //which class or controller has logged the exception


        public ErrorController(ILogger<ErrorController> logger)
        {
            this.ILogger_ErrorController_ = logger;
        }





        #region 1-Centralised 404 error handling
        //If there is 404 status code, the route path will become Error/404
        [Route("Error/{statusCode}")]

        public IActionResult HttpStatusCodeHandler(int statusCode)
        {

            //If you are using UseStatusCodePagesWithReExecute middleware, it's also possible to 
            // Retrieve the exception Details and get the original path in the ErrorController using
            // IStatusCodeReExecuteFeature interface as shown below:
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();


            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";

                    // Retrieve the exception Details
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QS = statusCodeResult.OriginalQueryString;


                    // LogWarning() method logs the message under
                    // Warning category in the log
                    ILogger_ErrorController_.LogWarning($" ops 404 error occured. Path = " +
                        $"{statusCodeResult.OriginalPath} and QueryString = " +
                        $"{statusCodeResult.OriginalQueryString}");

                    //Output:
                    // Books_Store.Controllers.ErrorController: Warning: ops 404 error occured. Path = / home / lol and QueryString = ? abc = xyz & 123 = 456


                    break;


            }


            return View("Centralised 404 error handling");
        }

        #endregion


        #region 2-Global Exception Handling

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            // Retrieve the exception Details
            IExceptionHandlerPathFeature exceptionHandlerPathFeature =
                    HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.Exception = exceptionHandlerPathFeature.Error;
            ViewBag.innerException = exceptionHandlerPathFeature.Error.InnerException?.Message;
            ViewBag.ExceptionPath = exceptionHandlerPathFeature.Path;
            ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;



            // LogError() method logs the exception under Error category in the log
            ILogger_ErrorController_.LogError($"ops The path {exceptionHandlerPathFeature.Path} " +
                $"threw an exception {exceptionHandlerPathFeature.Error}");
            //Output:
            /* Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware: Error: An unhandled exception has occurred while executing the request.

                 System.Exception: Error/Exception otherthan 404 errors
                    at Books_Store.Controllers.HomeController.Test9()
                    at lambda_method79(Closure , Object , Object[] )
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.SyncActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeActionMethodAsync()
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeNextActionFilterAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
                    at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
                    at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware.<Invoke>g__Awaited|6_0(ExceptionHandlerMiddleware middleware, HttpContext context, Task task)
                 Books_Store.Controllers.ErrorController: Error: ops The path /home/test9 threw an exception System.Exception: Error/Exception otherthan 404 errors
                    at Books_Store.Controllers.HomeController.Test9()
                    at lambda_method79(Closure , Object , Object[] )
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.SyncActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeActionMethodAsync()
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeNextActionFilterAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()
                 --- End of stack trace from previous location ---
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
                    at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
                    at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
                    at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware.<Invoke>g__Awaited|6_0(ExceptionHandlerMiddleware middleware, HttpContext context, Task task)
             */

            return View("Global Exception Handling");
        }

        #endregion



        // 3-test
        [AllowAnonymous]
        [Route("Error/test")]
        public void test()
        {

            ILogger_ErrorController_.LogTrace(" ops Trace Log");
            ILogger_ErrorController_.LogDebug(" ops Debug Log");
            ILogger_ErrorController_.LogInformation(" ops Information Log");
            ILogger_ErrorController_.LogWarning(" ops Warning Log");
            ILogger_ErrorController_.LogError(" ops Error Log");
            ILogger_ErrorController_.LogCritical(" ops Critical Log");
            /*
             2023-02-09 01:30:50.8532|TRACE|Books_Store.Controllers.ErrorController| ops Trace Log
            2023-02-09 01:30:50.8532|DEBUG|Books_Store.Controllers.ErrorController| ops Debug Log
            2023-02-09 01:30:50.8656|INFO|Books_Store.Controllers.ErrorController| ops Information Log
            2023-02-09 01:30:50.8656|WARN|Books_Store.Controllers.ErrorController| ops Warning Log
            2023-02-09 01:30:50.8656|ERROR|Books_Store.Controllers.ErrorController| ops Error Log
            2023-02-09 01:30:50.8656|FATAL|Books_Store.Controllers.ErrorController| ops Critical Log
                         */

        }

    }
}



