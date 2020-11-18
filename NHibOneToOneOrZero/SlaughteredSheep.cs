using System;

namespace NHibOneToOneOrZero
{
    public class SlaughteredSheep
    {
        public virtual int SheepId { get; set; }
        public virtual DateTime DateOfSlaughter { get; set; }
        public virtual Sheep Sheep { get; set; }
    }
}
