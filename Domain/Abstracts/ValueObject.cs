using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNnetcoreCleanArchitecture.Domain.Abstracts
{
    public abstract record ValueObject
    {
        public object WithClone() => this with { };
    }
}
