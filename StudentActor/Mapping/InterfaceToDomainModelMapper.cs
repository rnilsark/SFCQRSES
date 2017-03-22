using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentActor.Domain;

namespace StudentActor.Mapping
{
    public static class InterfaceToDomainModelMapper
    {
        public static Address ToDomainModel(this Interfaces.Address @this)
        {
            return Address.Create(@this.Street, @this.ZipCode, @this.City);
        }
    }
}
