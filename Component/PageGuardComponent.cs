
using PageGuard.Migrations;

namespace PageGuard.Component
{
    using System;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.Migrations;
    using Umbraco.Cms.Core.Scoping;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
    using Umbraco.Cms.Infrastructure.Migrations;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.PropertyEditors;
    using Umbraco.Cms.Core.Serialization;
    using Microsoft.Extensions.DependencyInjection;

    public class PageGuardComponent : IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IContentService _contentService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IServiceScopeFactory _scopeFactory;

        private const string PageGuardStatusDataTypeName = "Page Guard: Status";

        public PageGuardComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState,
            IContentService contentService, 
            IDataTypeService dataTypeService,
            IServiceScopeFactory seviceServiceScopeFactory)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
            _contentService = contentService;
            _dataTypeService = dataTypeService;
            _scopeFactory = seviceServiceScopeFactory;
        }


        void IComponent.Initialize()
        
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("PageGuard");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty).To<PageGuardMigration>("pageguard-db");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);

            // Setup DataTypes
            using var scope = _scopeFactory.CreateScope();
            var dataTypeService = scope.ServiceProvider.GetRequiredService<IDataTypeService>();

            var propertyEditorCollection = scope.ServiceProvider.GetRequiredService<PropertyEditorCollection>();
            var serializer = scope.ServiceProvider.GetRequiredService<IConfigurationEditorJsonSerializer>();

            if (dataTypeService.GetDataType(PageGuardStatusDataTypeName) is null)
            {
                propertyEditorCollection.TryGet("PageGuardStatus", out IDataEditor editor);

                _dataTypeService.Save(new DataType(editor, serializer)
                {
                    DatabaseType = ValueStorageType.Ntext,
                    CreateDate = DateTime.Now,
                    CreatorId = -1,
                    Name = PageGuardStatusDataTypeName
                });
            }

        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
