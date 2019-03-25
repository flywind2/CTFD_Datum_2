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
        ResetTcpClient = 4,
        ShowToast = 5,
        CurveVisibilityChanged = 6,
        SectionChanged = 7,
        Query = 8,
        QueryChart = 9,
        HistoryCurve1 = 10,
        HistoryCurve2 = 11,
        HistoryCurve3 = 12,
        LoadRole = 13,
        ShowFault = 14
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
        RealtimeAmplificationCurve = 0x04,

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
        FinalAmplificationCurve = 0x0C,

        [Description("标准熔解曲线")]
        RealtimeMeltingCurve = 0x0D,

        [Description("导数熔解曲线")]
        DerivationMeltingCurve = 0x0E,

        [Description("外圈温度")]
        OuterRingTemperature = 0x10,

        [Description("CT值")]
        CtValue = 0x11,

        [Description("TM值")]
        TmValue = 0x12,

        [Description("查询曲线1")]
        HistoryCurve1 = 0x13,

        [Description("查询曲线2")]
        HistoryCurve2 = 0x14,

        [Description("查询曲线3")]
        HistoryCurve3 = 0x15,

        [Description("阈值")]
        Threshold = 0x16,

        [Description("故障")]
        Fault = 0x17
    }

    [Flags]
    public enum Step
    {
        [Description("温升")]
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
