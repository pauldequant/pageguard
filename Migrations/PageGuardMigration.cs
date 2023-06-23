namespace PageGuard.Migrations
{
    using Microsoft.Extensions.Logging;
    using Schemas;
    using Umbraco.Cms.Infrastructure.Migrations;
    using System.Linq;
    using Umbraco.Cms.Core.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Umbraco.Cms.Core.Serialization;
    using System;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.PropertyEditors;

    public class PageGuardMigration : MigrationBase
    {
        private readonly ILocalizationService _localizationService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IServiceScopeFactory _scopeFactory;

        public PageGuardMigration(IMigrationContext context, ILocalizationService localizationService, IDataTypeService dataTypeService, IServiceScopeFactory scopeFactory) : base(context)
        {
            _localizationService = localizationService;
            _dataTypeService = dataTypeService;
            _scopeFactory = scopeFactory;
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "PageGuardMigration");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("PageGuardStatus") == false)
            {
                Create.Table<PageGuardStatusSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "PageGuardStatus");
            }

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("PageGuardHistory") == false)
            {
                Create.Table<PageGuardHistorySchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "PageGuardHistory");
            }

            // Setup Dictionary Items
            var pageGuardDictionary = _localizationService.CreateDictionaryItemWithIdentity("PageGuard", parentId: null, defaultValue: "PageGuard");

            var pageGuardNotifySubject = _localizationService.CreateDictionaryItemWithIdentity("PageGuardNotifySubject", parentId: pageGuardDictionary.Key, defaultValue: "Page Guard: CMS Page Unlock Required");
            var pageGuardDefaultSender = _localizationService.CreateDictionaryItemWithIdentity("PageGuardDefaultSender", parentId: pageGuardDictionary.Key, defaultValue: "noreply@s8080.com");

            var enLang = _localizationService.GetAllLanguages().FirstOrDefault(x => x.IsoCode == "en-US");

            if (enLang != null)
            {
                _localizationService.AddOrUpdateDictionaryValue(pageGuardDictionary, enLang, "Page Guard");
                _localizationService.Save(pageGuardDictionary);

                _localizationService.AddOrUpdateDictionaryValue(pageGuardNotifySubject, enLang, "Page Guard: CMS Page Unlock Required");
                _localizationService.Save(pageGuardNotifySubject);

                _localizationService.AddOrUpdateDictionaryValue(pageGuardDefaultSender, enLang, "no-reply@yourdomain.com");
                _localizationService.Save(pageGuardDefaultSender);
            }


        }

    }
}
