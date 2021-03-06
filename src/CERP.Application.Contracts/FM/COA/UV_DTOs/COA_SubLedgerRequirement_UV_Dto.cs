﻿using CERP.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace CERP.FM.COA.UV_DTOs
{
    public class COA_SubLedgerRequirement_UV_Dto : FullAuditedEntityTenantDto<Guid> 
    {
        public COA_SubLedgerRequirement_UV_Dto()
        {

        }
        public COA_SubLedgerRequirement_UV_Dto(Guid id)
        {
            Id = id;
        }

        [Required]
        public string Title { get; set; }
        [Required]
        public string TitleLocalizationKey { get; set; }

        public List<COA_SubLedgerRequirement_Account_UV_Dto> SubLedgerRequirementAccounts { get; set; }
    }
}
