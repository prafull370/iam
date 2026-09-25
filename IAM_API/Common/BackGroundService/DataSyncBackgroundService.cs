using ADMIN_SERVICE.Common.Enums;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.CustomUI;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;
using RepositoryLayer.Common.IDropdown;
using RepositoryLayer.IAM.IDataSyncing;

namespace IAM_API.Common.BackGroundService
{
    public class ScheduledDataSyncService : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduledDataSyncService> _logger;

        public ScheduledDataSyncService(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduledDataSyncService> logger)
        {
            _scopeFactory = scopeFactory
                ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider
                    .GetRequiredService<IDataSyncService>();
                var Dropdown = scope.ServiceProvider
                    .GetRequiredService<IDropdownService>();
                var orchestrator = scope.ServiceProvider
                    .GetRequiredService<IDataSyncOrchestrator>();

                DDLRequestModel ddlmodel = new DDLRequestModel();
                ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.PRODUCT.ToString();
                var Products=await Dropdown.Get(ddlmodel);
                ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.SYNCTYPE.ToString();
                var SyncTypes= await Dropdown.Get(ddlmodel);

                var productddata = Products?.ResponseData??new List<DDLResponseModel>();
                foreach (var product in productddata)
                {
                    foreach (var synctypedata in SyncTypes.ResponseData)
                    {
                        var model = new DataSyncRequestModel()
                        {
                            Sync_Type_Id = synctypedata.Id,
                            Product_Id = product.Id,
                            Status = false
                        };
                        var result = await repo.Get(model);
                        if (result.ResponseData != null && result.ResponseData.Any())
                        {
                           await orchestrator.ProcessDataSyncing(
                           result.ResponseData,
                           synctypedata.Id,
                           product.Id);
                        }
                       
                    }
                }
                

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
