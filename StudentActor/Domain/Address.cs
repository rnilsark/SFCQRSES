using System;
using Common.DDD;

namespace StudentActor.Domain
{
    public class Address : ValueObject<Address>
    {
        public string Street { get; }
        public string ZipCode { get; }
        public string City { get; }

        private Address(string street, string zipCode, string city)
        {
            Street = street;
            ZipCode = zipCode;
            City = city;
        }

        public static Address Create(string street, string zipCode, string city)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new ArgumentException($"{nameof(street)} can not be empty.");
            }

            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentException($"{nameof(zipCode)} can not be empty.");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException($"{nameof(city)} can not be empty.");
            }

            return new Address(street, zipCode, city);
        }

        public override string ToString()
        {
            return $"{Street} {ZipCode} {City}";
        }
    }
}