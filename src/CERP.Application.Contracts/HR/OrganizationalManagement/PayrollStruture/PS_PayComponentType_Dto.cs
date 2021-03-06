﻿using CERP.App;
using CERP.Attributes;
using CERP.Base;
using CERP.FM;

using CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure;
using CERP.HR.Setup.OrganizationalManagement.OrganizationStructure;
using CERP.HR.Setup.OrganizationalManagement.PayrollStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using CERP.App.Helpers;

namespace CERP.ApplicationContracts.HR.OrganizationalManagement.PayrollStructure
{
    public class PS_PayComponentType_Dto : AuditedEntityTenantDto<int>
    {
        public PS_PayComponentType_Dto()
        {
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameLocalized { get; set; }
        public string Description { get; set; }

        public double Amount { get; set; }
        public double Percentage { get; set; }

        public string ValueTypeDescription { get => EnumExtensions.GetDescription(ValueType); set => ValueType = EnumExtensions.GetValueFromDescription<PS_PayComponentTypeValueType>(value); }
        public PS_PayComponentTypeValueType ValueType { get; set; }

        public PS_PayComponentType_Dto? ValueComponentType { get; set; }
        public int? ValueComponentTypeId { get; set; }

    }
}
