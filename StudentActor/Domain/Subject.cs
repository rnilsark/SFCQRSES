using Common.DDD;

namespace StudentActor.Domain
{
    public class Subject : ValueObject<Subject>
    {
        public string Name { get; }
        public Level Level { get; }

        private Subject(string name, Level level)
        {
            Name = name;
            Level = level;
        }

        public static Subject Create(string name, Level level)
        {
            return new Subject(name, level);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}