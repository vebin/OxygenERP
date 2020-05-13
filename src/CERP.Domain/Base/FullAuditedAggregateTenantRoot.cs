﻿using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace CERP.Base
{
    public class FullAuditedAggregateTenantRoot<TKey> : FullAuditedAggregateRoot<TKey>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
    }
}
