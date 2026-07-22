#pragma warning disable

namespace AwesomeAssertions.Builders;

public static class DateBuilding
{
    extension(int day)
    {
        public DateBuilder Jan(int year) => new(year, 01, day);
        public DateBuilder Feb(int year) => new(year, 02, day);
        public DateBuilder Mar(int year) => new(year, 03, day);
        public DateBuilder Apr(int year) => new(year, 04, day);
        public DateBuilder May(int year) => new(year, 05, day);
        public DateBuilder Jun(int year) => new(year, 06, day);
        public DateBuilder Jul(int year) => new(year, 07, day);
        public DateBuilder Aug(int year) => new(year, 08, day);
        public DateBuilder Sep(int year) => new(year, 09, day);
        public DateBuilder Oct(int year) => new(year, 10, day);
        public DateBuilder Nov(int year) => new(year, 11, day);
        public DateBuilder Dec(int year) => new(year, 12, day);
    }
}
