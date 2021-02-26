using System;

namespace DaprCleanArchitecture.Domain.Enums
{
    [Flags]
    public enum QualityCode
    {
        Unknown = 0,

        Bad = 0x00000001,
        BadConfigurationError = 0x00000005,
        BadNotConnected = 0x00000009,
        BadDeviceFailure = 0x0000000D,
        BadWaitingForInitialData = 0x00000015,
        BadNoCommunication = 0x00000019,
        BadOutOfService = 0x0000001D,

        Uncertain = 0x00000040,
        UncertainLastUsableValue = 0x00000044,
        UncertainSensorNotAccurate = 0x00000050,
        UncertainEngineeringUnitsExceeded = 0x00000054,

        Good = 0x000000C0,
        GoodLocalOverride = 0x000000D8,
    }
}
