using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class PredicateDto
    {
        public string Sql { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }
}
