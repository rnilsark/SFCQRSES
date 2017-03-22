using StudentActor.Domain;

namespace StudentActor.Mapping
{
    public static class InterfaceToDomainModelMapper
    {
        public static Address ToDomainModel(this Interfaces.Address @this)
        {
            return Address.Create(@this.Street, @this.ZipCode, @this.City);
        }

        public static Subject ToDomainModel(this Interfaces.Subject @this)
        {
            return (Subject) @this;
        }
    }
}