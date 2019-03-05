using System;
using System.ComponentModel;

namespace CTFD.Global.Common
{
    [Flags]
    public enum GlobalEvent
    {
        Test = -1,
        ShowLoginView = 0,
        ShowWorkingView = 1,
        RaiseSelectedSamplesFromRack = 2,
        RaiseSelectedSamplesFromTable = 3,
        ResetTcpServer = 4,
        ShowToast = 5,
        CurveVisibilityChanged = 6,
        SectionChanged = 7
    }

    [Flags]
    public enum ViewState
    {
        RunningView = 0,
        HistoryView = 1,
        HelpView = 2,
        SettingView = 3,
        LoginView = 4
    }

    [Flags]
    public enum ThermalChange
    {
        RemoveThermalCell = 0,
        ChangeLastThermalUnit = 1,
        ChangeFirstThermalUnit = 2,
    }

    [Flags]
    public enum Role
    {
        Enginner = 0,
        Administrator = 1,
        Operator = 2
    }

    [Flags]
    public enum Token
    {
        [Description("初始化设备")]
        Initialization = 0x00,

        [Description("参数配置")]
        Parameter = 0x01,

        [Description("开始实验")]
        Start = 0x02,

        [Description("结束实验")]
        End = 0x03,

        [Description("荧光曲线")]
        RealtimeFluorescenceCurve = 0x04,

        [Description("上盖温度")]
        UpperTemperature = 0x05,

        [Description("内圈高温")]
        InnerRingTemperature = 0x06,

        [Description("收到转速")]
        Rpm = 0x07,

        [Description("收到步骤")]
        Step = 0x08,

        [Description("查询结果")]
        Query = 0x09,

        [Description("设备状态")]
        Status = 0x0A,

        [Description("查询结果曲线")]
        QueryChart = 0x0B,

        [Description("扩增曲线")]
        AmplificationCurve = 0x0C,

        [Description("标准熔解曲线")]
        RealtimeMeltingCurve = 0x0D,

        [Description("导数熔解曲线")]
        DerivationMeltingCurve = 0x0E,

        [Description("外圈温度")]
        OuterRingTemperature = 0x10,

        [Description("CT值")]
        CtValue = 0x11
    }

    [Flags]
    public enum Step
    {
        [Description("就绪")]
        CoolUp = 0x00,

        [Description("裂解")]
        Dissociation = 0x01,

        [Description("降温")]
        Cooldown = 0x02,

        [Description("离心")]
        Centrifugation = 0x03,

        [Description("扩增")]
        Amplification = 0x04,

        [Description("出仓")]
        OutWarehouse = 0x05,

        [Description("进仓")]
        InWarehouse = 0x06,

        [Description("溶解")]
        Dissolution = 0x07,

        [Description("完成")]
        Compeleted = 0x08,
    }

    [Flags]
    public enum Status
    {
        [Description("运行")]
        Run = 0x00,

        [Description("准备")]
        CoolDown = 0x01,

        [Description("停止")]
        Stop = 0x02
    }
}
