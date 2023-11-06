using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Bus.Interfaces
{
    public interface IBaseEventHandler
    {
        public Task Handle(object payload);
    }
}
