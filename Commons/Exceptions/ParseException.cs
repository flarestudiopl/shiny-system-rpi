using System;

namespace Commons.Exceptions
{
    public class ParseException : Exception
    {
        public string StringToParse { get; }

        public ParseException(Exception exception, string stringToParse)
            : base("Unable to parse string", exception)
        {
            StringToParse = stringToParse;
        }

        public override string Message => $"{base.Message}: {StringToParse}";
    }
}
