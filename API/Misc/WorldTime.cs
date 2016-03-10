using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTA.Misc
{
    /// <summary>
    /// 12 hour world time for use with plugins. It provides easy modifications of game time.
    /// </summary>
    public struct WorldTime
    {
        /// <summary>
        /// The maximum time possible.
        /// </summary>
        public const double TimeMax = 86400;

        /// <summary>
        /// The minimum time
        /// </summary>
        public const double TimeMin = 0;

        /// <summary>
        /// Gets or sets the hour.
        /// </summary>
        /// <value>The hour.</value>
        public byte Hour { get; set; }

        /// <summary>
        /// Gets or sets the minute.
        /// </summary>
        /// <value>The minute.</value>
        public byte Minute { get; set; }

        /// <summary>
        /// The period flag for a 12-hour clock
        /// </summary>
        /// <value><c>true</c> if A; otherwise, <c>false</c>.</value>
        public bool AM { get; set; }

        /// <summary>
        /// Translated game time version of the current instance time
        /// </summary>
        /// <value>The game time.</value>
        public double GameTime
        {
            get
            {
                var time = ((this.Hour * 60.0 * 60.0) + (this.Minute * 60.0));

                if (!this.AM && this.Hour < 12)
                    time += 12.0 * 60.0 * 60.0;
                else if (this.AM && this.Hour == 12)
                    time -= 12.0 * 60.0 * 60.0;

                time -= 4.5 * 60.0 * 60.0;

                if (time < 0) time = TimeMax + time;

                return time;
            }
        }

        /// <summary>
        /// Parses time in the format of HH:mm[am|pm]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static WorldTime? Parse(string input)
        {
            var split = input.Split(':');
            if (split.Length == 2)
            {
                byte hour, minute;
                if (Byte.TryParse(split[0], out hour) && split[1].Length > 2 && hour < 13)
                {
                    if (Byte.TryParse(split[1].Substring(0, split[1].Length - 2), out minute) && minute < 60)
                    {
                        var tk = split[1].Remove(0, split[1].Length - 2);

                        return new WorldTime()
                        {
                            Hour = hour,
                            Minute = minute,
                            AM = tk.ToLower() == "am"
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Parses game time to a WorldTime instance
        /// </summary>
        /// <param name="time">Time.</param>
        public static WorldTime? Parse(double time)
        {
            time += 4.5 * 60.0 * 60.0;

            if (time > TimeMax) time = time - TimeMax;

            bool am = time < 12.0 * 60.0 * 60.0 || time == TimeMax;
            if (!am) time -= 12.0 * 60.0 * 60.0;

            var hour = (int)(time / 60.0 / 60.0);
            var min = (int)((time - (hour * 60.0 * 60.0)) / 60.0);

            if (hour == 0) hour = 12;
            if (hour > 12) hour -= 12;

            return new WorldTime()
            {
                Hour = (byte)hour,
                Minute = (byte)min,
                AM = am
            };
        }

        /// <summary>
        /// Formats to 12-hour time
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OTA.Command.WorldTime"/>.</returns>
        public override string ToString()
        {
            return String.Format("{0}:{1:00} {2}", Hour, Minute, AM ? "AM" : "PM");
        }

#if true
        public static bool Test()
        {
            if ((new WorldTime()
            {
                Hour = 4,
                Minute = 30,
                AM = true
            }).ToString() != "4:30 AM")
                return false;

            if (WorldTime.Parse(0).ToString() != "4:30 AM")
                return false;

            if (WorldTime.Parse("12:00pm").Value.GameTime != 27000) //43200.0)
                return false;
            if (WorldTime.Parse("12:00am").Value.GameTime != WorldTime.TimeMax - (4.5 * 60.0 * 60.0)) //43200.0)
            {
                var aa = WorldTime.Parse("12:00am").Value.GameTime;
                var a = WorldTime.Parse("12:30am").Value.GameTime;
                var ab = WorldTime.Parse("12:59am").Value.GameTime;
                var b = WorldTime.Parse("1:00am").Value.GameTime;
                var c = WorldTime.Parse("3:00am").Value.GameTime;
                var d = WorldTime.Parse("4:00am").Value.GameTime;
                var f = WorldTime.Parse("5:00am").Value.GameTime;
                return false;
            }

            var _12 = WorldTime.Parse("12:00am").Value.GameTime;
            var parsed = WorldTime.Parse(_12);
            if (parsed.ToString() != "12:00 AM")
            {
                return false;
            }

            _12 = WorldTime.Parse("12:01am").Value.GameTime;
            parsed = WorldTime.Parse(_12);
            if (parsed.ToString() != "12:01 AM")
            {
                return false;
            }

            for (var h = 1; h <= 24; h++)
            {
                for (var m = 0; m < 60; m++)
                {
                    var time = String.Format("{0}:{1:00} {2}", h > 12 ? h - 12 : h, m, h < 12 ? "AM" : "PM");

                    System.Diagnostics.Debug.WriteLine("Testing time " + time);
                    var t = Parse(time);
                    if (t.ToString() != time)
                    {
                        return false;
                    }

                    var t2 = Parse(t.Value.GameTime);
                    if (t2.ToString() != time)
                    {
                        Parse(t.Value.GameTime);
                        Parse(t.Value.GameTime);
                        return false;
                    }

                    //if (t2.ToString() == (new WorldTime()
                    //{
                    //    Hour = (byte)h,
                    //    Minute = (byte)m,
                    //    AM = h <= 12
                    //}).ToString())
                    //{
                    //    return false;
                    //}
                }
            }

            return true;
        }
#endif
    }
}
