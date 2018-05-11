using System;
using Dapper;

namespace Storage.StorageDatabase.Counter
{
    public interface ICounterResetter
    {
        void Reset(CounterResetterInput input);
    }

    public class CounterResetterInput
    {
        public int HeaterId { get; set; }
        public int UserId { get; set; }
    }

    public class CounterResetter : ICounterResetter
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public CounterResetter(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Reset(CounterResetterInput input)
        {
            const string query = @"
UPDATE [Counter]
SET [ResetDateTime] = @ResetDateTime,
    [ResettedBy] = @UserId
WHERE [HeaterId] = @HeaterId AND 
	  [ResetDateTime] IS NULL";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query, new
                                          {
                                              input.HeaterId,
                                              input.UserId,
                                              ResetDateTime = DateTime.Now.Ticks
                                          });
            }
        }
    }
}
