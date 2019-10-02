using System;

namespace HeatingControl.Application.DataAccess.Counter
{
    public interface ICounterAccumulator
    {
        void Accumulate(CounterAccumulatorInput input);
    }

    public class CounterAccumulatorInput
    {
        public int HeaterId { get; set; }
        public int SecondsToAccumulate { get; set; }
    }

    public class CounterAccumulator : ICounterAccumulator
    {
        private readonly IRepository<Domain.Counter> _counterRepository;

        public CounterAccumulator(IRepository<Domain.Counter> counterRepository)
        {
            _counterRepository = counterRepository;
        }

        public void Accumulate(CounterAccumulatorInput input)
        {
            var existingCounter = _counterRepository.ReadSingleOrDefault(x => x.HeaterId == input.HeaterId && !x.ResetDate.HasValue);

            if (existingCounter != null)
            {
                existingCounter.CountedSeconds += input.SecondsToAccumulate;

                _counterRepository.Update(existingCounter);
            }
            else
            {
                var newCounter = new Domain.Counter
                {
                    CountedSeconds = input.SecondsToAccumulate,
                    HeaterId = input.HeaterId,
                    StartDate = DateTime.UtcNow
                };

                _counterRepository.Create(newCounter);
            }
        }
    }
}
