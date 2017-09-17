using System;

namespace NarrowIM.Common
{
    public class Candidate
    {
        private string _word;
        public string Word {
            get { return _word; }
            set {
                _word = value;
                if (string.IsNullOrEmpty(Abbr))
                {
                    Abbr = value;
                }
            }
        }

        public string Abbr        { get; set; }

        public string Description { get; set; }

        public object Value       { get; set; }

        public Func<object, bool> Func { get; set; }

        public bool Invoke()
        {
            return Func(Value);
        }
    }
}
