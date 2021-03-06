﻿using CERP.App;
using CERP.Attributes;
using CERP.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;

namespace CERP.HR.Documents
{
    public class Contact : AuditedAggregateTenantRoot<int>
    {
        public Contact()
        {

        }

        public DictionaryValue RelationshipType { get; set; }
        [CustomAudited]
        public Guid RelationshipTypeId { get; set; }

        [CustomAudited]
        public string Name { get; set; }
        [CustomAudited]
        public string PhoneNumber { get; set; }
        [CustomAudited]
        public string EmailAddress { get; set; }
        [CustomAudited]
        public string AlternatePhone { get; set; }

        [CustomAudited]
        public bool IsEmergencyContact { get; set; }

        [CustomAudited]
        public string AddressLine1 { get; set; }
        [CustomAudited]
        public string AddressLine2 { get; set; }

        [CustomAudited]
        public string City { get; set; }
        [CustomAudited]
        public string State { get; set; }
        [CustomAudited]
        public string PostalCode { get; set; }

        public DictionaryValue Country { get; set; }
        [CustomAudited]
        public Guid CountryId { get; set; }

        [CustomAudited]
        public bool IsPrimary { get; set; }
    }
}
