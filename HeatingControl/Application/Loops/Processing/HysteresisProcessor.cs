namespace HeatingControl.Application.Loops.Processing
{
    public interface IHysteresisProcessor
    {
        bool Process(float currentTemperature, bool currentState, float setPoint, float hysteresis);
    }

    public class HysteresisProcessor : IHysteresisProcessor
    {
        public bool Process(float currentTemperature, bool currentState, float setPoint, float hysteresis)
        {
            // TODO: can handle both heating and cooling approach

            var halfOfHysteresis = hysteresis / 2f;

            if (currentTemperature >= setPoint + halfOfHysteresis)
            {
                return false;
            }

            if (currentTemperature <= setPoint - halfOfHysteresis)
            {
                return true;
            }

            return currentState;
        }
    }
}
