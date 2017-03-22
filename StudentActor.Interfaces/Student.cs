using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StudentActor.Interfaces
{
    [DataContract]
    public class Student
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Address Address { get; set; }

        [DataMember]
        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string City { get; set; }

    }
    
}