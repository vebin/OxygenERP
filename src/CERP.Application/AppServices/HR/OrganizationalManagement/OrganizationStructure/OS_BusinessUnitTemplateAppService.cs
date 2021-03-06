﻿using CERP.ApplicationContracts.HR.OrganizationalManagement;
using CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure;
using CERP.HR.EMPLOYEE.DTOs;
using CERP.HR.OrganizationalManagement;
using CERP.HR.OrganizationalManagement.OrganizationStructure;
using CERP.HR.Timesheets;
using CERP.HR.Workshifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CERP.AppServices.HR.OrganizationalManagement.OrganizationStructure
{
    public class OS_BusinessUnitTemplateAppService : CrudAppService<OS_BusinessUnitTemplate, OS_BusinessUnitTemplate_Dto, int, PagedAndSortedResultRequestDto, OS_BusinessUnitTemplate_Dto, OS_BusinessUnitTemplate_Dto>
    {
        public IRepository<OS_BusinessUnitTemplate, int> Repository { get; }
        public IRepository<OS_OrganizationStructureTemplate> OrganizationStructureRepository { get; }

        public OS_BusinessUnitTemplateAppService(IRepository<OS_BusinessUnitTemplate, int> repository, IRepository<OS_OrganizationStructureTemplate> organizationStructureRepository) : base(repository)
        {
            Repository = repository;
            OrganizationStructureRepository = organizationStructureRepository;
        }

        public async Task<List<OS_BusinessUnitTemplate_Dto>> GetAllBusinessUnitTemplatesAsync()
        {
            List<OS_BusinessUnitTemplate_Dto> list = (await Repository.GetListAsync(true)).Select(MapToGetListOutputDto).ToList();
            return list;
        }

        public async Task<List<EntityReference>> GetAllReferences(int id)
        {
            List<EntityReference> entityReferences = new List<EntityReference>();

            entityReferences.AddRange(OrganizationStructureRepository.WithDetails(x => x.OrganizationStructureTemplateBusinessUnits).Where(x => x.OrganizationStructureTemplateBusinessUnits.Any(x => x.BusinessUnitTemplateId == id))
                .ToList()
                .Select(x => new EntityReference() { Id = entityReferences.Count + 1, Name = x.Name, Code = x.Code, Type = "Organization Structure" }));

            //entityReferences.AddRange(TasksReferenceRepo.WithDetails(x => x.TaskTemplate).Where(x => x.BusinessUnitTemplateId == id)
            //    .ToList()
            //    .Select(x => new EntityReference() { Id = entityReferences.Count + 1, Name = x.TaskTemplate.Name, Code = x.TaskTemplate.Code, Type = "Task" }));

            //entityReferences.AddRange(JobsReferenceRepo.WithDetails(x => x.JobTemplate).Where(x => x.BusinessUnitTemplateId == id)
            //    .ToList()
            //    .Select(x => new EntityReference() { Id = entityReferences.Count + 1, Name = x.JobTemplate.Name, Code = x.JobTemplate.Code, Type = "Job" }));

            return entityReferences;
        }
    }
}
