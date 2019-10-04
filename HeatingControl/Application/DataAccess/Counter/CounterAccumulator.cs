using Storage.StorageDatabase;
using System;
using System.Linq;

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
        private readonly IDbExecutor _dbExecutor;

        public CounterAccumulator(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Accumulate(CounterAccumulatorInput input)
        {
            _dbExecutor.Execute(c =>
            {
                var counter = c.Set<Domain.Counter>().SingleOrDefault(x => x.HeaterId == input.HeaterId && !x.ResetDate.HasValue);

                if (counter != null)
                {
                    counter.CountedSeconds += input.SecondsToAccumulate;
                }
                else
                {
                    var newCounter = new Domain.Counter
                    {
                        CountedSeconds = input.SecondsToAccumulate,
                        HeaterId = input.HeaterId,
                        StartDate = DateTime.UtcNow
                    };

                    c.Set<Domain.Counter>().Add(newCounter);
                }

                c.SaveChanges();
            });
        }
    }
}
