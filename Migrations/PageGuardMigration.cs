namespace PageGuard.Migrations
{
    using Microsoft.Extensions.Logging;
    using Schemas;
    using Umbraco.Cms.Infrastructure.Migrations;

    public class PageGuardMigration : MigrationBase
    {

        public PageGuardMigration(IMigrationContext context) : base(context)
        {
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
        }
    }
}
