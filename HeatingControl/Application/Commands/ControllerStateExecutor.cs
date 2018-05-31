using System;
using System.Collections.Generic;
using System.Text;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public interface IControllerStateExecutor
    {
        void Execute(bool state, ControllerState controllerState, Building model);
    }

    public class ControllerStateExecutor : IControllerStateExecutor
    {
        public void Execute(bool state, ControllerState controllerState, Building model)
        {
            if (state)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        private void TurnOn()
        {

        }

        private void TurnOff()
        {

        }
    }
}
