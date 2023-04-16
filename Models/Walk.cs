namespace VIber.Bot_ASP.NET.Core.Models
{
    public sealed class Walk
    {
        public double KilometersWalked { get; set; }
        public double DurationWalk { get; set; }

        public Walk(double kilometersWalked, double durationWalk)
        {
            if(kilometersWalked < 0)
            {
                throw new ArgumentNullException("Input data cannot be less than zero.", nameof(kilometersWalked));
            }
            else if (durationWalk < 0)
            {
                throw new ArgumentNullException("Input data cannot be less than zero.", nameof(durationWalk));
            }

            KilometersWalked = kilometersWalked;
            DurationWalk = durationWalk;
        }
    }
}
