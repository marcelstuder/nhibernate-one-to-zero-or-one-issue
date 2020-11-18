using FluentNHibernate.Mapping;

namespace NHibOneToOneOrZero
{
    public class SheepMap : ClassMap<Sheep>
    {
        public SheepMap()
        {
            Table("Sheep");

            Id(x => x.SheepId).GeneratedBy.Identity();
            Map(x => x.Name).Length(200).Not.Nullable();
            // Bad: 
            HasOne(x => x.SlaughterInfo).Cascade.SaveUpdate();
            // Works, but with extra column in Sheep (SlaughterInfo_Id) because References is for many-to-one...
            // References(x => x.SlaughterInfo).Cascade.SaveUpdate();
        }
    }
}
