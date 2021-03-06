﻿using CERP.App;
using CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure;
using CERP.Attributes;
using CERP.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure
{
    public class OS_JobQualificationTemplate_Dto : AuditedEntityTenantDto<int>
    {
        public OS_JobQualificationTemplate_Dto()
        {
        }

        public OS_JobTemplate_Dto JobTemplate { get; set; }
        public int JobTemplateId { get; set; }

        public DictionaryValue_Dto Degree { get; set; }
        public Guid DegreeId { get; set; }

        public DictionaryValue_Dto Institute { get; set; }
        public Guid InstituteId { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}
