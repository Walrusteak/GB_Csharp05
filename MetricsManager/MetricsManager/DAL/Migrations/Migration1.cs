using FluentMigrator;

namespace MetricsManager.DAL.Migrations
{
    [Migration(1)]
    public class Migration1 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("agents").Exists())
            {
                Create.Table("agents")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Url").AsString(255).Nullable()
                    .WithColumn("Enabled").AsBoolean();
            }

            if (!Schema.Table("cpumetrics").Exists())
            {
                Create.Table("cpumetrics")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }

            if (!Schema.Table("dotnetmetrics").Exists())
            {
                Create.Table("dotnetmetrics")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }

            if (!Schema.Table("hddmetrics").Exists())
            {
                Create.Table("hddmetrics")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }

            if (!Schema.Table("networkmetrics").Exists())
            {
                Create.Table("networkmetrics")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }

            if (!Schema.Table("rammetrics").Exists())
            {
                Create.Table("rammetrics")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("AgentId").AsInt32()
                    .WithColumn("Value").AsInt32()
                    .WithColumn("Time").AsInt64();
            }
        }

        public override void Down()
        {
            Delete.Table("agents");
            Delete.Table("cpumetrics");
            Delete.Table("dotnetmetrics");
            Delete.Table("hddmetrics");
            Delete.Table("networkmetrics");
            Delete.Table("rammetrics");
        }
    }
}
