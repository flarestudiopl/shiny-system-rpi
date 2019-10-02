using System;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Obsolete]
    [Route("/api/setup")]
    public class SetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;

        public SetupController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }

        [HttpGet]
        public Building GetBuildingModel()
        {
            return _heatingControl.State.Model;
        }
    }
}
