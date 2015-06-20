using System;
using System.Collections.Generic;

namespace CodeVision.Model
{
    public class Filter
    {
        public string Field { get; private set; }
        public List<string> Terms { get; private set; }

        public Filter(string field, string term)
            : this(field, new List<string> { term })
        {
        }

        public Filter(string field, List<string> terms)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new NullReferenceException("Must have field");
            }

            if (terms == null)
            {
                throw new NullReferenceException("Must have terms");
            }

            Field = field;
            Terms = terms;
        }
    }
}
