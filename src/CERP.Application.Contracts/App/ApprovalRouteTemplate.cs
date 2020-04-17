﻿using CERP.Base;
using CERP.HR.Employees;
using CERP.Setup;
using System;
using System.Collections.Generic;
using System.Text;

namespace CERP.App
{
    public class ApprovalRouteTemplate_Dto : AuditedEntityTenantDto<int>
    {
        public ApprovalRouteTemplate_Dto()
        {

        }
        public ApprovalRouteTemplate_Dto(int id)
        {
            Id = id;
        }

        public ApprovalRouteModule ApprovalRouteModule { get; set; }

        public List<ApprovalRouteTemplateItem_Dto> ApprovalRouteTemplateItems { get; set; }
    }
}