using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.CQRS;

namespace StudentActor.Interfaces
{
    public class RegisterCommand : CommandBase
    {
        public string Name { get; set; }
    }
}
