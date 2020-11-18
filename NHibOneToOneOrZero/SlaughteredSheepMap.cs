using FluentNHibernate.Mapping;

namespace NHibOneToOneOrZero
{
    public class SlaughteredSheepMap : ClassMap<SlaughteredSheep>
    {
        public SlaughteredSheepMap()
        {
            Table("SlaughteredSheep");
            
            Id(x => x.SheepId).GeneratedBy.Foreign("Sheep");
            Map(x => x.DateOfSlaughter).Not.Nullable();
            HasOne(x => x.Sheep).Cascade.None();
        }
    }
}
