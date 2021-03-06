#pragma warning disable CS8618 // Non-nullable field is uninitialized. The pattern is intentional for an event-sourced domain object.
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NosAyudamos
{
    abstract class Person : DomainObject, IIdentifiable
    {
        public Person(IEnumerable<DomainEvent> history)
            : this() => Load(history);

        protected internal Person(
            string personId,
            string firstName,
            string lastName,
            string phoneNumber,
            Role role = Role.Donee,
            DateTime? dateOfBirth = default,
            Sex? sex = default)
            : this()
        {
            IsReadOnly = false;
            Raise(new PersonRegistered(personId, firstName, lastName, phoneNumber, role, dateOfBirth, sex));
        }

        protected internal Person()
        {
            Handles<PersonRegistered>(OnRegistered);
            Handles<PhoneNumberUpdated>(OnPhoneNumberUpdated);
            Handles<TaxStatusApproved>(OnTaxStatusApproved);
            Handles<TaxStatusRejected>(OnTaxStatusRejected);
            Handles<TaxStatusValidated>(OnTaxStatusValidated);
        }

        // NOTE: the [JsonProperty] attributes allow the deserialization from 
        // JSON to be able to set the properties when loading from the last  
        // saved known snapshot state.

        string IIdentifiable.Id => PersonId;

        [JsonProperty]
        public string PersonId { get; private set; }

        [JsonProperty]
        public string FirstName { get; private set; }

        [JsonProperty]
        public string LastName { get; private set; }

        [JsonProperty]
        public string PhoneNumber { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; } = Role.Donee;

        [JsonProperty]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? DateOfBirth { get; private set; }

        [JsonIgnore]
        public int? Age => (DateTime.Now - DateOfBirth)?.Days / 365;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Sex? Sex { get; private set; }

        [JsonProperty]
        public int State { get; private set; } = 0;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public TaxStatus TaxStatus { get; private set; } = TaxStatus.Unknown;

        /// <summary>
        /// Whether the user has been validated for operating in the platform.
        /// </summary>
        public bool IsValidated => TaxStatus == TaxStatus.Approved || TaxStatus == TaxStatus.Validated;

        public void UpdatePhoneNumber(string phoneNumber)
        {
            if (PhoneNumber == phoneNumber)
                return;

            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

            Raise(new PhoneNumberUpdated(PhoneNumber, phoneNumber));
        }

        public bool CanUpdateTaxStatus(TaxId taxId)
            => taxId != TaxId.Unknown &&
            (taxId.Kind == TaxIdKind.CUIL ||
            taxId.HasIncomeTax == true ||
            (taxId.Category != TaxCategory.Unknown && taxId.Category != TaxCategory.A));

        /// <summary>
        /// Marks the user as approved, regardless of its current tax category and kind.
        /// </summary>
        public void ApproveTaxStatus(string approver)
        {
            if (string.IsNullOrEmpty(approver))
                throw new ArgumentException("Approver cannot be empty.", nameof(approver));

            if (TaxStatus != TaxStatus.Validated)
                Raise(new TaxStatusApproved(approver));
        }

        /// <summary>
        /// Tries to validate the tax status given the tax information.
        /// Returns whether the information was sufficent to determine 
        /// the final status.
        /// </summary>
        public void UpdateTaxStatus(TaxId taxId)
        {
            if (taxId == TaxId.Unknown)
                return;

            if (taxId == TaxId.None || taxId.Kind == TaxIdKind.CUIL)
            {
                // We just accept CUIL-based registrations, we can't know whether 
                // they pay earnings or not :(
                Raise(new TaxStatusValidated(taxId));
                return;
            }

            if (taxId.HasIncomeTax == true)
            {
                Raise(new TaxStatusRejected(taxId, TaxStatusRejectedReason.HasIncomeTax));
                return;
            }

            if (taxId.Category == TaxCategory.NotApplicable)
            {
                Raise(new TaxStatusRejected(taxId, TaxStatusRejectedReason.NotApplicable));
                return;
            }

            if (taxId.Category != TaxCategory.Unknown &&
                taxId.Category != TaxCategory.A)
            {
                Raise(new TaxStatusRejected(taxId, TaxStatusRejectedReason.HighCategory));
                return;
            }

            if (taxId.Category == TaxCategory.A)
            {
                Raise(new TaxStatusValidated(taxId));
                return;
            }

            // Other combinations might not be approved
        }

        void OnRegistered(PersonRegistered e)
            => (PersonId, FirstName, LastName, PhoneNumber, Role, DateOfBirth, Sex)
            = (e.PersonId, e.FirstName, e.LastName, e.PhoneNumber, e.Role, e.DateOfBirth, e.Sex);

        void OnPhoneNumberUpdated(PhoneNumberUpdated e) => PhoneNumber = e.NewNumber;

        void OnTaxStatusApproved(TaxStatusApproved e) => TaxStatus = TaxStatus.Approved;

        void OnTaxStatusRejected(TaxStatusRejected e) => TaxStatus = TaxStatus.Rejected;

        void OnTaxStatusValidated(TaxStatusValidated e) => TaxStatus = TaxStatus.Validated;
    }
}
