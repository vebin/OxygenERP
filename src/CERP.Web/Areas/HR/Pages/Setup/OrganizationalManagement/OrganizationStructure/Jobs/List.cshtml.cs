using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CERP.App;
using CERP.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Syncfusion.EJ2.Grids;
using Volo.Abp.AuditLogging;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.Auditing;
using CERP.CERP.HR.Documents;
using CERP.App.CustomEntityHistorySystem;
using CERP.Attributes;
using CERP.ApplicationContracts.HR.OrganizationalManagement.OrganizationStructure;
using CERP.HR.OrganizationalManagement.OrganizationStructure;
using CERP.AppServices.HR.OrganizationalManagement.OrganizationStructure;

namespace CERP.Web.Areas.HR.Setup.OrganizationalManagement.OrganizationStructure.Pages.Jobs
{
    public class ListModel : CERPPageModel
    {
        public IWebHostEnvironment webHostEnvironment { get; set; }
        public IRepository<DictionaryValue, Guid> DictionaryValuesRepo { get; set; }
        public IAuditLogRepository AuditLogsRepo { get; set; }

        public OS_JobTemplateAppService OS_JobTemplateAppService { get; set; }

        public IAuditingManager AuditingManager { get; set; }
        public IRepository<CustomEntityChange> CustomEntityChangesRepo { get; set; }

        public ListModel(IJsonSerializer jsonSerializer, IRepository<DictionaryValue, Guid> dictionaryValuesRepo, IWebHostEnvironment webHostEnvironment, IAuditLogRepository auditLogsRepo, OS_JobTemplateAppService oS_JobTemplateAppService)
        {
            JsonSerializer = jsonSerializer;
            DictionaryValuesRepo = dictionaryValuesRepo;
            this.webHostEnvironment = webHostEnvironment;
            AuditLogsRepo = auditLogsRepo;
            OS_JobTemplateAppService = oS_JobTemplateAppService;
        }

