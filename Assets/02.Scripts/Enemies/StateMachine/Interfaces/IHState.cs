using RobustFSM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobustFSM.Interfaces
{
    public interface IHState : IState
    {
        public IFSM Machine { get; set; }
    }
}
