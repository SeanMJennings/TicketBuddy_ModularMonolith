using Migrations;
using Migrations.Host;

return Migration.Upgrade(Settings.Database.Connection) ? 0 : -1;