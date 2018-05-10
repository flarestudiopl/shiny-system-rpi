using System;
using Dapper;

namespace Storage.StorageDatabase.Counter
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
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public CounterAccumulator(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Accumulate(CounterAccumulatorInput input)
        {
            const string query = @"
INSERT OR REPLACE INTO [Counter] 
 ([CounterId], [HeaterId], [CountedSeconds], [StartDateTime]) 
VALUES 
 ((SELECT [CounterId] FROM [Counter] WHERE [HeaterId] = @HeaterId AND [EndDateTime] IS NULL),
  @HeaterId,
  @SecondsToAccumulate + COALESCE((SELECT [CountedSeconds] FROM [Counter] WHERE [HeaterId] = @HeaterId AND [EndDateTime] IS NULL), 0),
  COALESCE((SELECT [StartDateTime] FROM [Counter] WHERE [HeaterId] = @HeaterId AND [EndDateTime] IS NULL), @StartDateTime));";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query, new
                                          {
                                              input.HeaterId,
                                              input.SecondsToAccumulate,
                                              StartDateTime = DateTime.Now.Ticks
                                          });
            }
        }
    }
}
