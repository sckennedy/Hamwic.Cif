using System;
using System.IO;
using System.Threading.Tasks;
using Hamwic.Cif.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Hamwic.Cif.Core.Implementation.Services
{
    /// <summary>
    /// Class to render views to strings used when updating pages
    /// using ajax calls rather than refreshing the entire page
    /// </summary>
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HttpContext _httpContext;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(_httpContext, new RouteData(), new ActionDescriptor());

            var viewEngineResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (viewEngineResult.View == null || (!viewEngineResult.Success))
            {
                viewEngineResult = _razorViewEngine.GetView(viewName, viewName, false);

                if (viewEngineResult.View == null || (!viewEngineResult.Success))
                    throw new ArgumentNullException($"Unable to find view '{viewName}'");
            }

            var view = viewEngineResult.View;

            using (var sw = new StringWriter())
            {
                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary()) {Model = model};

                var tempData = new TempDataDictionary(_httpContext, _tempDataProvider);

                var viewContext = new ViewContext(actionContext, view, viewDictionary, tempData, sw,
                    new HtmlHelperOptions()) {RouteData = _httpContext.GetRouteData()};

                await view.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
