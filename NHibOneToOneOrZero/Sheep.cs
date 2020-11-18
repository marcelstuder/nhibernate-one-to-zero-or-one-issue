namespace NHibOneToOneOrZero
{
    public class Sheep
    {
        public virtual int SheepId { get; set; }
        public virtual string Name { get; set; }
        public virtual SlaughteredSheep SlaughterInfo { get; protected set; }

        public virtual void RegisterSlaughterInfo(SlaughteredSheep slaughterInfo)
        {
            SlaughterInfo = slaughterInfo;
        }
    }
}
