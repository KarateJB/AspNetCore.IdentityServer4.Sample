using System;

namespace AspNetCore.IdentityServer4.Core.Models.Config
{
    /// <summary>
    /// Default expiry options
    /// </summary>
    public class DefaultExpiryOptions
    {
        /// <summary>
        /// Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Day
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// Hour
        /// </summary>
        public int? Hour { get; set; }

        /// <summary>
        /// Minute
        /// </summary>
        public int? Minute { get; set; }

        /// <summary>
        /// Second
        /// </summary>
        public int? Second { get; set; }

        /// <summary>
        /// Get Expire datetime
        /// </summary>
        /// <returns>Expire datetime</returns>
        public DateTimeOffset GetExpireOn(DateTimeOffset? startOn = null)
        {
            var expireOn = startOn.HasValue ? startOn.Value : DateTimeOffset.UtcNow;

            if (this.Year.HasValue)
            {
                expireOn = expireOn.AddYears(this.Year.Value);
            }
            else if (this.Month.HasValue)
            {
                expireOn = expireOn.AddMonths(this.Month.Value);
            }
            else if (this.Day.HasValue)
            {
                expireOn = expireOn.AddDays(this.Day.Value);
            }
            else if (this.Hour.HasValue)
            {
                expireOn = expireOn.AddHours(this.Hour.Value);
            }
            else if (this.Minute.HasValue)
            {
                expireOn = expireOn.AddMinutes(this.Minute.Value);
            }
            else if (this.Second.HasValue)
            {
                expireOn = expireOn.AddSeconds(this.Second.Value);
            }

            return expireOn;
        }
    }
}
