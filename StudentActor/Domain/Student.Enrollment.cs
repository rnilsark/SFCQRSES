using StudentActor.Events;

namespace StudentActor.Domain
{
    public partial class Student
    {
        public class Enrollment : Entity<Student, IEnrollmentEvent>
        {
            public Enrollment(Student student, int enrollmentId) : base(student)
            {
                Id = enrollmentId;

                RegisterEventAppliers()
                    .For<IStudentEnrolledEvent>(e => Subject = e.Subject);
            }

            public int Id { get; set; }
            public Subject Subject { get; set; }
        }
    }
}