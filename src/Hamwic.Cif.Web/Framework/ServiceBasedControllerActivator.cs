using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Hamwic.Cif.Web.Framework
{
    public class ServiceBasedControllerActivator : IControllerActivator  
    {
        private readonly IIocUtilities _iocUtilities;

        #region Constructor
        /// <summary>
        /// Constructor for the ServiceBasedControllerActivator
        /// which injects the properties to the ControllerBase class
        /// </summary>
        /// <param name="iocUtilities"></param>
        public ServiceBasedControllerActivator(IIocUtilities iocUtilities)
        {
            _iocUtilities = iocUtilities;
        }

        #endregion

        /// <summary>
        /// This method is called each team a controller is created and therefore populates any
        /// public settable properties on the controller
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public object Create(ControllerContext actionContext)
        {
            var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();

            var controller = actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
            _iocUtilities.InjectProperties(controller);
            return controller;
        }

        public virtual void Release(ControllerContext context, object controller)
        {
        }
    }
}