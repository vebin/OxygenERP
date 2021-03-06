﻿using CERP.ApplicationContracts.HR.OrganizationalManagement;
using CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure;
using CERP.ApplicationContracts.HR.OrganizationalManagement.PayrollStructure;
using CERP.HR.EMPLOYEE.DTOs;
using CERP.HR.OrganizationalManagement;
using CERP.HR.OrganizationalManagement.OrganizationStructure;
using CERP.HR.OrganizationalManagement.PayrollStructure;
using CERP.HR.Timesheets;
using CERP.HR.Workshifts;
using CERP.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CERP.AppServices.HR.OrganizationalManagement.PayrollStructure
{
    public class PS_PayComponentAppService : CrudAppService<PS_PayComponent, PS_PayComponent_Dto, int, PagedAndSortedResultRequestDto, PS_PayComponent_Dto, PS_PayComponent_Dto>
    {
        public PS_PayComponentAppService(IRepository<PS_PayComponent, int> repository, IRepository<Currency, int> currencyRepository, IRepository<PS_PayComponentType, int> componentTypeRepository, IRepository<PS_PayFrequency, int> frequencyRepository) : base(repository)
        {
            Repository = repository;
            CurrencyRepository = currencyRepository;
            ComponentTypeRepository = componentTypeRepository;
            FrequencyRepository = frequencyRepository;
        }

        public IRepository<PS_PayComponent, int> Repository { get; }
        public IRepository<Currency, int> CurrencyRepository { get; }
        public IRepository<PS_PayComponentType, int> ComponentTypeRepository { get; }
        public IRepository<PS_PayFrequency, int> FrequencyRepository { get; }

        public async Task<List<PS_PayComponent_Dto>> GetAllComponentsAsync()
        {
            List<PS_PayComponent_Dto> list = (await Repository.GetListAsync(true)).Select(MapToGetListOutputDto).ToList();
            return list;
        }
        public List<PS_PayComponent_Dto> GetAllByPayComponentTypeAsync(int typeId)
        {
            List<PS_PayComponent_Dto> list = (Repository.WithDetails().Where(x => x.PayComponentTypeId == typeId).ToList()).Select(MapToGetListOutputDto).ToList();
            return list;
        }
        public async Task<PS_PayComponent_Dto> GetComponentAsync(int id)
        {
            PS_PayComponent_Dto obj = ObjectMapper.Map<PS_PayComponent, PS_PayComponent_Dto>(await Repository.GetAsync(id, true));
            return obj;
        }


        //public async Task<List<EntityReference>> GetAllReferences(int id)
        //{
        //    List<EntityReference> entityReferences = new List<EntityReference>();

        //    entityReferences.AddRange(PositionsReferenceRepo.WithDetails(x => x.PositionTemplate).Where(x => x.JobTemplateId == id)
        //        .ToList()
        //        .Select(x => new EntityReference() { Id = entityReferences.Count + 1, Name = x.PositionTemplate.Name, Code = x.PositionTemplate.Code, Type = "Position" }));

        //    return entityReferences;
        //}
    }
}
