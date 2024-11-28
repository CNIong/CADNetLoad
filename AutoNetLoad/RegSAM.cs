namespace AutoNetLoad
{
    using System;

    [Flags]
    public enum RegSAM
    {
        QueryValue = 1,
        SetValue = 2,
        CreateSubKey = 4,
        EnumerateSubKeys = 8,
        Notify = 0x10,
        CreateLink = 0x20,
        WOW64_32Key = 0x200,
        WOW64_64Key = 0x100,
        WOW64_Res = 0x300,
        Read = 0x20019,
        Write = 0x20006,
        Execute = 0x20019,
        AllAccess = 0xf003f
    }
}

