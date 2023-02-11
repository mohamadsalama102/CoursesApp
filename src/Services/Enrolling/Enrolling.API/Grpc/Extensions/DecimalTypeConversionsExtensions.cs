using nagiashraf.CoursesApp.Services.Catalog.API.Protos;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Grpc.Extensions;

public static class DecimalTypeConversionsExtensions
{
    public static decimal ToDecimal(this DecimalValue decimalValue)
    {
        var nanoFactor = 1_000_000_000M;
        return decimalValue.Units + decimalValue.Nanos / nanoFactor;
    }
}
