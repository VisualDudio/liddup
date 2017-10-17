using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Couchbase.Lite;

namespace Liddup
{
    public interface IListener
    {
        void Start(Manager manager, ushort port);
    }
}
