using System.Data.Entity.Migrations;

namespace Treefort.EntityFramework.Eventing
{
        public class EventContextConfiguration : DbMigrationsConfiguration<EventContext>
        {
            public EventContextConfiguration()
            {
                AutomaticMigrationsEnabled = true;
                AutomaticMigrationDataLossAllowed = false;
            }

            protected override void Seed(EventContext context)
            {
            }
        } 
    }