        public IJsonSerializer JsonSerializer { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostJobTemplate()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var FormData = Request.Form;

                    OS_JobTemplate_Dto jobTemplate_Dto = JsonSerializer.Deserialize<OS_JobTemplate_Dto>(FormData["info"]);

                    bool IsEditing = jobTemplate_Dto.Id > 0;
                    if (IsEditing)
                    {
                        OS_JobTemplate curJobTemplate = await OS_JobTemplateAppService.Repository.GetAsync(jobTemplate_Dto.Id);

                        if (AuditingManager.Current != null)
                        {
                            EntityChangeInfo entityChangeInfo = new EntityChangeInfo();

                            entityChangeInfo.EntityId = jobTemplate_Dto.Id.ToString();
                            entityChangeInfo.EntityTenantId = jobTemplate_Dto.TenantId;
                            entityChangeInfo.ChangeTime = DateTime.Now;
                            entityChangeInfo.ChangeType = EntityChangeType.Updated;
                            entityChangeInfo.EntityTypeFullName = typeof(OS_JobTemplate).FullName;

                            entityChangeInfo.PropertyChanges = new List<EntityPropertyChangeInfo>();
                            List<EntityPropertyChangeInfo> entityPropertyChanges = new List<EntityPropertyChangeInfo>();
                            var auditProps = typeof(OS_JobTemplate).GetProperties().Where(x => Attribute.IsDefined(x, typeof(CustomAuditedAttribute))).ToList();

                            OS_JobTemplate mappedInput = ObjectMapper.Map<OS_JobTemplate_Dto, OS_JobTemplate>(jobTemplate_Dto);
                            foreach (var prop in auditProps)
                            {
                                EntityPropertyChangeInfo propertyChange = new EntityPropertyChangeInfo();
                                object origVal = prop.GetValue(curJobTemplate);
                                propertyChange.OriginalValue = origVal == null ? "" : origVal.ToString();
                                object newVal = prop.GetValue(mappedInput);
                                propertyChange.NewValue = newVal == null ? "" : newVal.ToString();
                                if (propertyChange.OriginalValue == propertyChange.NewValue) continue;

                                propertyChange.PropertyName = prop.Name;

                                if (prop.Name.EndsWith("Id"))
                                {
                                    try
                                    {
                                        string valuePropName = prop.Name.TrimEnd('d', 'I');
                                        propertyChange.PropertyName = valuePropName;

                                        var valueProp = typeof(OS_JobTemplate).GetProperty(valuePropName);

                                        DictionaryValue _origValObj = (DictionaryValue)valueProp.GetValue(jobTemplate_Dto);
                                        if (_origValObj == null) _origValObj = await DictionaryValuesRepo.GetAsync((Guid)origVal);
                                        string _origVal = _origValObj.Value;
                                        propertyChange.OriginalValue = origVal == null ? "" : _origVal;
                                        DictionaryValue _newValObj = (DictionaryValue)valueProp.GetValue(mappedInput);
                                        if (_newValObj == null) _newValObj = await DictionaryValuesRepo.GetAsync((Guid)newVal);
                                        string _newVal = _newValObj.Value;
                                        propertyChange.NewValue = _newValObj == null ? "" : _newVal;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                propertyChange.PropertyTypeFullName = prop.Name.GetType().FullName;

                                entityChangeInfo.PropertyChanges.Add(propertyChange);
                            }

                            #region ExtraProperties
                            //List<EmployeeExtraPropertyHistory> allExtraPropertyHistories = new List<EmployeeExtraPropertyHistory>();
                            //if (departmentTemplate_Dto.ExtraProperties != curJobTemplate.ExtraProperties)
                            //{
                            //    //GeneralInfo oldGeneralInfo = department.GetProperty<GeneralInfo>("generalInfo");
                            //    //List<EmployeeExtraPropertyHistory> physicalIdsHistory = new List<EmployeeExtraPropertyHistory>();
                            //    //var generalInfoPhysicalIdAuditProps = typeof(PhysicalID).GetProperties().Where(x => Attribute.IsDefined(x, typeof(CustomAuditedAttribute))).ToList();
                            //    //List<PhysicalId<Guid>> NewPhysicalIds = generalInfo.PhysicalIds.Where(x => !oldGeneralInfo.PhysicalIds.Any(y => y.Id == x.Id)).ToList();
                            //    //List<PhysicalId<Guid>> UpdatedPhysicalIds = generalInfo.PhysicalIds.Where(x => oldGeneralInfo.PhysicalIds.Any(y => y.Id == x.Id)).ToList();
                            //    //List<PhysicalId<Guid>> DeletedPhysicalIds = oldGeneralInfo.PhysicalIds.Where(x => !generalInfo.PhysicalIds.Any(y => y.Id == x.Id)).ToList();
                            //    //for (int i = 0; i < NewPhysicalIds.Count; i++)
                            //    //{
                            //    //    PhysicalId<Guid> curPhId = generalInfo.PhysicalIds[i];

                            //    //    EmployeeExtraPropertyHistory newPhIdHistory = new EmployeeExtraPropertyHistory(2, "Physical Id", curPhId.IDNumber, "Created");
                            //    //    physicalIdsHistory.Add(newPhIdHistory);
                            //    //}
                            //    //for (int i = 0; i < UpdatedPhysicalIds.Count; i++)
                            //    //{
                            //    //    PhysicalId<Guid> curPhId = generalInfo.PhysicalIds[i];
                            //    //    PhysicalId<Guid> oldPhId = oldGeneralInfo.PhysicalIds.First(x => x.Id == curPhId.Id);

                            //    //    EmployeeExtraPropertyHistory updatedPhIdHistory = new EmployeeExtraPropertyHistory(2, "Physical Id", curPhId.IDNumber, "Updated");
                            //    //    foreach (var prop in generalInfoPhysicalIdAuditProps)
                            //    //    {
                            //    //        updatedPhIdHistory.PropertyChanges = new List<EmployeeTypePropertyChange>();

                            //    //        EmployeeTypePropertyChange propertyChange = new EmployeeTypePropertyChange();

                            //    //        object origVal = prop.GetValue(oldPhId);
                            //    //        propertyChange.OriginalValue = origVal == null ? "" : origVal.ToString();
                            //    //        object newVal = prop.GetValue(curPhId);
                            //    //        propertyChange.NewValue = newVal == null ? "" : newVal.ToString();
                            //    //        if (propertyChange.OriginalValue == propertyChange.NewValue) continue;

                            //    //        propertyChange.PropertyName = prop.Name;

                            //    //        if (prop.Name.EndsWith("Id"))
                            //    //        {
                            //    //            try
                            //    //            {
                            //    //                string valuePropName = prop.Name.TrimEnd('d', 'I');
                            //    //                propertyChange.PropertyName = valuePropName;

                            //    //                var valueProp = typeof(PhysicalID).GetProperty(valuePropName);

                            //    //                DictionaryValue _origValObj = (DictionaryValue)valueProp.GetValue(oldPhId);
                            //    //                if (_origValObj == null) _origValObj = await DictionaryValuesRepo.GetAsync((Guid)origVal);
                            //    //                string _origVal = _origValObj.Value;
                            //    //                propertyChange.OriginalValue = origVal == null ? "" : _origVal;
                            //    //                DictionaryValue _newValObj = (DictionaryValue)valueProp.GetValue(curPhId);
                            //    //                if (_newValObj == null) _newValObj = await DictionaryValuesRepo.GetAsync((Guid)newVal);
                            //    //                string _newVal = _newValObj.Value;
                            //    //                propertyChange.NewValue = _newValObj == null ? "" : _newVal;
                            //    //            }
                            //    //            catch (Exception ex)
                            //    //            {

                            //    //            }
                            //    //        }

                            //    //        propertyChange.PropertyTypeFullName = prop.Name.GetType().FullName;

                            //    //        updatedPhIdHistory.PropertyChanges.Add(propertyChange);
                            //    //    }
                            //    //    physicalIdsHistory.Add(updatedPhIdHistory);
                            //    //}
                            //    //for (int i = 0; i < DeletedPhysicalIds.Count; i++)
                            //    //{
                            //    //    PhysicalId<Guid> curPhId = generalInfo.PhysicalIds[i];

                            //    //    EmployeeExtraPropertyHistory deletedPhIdHistory = new EmployeeExtraPropertyHistory(2, "Physical Id", curPhId.IDNumber, "Deleted");
                            //    //    physicalIdsHistory.Add(deletedPhIdHistory);
                            //    //}

                            //    entityChangeInfo.SetProperty("extraPropertiesHistory", allExtraPropertyHistories);
                            //}
                            #endregion

                            AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                        }

                        curJobTemplate.Name = jobTemplate_Dto.Name;
                        curJobTemplate.NameLocalized = jobTemplate_Dto.NameLocalized;
                        curJobTemplate.Code = jobTemplate_Dto.Code;
                        curJobTemplate.ValidityFromDate = jobTemplate_Dto.ValidityFromDate;
                        curJobTemplate.ValidityToDate = jobTemplate_Dto.ValidityToDate;
                        curJobTemplate.Description = jobTemplate_Dto.Description;

                        OS_JobTemplate_Dto updated = ObjectMapper.Map<OS_JobTemplate, OS_JobTemplate_Dto>(await OS_JobTemplateAppService.Repository.UpdateAsync(curJobTemplate));

                        return StatusCode(200, updated);
                    }
                    else
                    {
                        jobTemplate_Dto.Id = 0;

                        OS_JobTemplate_Dto added = await OS_JobTemplateAppService.CreateAsync(jobTemplate_Dto);

                        if (AuditingManager.Current != null)
                        {
                            EntityChangeInfo entityChangeInfo = new EntityChangeInfo();
                            entityChangeInfo.EntityId = added.Id.ToString();
                            entityChangeInfo.EntityTenantId = added.TenantId;
                            entityChangeInfo.ChangeTime = DateTime.Now;
                            entityChangeInfo.ChangeType = EntityChangeType.Created;
                            entityChangeInfo.EntityTypeFullName = typeof(OS_JobTemplate).FullName;

                            AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                        }

                        return StatusCode(200, added);
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }

            return StatusCode(500);
        }
        public async Task<IActionResult> OnDeleteJobTemplate()
        {
            List<OS_JobTemplate_Dto> entitites = JsonSerializer.Deserialize<List<OS_JobTemplate_Dto>>(Request.Form["jobs"]);
            try
            {
                for (int i = 0; i < entitites.Count; i++)
                {
                    OS_JobTemplate_Dto entity = entitites[i];
                    //await TaskTemplatesAppService.Repository.DeleteAsync(leaveRequest.);
                    await OS_JobTemplateAppService.Repository.DeleteAsync(entity.Id);

                    if (AuditingManager.Current != null)
                    {
                        EntityChangeInfo entityChangeInfo = new EntityChangeInfo();
                        entityChangeInfo.EntityId = entity.Id.ToString();
                        entityChangeInfo.EntityTenantId = entity.TenantId;
                        entityChangeInfo.ChangeTime = DateTime.Now;
                        entityChangeInfo.ChangeType = EntityChangeType.Deleted;
                        entityChangeInfo.EntityTypeFullName = typeof(OS_JobTemplate).FullName;

                        AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> OnPostJobQualificationTemplate()
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var FormData = Request.Form;

                    OS_JobQualificationTemplate_Dto jobQualificationTemplate_Dto = JsonSerializer.Deserialize<OS_JobQualificationTemplate_Dto>(FormData["info"]);

                    bool IsEditing = jobQualificationTemplate_Dto.Id > 0;
                    if (IsEditing)
                    {
                        OS_JobQualificationTemplate curJobQualificationTemplate = await OS_JobTemplateAppService.QualificationsRepository.GetAsync(jobQualificationTemplate_Dto.Id);

                        if (AuditingManager.Current != null)
                        {
                            EntityChangeInfo entityChangeInfo = new EntityChangeInfo();

                            entityChangeInfo.EntityId = jobQualificationTemplate_Dto.Id.ToString();
                            entityChangeInfo.EntityTenantId = jobQualificationTemplate_Dto.TenantId;
                            entityChangeInfo.ChangeTime = DateTime.Now;
                            entityChangeInfo.ChangeType = EntityChangeType.Updated;
                            entityChangeInfo.EntityTypeFullName = typeof(OS_JobQualificationTemplate).FullName;

                            entityChangeInfo.PropertyChanges = new List<EntityPropertyChangeInfo>();
                            List<EntityPropertyChangeInfo> entityPropertyChanges = new List<EntityPropertyChangeInfo>();
                            var auditProps = typeof(OS_JobQualificationTemplate).GetProperties().Where(x => Attribute.IsDefined(x, typeof(CustomAuditedAttribute))).ToList();

                            OS_JobQualificationTemplate mappedInput = ObjectMapper.Map<OS_JobQualificationTemplate_Dto, OS_JobQualificationTemplate>(jobQualificationTemplate_Dto);
                            foreach (var prop in auditProps)
                            {
                                EntityPropertyChangeInfo propertyChange = new EntityPropertyChangeInfo();
                                object origVal = prop.GetValue(curJobQualificationTemplate);
                                propertyChange.OriginalValue = origVal == null ? "" : origVal.ToString();
                                object newVal = prop.GetValue(mappedInput);
                                propertyChange.NewValue = newVal == null ? "" : newVal.ToString();
                                if (propertyChange.OriginalValue == propertyChange.NewValue) continue;

                                propertyChange.PropertyName = prop.Name;

                                if (prop.Name.EndsWith("Id"))
                                {
                                    try
                                    {
                                        string valuePropName = prop.Name.TrimEnd('d', 'I');
                                        propertyChange.PropertyName = valuePropName;

                                        var valueProp = typeof(OS_JobQualificationTemplate).GetProperty(valuePropName);

                                        DictionaryValue _origValObj = (DictionaryValue)valueProp.GetValue(jobQualificationTemplate_Dto);
                                        if (_origValObj == null) _origValObj = await DictionaryValuesRepo.GetAsync((Guid)origVal);
                                        string _origVal = _origValObj.Value;
                                        propertyChange.OriginalValue = origVal == null ? "" : _origVal;
                                        DictionaryValue _newValObj = (DictionaryValue)valueProp.GetValue(mappedInput);
                                        if (_newValObj == null) _newValObj = await DictionaryValuesRepo.GetAsync((Guid)newVal);
                                        string _newVal = _newValObj.Value;
                                        propertyChange.NewValue = _newValObj == null ? "" : _newVal;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                propertyChange.PropertyTypeFullName = prop.Name.GetType().FullName;

                                entityChangeInfo.PropertyChanges.Add(propertyChange);
                            }

                            AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                        }

                        curJobQualificationTemplate.DegreeId = jobQualificationTemplate_Dto.DegreeId;
                        curJobQualificationTemplate.InstituteId = jobQualificationTemplate_Dto.InstituteId;
                        curJobQualificationTemplate.PeriodStartDate = jobQualificationTemplate_Dto.PeriodStartDate;
                        curJobQualificationTemplate.PeriodEndDate = jobQualificationTemplate_Dto.PeriodEndDate;

                        OS_JobQualificationTemplate_Dto updated = ObjectMapper.Map<OS_JobQualificationTemplate, OS_JobQualificationTemplate_Dto>(await OS_JobTemplateAppService.QualificationsRepository.UpdateAsync(curJobQualificationTemplate));
                        updated = ObjectMapper.Map<OS_JobQualificationTemplate, OS_JobQualificationTemplate_Dto>(await OS_JobTemplateAppService.QualificationsRepository.GetAsync(updated.Id));

                        return StatusCode(200, updated);
                    }
                    else
                    {
                        jobQualificationTemplate_Dto.Id = 0;

                        OS_JobQualificationTemplate_Dto added = await OS_JobTemplateAppService.AddQualificationTemplate(jobQualificationTemplate_Dto);

                        if (AuditingManager.Current != null)
                        {
                            EntityChangeInfo entityChangeInfo = new EntityChangeInfo();
                            entityChangeInfo.EntityId = added.Id.ToString();
                            entityChangeInfo.EntityTenantId = added.TenantId;
                            entityChangeInfo.ChangeTime = DateTime.Now;
                            entityChangeInfo.ChangeType = EntityChangeType.Created;
                            entityChangeInfo.EntityTypeFullName = typeof(OS_JobQualificationTemplate).FullName;

                            AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                        }

                        added = ObjectMapper.Map<OS_JobQualificationTemplate, OS_JobQualificationTemplate_Dto>(await OS_JobTemplateAppService.QualificationsRepository.GetAsync(added.Id));

                        return StatusCode(200, added);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return StatusCode(500);
        }
        public async Task<IActionResult> OnDeleteJobQualificationTemplate()
        {
            List<OS_JobQualificationTemplate_Dto> entitites = JsonSerializer.Deserialize<List<OS_JobQualificationTemplate_Dto>>(Request.Form["qualifications"]);
            try
            {
                for (int i = 0; i < entitites.Count; i++)
                {
                    OS_JobQualificationTemplate_Dto entity = entitites[i];
                    //await JobQualificationTemplatesAppService.Repository.DeleteAsync(leaveRequest.);
                    await OS_JobTemplateAppService.QualificationsRepository.DeleteAsync(entity.Id);

                    if (AuditingManager.Current != null)
                    {
                        EntityChangeInfo entityChangeInfo = new EntityChangeInfo();
                        entityChangeInfo.EntityId = entity.Id.ToString();
                        entityChangeInfo.EntityTenantId = entity.TenantId;
                        entityChangeInfo.ChangeTime = DateTime.Now;
                        entityChangeInfo.ChangeType = EntityChangeType.Deleted;
                        entityChangeInfo.EntityTypeFullName = typeof(OS_JobQualificationTemplate).FullName;

                        AuditingManager.Current.Log.EntityChanges.Add(entityChangeInfo);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public dynamic GetDataAuditTrailModel()
        {
            dynamic result = new ExpandoObject();

            List<GridColumn> primaryGridColumns = new List<GridColumn>()
            {
                new GridColumn { Field = "AuditLogId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "EntityChangeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Id", HeaderText = "Id", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Name", HeaderText = "Name", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Date", HeaderText = "Date", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Time", HeaderText = "Time", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "User", HeaderText = "User", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Status", HeaderText = "Status", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "TypeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Type", HeaderText = "Type", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "ChangeStatus", HeaderText = "Change Status", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Field", HeaderText = "Field", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "OriginalValue", HeaderText = "Original Value", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "NewValue", HeaderText = "New Value", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
            };

            result.primaryGridColumns = primaryGridColumns;

            List<GridColumn> secondaryGridColumns = new List<GridColumn>()
            {
                new GridColumn { Field = "EntityChangeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "TypeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Type", HeaderText = "Type", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Status", HeaderText = "Status", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },

            };
            List<GridColumn> tertiaryGridColumns = new List<GridColumn>()
            {
                new GridColumn { Field = "EntityChangeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "TypeId", HeaderText = "", TextAlign=TextAlign.Center, Visible=false, ShowInColumnChooser=false,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "Field", HeaderText = "Field", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "OriginalValue", HeaderText = "Original Value", TextAlign=TextAlign.Center,  AllowEditing=false,  MinWidth = "10", CustomAttributes=new {id="detailed"}  },
                new GridColumn { Field = "NewValue", HeaderText = "New Value", TextAlign=TextAlign.Center,  AllowEditing=false, MinWidth = "10", CustomAttributes=new {id="detailed"}  },
            };

            Grid SecondaryAGTGrid = new Grid()
            {
                DataSource = new List<dynamic>(),
                QueryString = "EntityChangeId",
                EditSettings = new Syncfusion.EJ2.Grids.GridEditSettings() { },
                AllowExcelExport = true,
                //AllowGrouping = true,
                AllowPdfExport = true,
                HierarchyPrintMode = HierarchyGridPrintMode.All,
                AllowSelection = true,
                AllowFiltering = false,
                AllowSorting = true,
                AllowMultiSorting = true,
                AllowResizing = true,
                GridLines = GridLine.Both,
                SearchSettings = new GridSearchSettings() { },
                //Toolbar = new List<object>() { "ExcelExport", "CsvExport", "Print", "Search",new { text = "Copy", tooltipText = "Copy", prefixIcon = "e-copy", id = "copy" }, new { text = "Copy With Header", tooltipText = "Copy With Header", prefixIcon = "e-copy", id = "copyHeader" } },
                ContextMenuItems = new List<object>() { "AutoFit", "AutoFitAll", "SortAscending", "SortDescending", "Copy", "Edit", "Delete", "Save", "Cancel", "PdfExport", "ExcelExport", "CsvExport", "FirstPage", "PrevPage", "LastPage", "NextPage" },
                Columns = secondaryGridColumns
            };
            Grid TertiaryAGTGrid = new Grid()
            {
                DataSource = new List<dynamic>(),
                QueryString = "TypeId",
                EditSettings = new Syncfusion.EJ2.Grids.GridEditSettings() { },
                AllowExcelExport = true,
                //AllowGrouping = true,
                AllowPdfExport = true,
                HierarchyPrintMode = HierarchyGridPrintMode.All,
                AllowSelection = true,
                AllowFiltering = false,
                AllowSorting = true,
                AllowMultiSorting = true,
                AllowResizing = true,
                GridLines = GridLine.Both,
                SearchSettings = new GridSearchSettings() { },
                //Toolbar = new List<object>() { "ExcelExport", "CsvExport", "Print", "Search",new { text = "Copy", tooltipText = "Copy", prefixIcon = "e-copy", id = "copy" }, new { text = "Copy With Header", tooltipText = "Copy With Header", prefixIcon = "e-copy", id = "copyHeader" } },
                ContextMenuItems = new List<object>() { "AutoFit", "AutoFitAll", "SortAscending", "SortDescending", "Copy", "Edit", "Delete", "Save", "Cancel", "PdfExport", "ExcelExport", "CsvExport", "FirstPage", "PrevPage", "LastPage", "NextPage" },
                Columns = tertiaryGridColumns
            };
            SecondaryAGTGrid.ChildGrid = TertiaryAGTGrid;

            //result.secondaryGrid = SecondaryAGTGrid;
            result.dataSource = null;

            return result;
        }

        public async Task<JsonResult> OnGetDataAuditTrail()
        {
            dynamic result = new ExpandoObject();
            List<dynamic> DS = new List<dynamic>();
            List<dynamic> secondaryDS = new List<dynamic>();
            List<dynamic> tertiaryDS = new List<dynamic>();
            var departmentLogs = AuditLogsRepo.WithDetails().Where(x => x.Url == HttpContext.Request.Path.Value && x.EntityChanges != null && x.EntityChanges.Count > 0).ToList();

            List<OS_JobTemplate_Dto> Entities = await OS_JobTemplateAppService.GetAllJobTemplatesAsync();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            for (int i = 0; i < departmentLogs.Count; i++)
            {
                AuditLog auditLog = departmentLogs[i];
                if (auditLog.EntityChanges == null || auditLog.EntityChanges.Count == 0) continue;
                var entityChanges = auditLog.EntityChanges.ToList();
                for (int j = 0; j < entityChanges.Count; j++)
                {
                    EntityChange entityChange = entityChanges[j];

                    dynamic changeRow = new ExpandoObject();
                    changeRow.AuditLogId = entityChange.Id;
                    changeRow.EntityChangeId = entityChange.Id;

                    OS_JobTemplate_Dto department = Entities.First(x => x.Id.ToString() == entityChange.EntityId);
                    changeRow.Id = department.Id;
                    changeRow.Name = department.Name;
                    changeRow.Date = entityChange.ChangeTime.ToShortDateString();
                    changeRow.Time = entityChange.ChangeTime.ToShortTimeString();
                    changeRow.User = auditLog.UserName;
                    changeRow.Status = entityChange.ChangeType.GetDisplayName();

                    DS.Add(changeRow);

                    dynamic generalTypeRow = new ExpandoObject();
                    generalTypeRow.EntityChangeId = entityChange.Id;
                    generalTypeRow.TypeId = 1;
                    generalTypeRow.Type = "General";
                    generalTypeRow.Name = "";
                    generalTypeRow.Status = "Updated";

                    changeRow.Type = "General";
                    //changeRow.Name = "";
                    changeRow.ChangeStatus = "Updated";

                    secondaryDS.Add(generalTypeRow);

                    var generalPropertyChanges = entityChange.PropertyChanges.ToList();

                    for (int k = 0; k < generalPropertyChanges.Count; k++)
                    {
                        EntityPropertyChange propertyChange = generalPropertyChanges[k];
                        dynamic propertyChangeRow = new ExpandoObject();
                        propertyChangeRow.TypeId = 1;
                        propertyChangeRow.EntityChangeId = propertyChange.EntityChangeId;
                        propertyChangeRow.Field = textInfo.ToTitleCase(propertyChange.PropertyName.ToSentenceCase());
                        propertyChangeRow.NewValue = propertyChange.NewValue != "null" && propertyChange.NewValue != "\"\"" ? propertyChange.NewValue.TrimStart('"').TrimEnd('"') : "�";
                        propertyChangeRow.OriginalValue = propertyChange.OriginalValue != "null" && propertyChange.OriginalValue != "\"\"" ? propertyChange.OriginalValue.TrimStart('"').TrimEnd('"') : "�"; ;

                        changeRow.Field = textInfo.ToTitleCase(propertyChange.PropertyName.ToSentenceCase());
                        changeRow.NewValue = propertyChange.NewValue != "null" && propertyChange.NewValue != "\"\"" ? propertyChange.NewValue.TrimStart('"').TrimEnd('"') : "�";
                        changeRow.OriginalValue = propertyChange.OriginalValue != "null" && propertyChange.OriginalValue != "\"\"" ? propertyChange.OriginalValue.TrimStart('"').TrimEnd('"') : "�"; ;

                        tertiaryDS.Add(propertyChangeRow);
                    }

                    //List<EmployeeExtraPropertyHistory> extraPropertyHistories = entityChange.GetProperty<List<EmployeeExtraPropertyHistory>>("extraPropertiesHistory");
                    //if (extraPropertyHistories != null && extraPropertyHistories.Count > 0)
                    //{
                    //    foreach (EmployeeExtraPropertyHistory extraPropertyHistory in extraPropertyHistories)
                    //    {
                    //        dynamic typeRow = new ExpandoObject();
                    //        typeRow.EntityChangeId = entityChange.Id;
                    //        typeRow.TypeId = extraPropertyHistory.TypeId;
                    //        typeRow.Type = extraPropertyHistory.Type;
                    //        typeRow.Name = extraPropertyHistory.Name;
                    //        typeRow.Status = extraPropertyHistory.Status;

                    //        secondaryDS.Add(typeRow);

                    //        var propertyChanges = extraPropertyHistory.PropertyChanges.ToList();

                    //        for (int k = 0; k < propertyChanges.Count; k++)
                    //        {
                    //            EmployeeTypePropertyChange propertyChange = propertyChanges[k];
                    //            dynamic propertyChangeRow = new ExpandoObject();
                    //            propertyChangeRow.TypeId = extraPropertyHistory.TypeId;
                    //            propertyChangeRow.EntityChangeId = typeRow.EntityChangeId;
                    //            propertyChangeRow.Field = textInfo.ToTitleCase(propertyChange.PropertyName.ToSentenceCase());
                    //            propertyChangeRow.NewValue = propertyChange.NewValue != "null" && propertyChange.NewValue != "\"\"" ? propertyChange.NewValue.TrimStart('"').TrimEnd('"') : "�";
                    //            propertyChangeRow.OriginalValue = propertyChange.OriginalValue != "null" && propertyChange.OriginalValue != "\"\"" ? propertyChange.OriginalValue.TrimStart('"').TrimEnd('"') : "�"; ;

                    //            tertiaryDS.Add(propertyChangeRow);
                    //        }
                    //    }
                    //}
                }
            }
            result.ds = DS;
            result.secondaryDS = secondaryDS;
            result.tertiaryDS = tertiaryDS;

            var secondaryGrid = new JsonResult(result);
            return secondaryGrid;
        }
    }
}
