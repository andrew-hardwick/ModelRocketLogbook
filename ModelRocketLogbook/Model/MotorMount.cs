using System;

namespace ModelRocketLogbook.Model
{
    public enum MotorMount { None, _6mm, _13mm, _18mm, _24mm, _29mm, _38mm, _54mm }

    public static class MotorMountExtensions
    {
        public static MotorMount ToMotorMount(this string value, bool ignoreCase = true)
        {
            if (value.Equals(MotorMount.None.ToString()))
            {
                return MotorMount.None;
            }
            else
            {
                return(MotorMount)Enum.Parse(typeof(MotorMount), $"_{value}");
            }
        }

        public static string ToFormattedString(this MotorMount value)
            => value.ToString().Replace("_", string.Empty);
    }
}