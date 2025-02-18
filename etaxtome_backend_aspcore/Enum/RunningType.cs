using System.ComponentModel;

namespace MyFirestoreApi.Enums
{
    public enum RunningType
    {
        [Description("รันนิ่งใหม่ทุกวัน (YYYYMMDD)")]
        YYYYMMDD,
        
        [Description("รันนิ่งใหม่ทุกสัปดาห์ (WW)")]
        WW,
        
        [Description("รันนิ่งใหม่ทุกเดือน (YYYYMM)")]
        YYYYMM,
        
        [Description("รันนิ่งใหม่ทุกไตรมาส (Q)")]
        Q,
        
        [Description("รันนิ่งใหม่ทุกปี (YYYY)")]
        YYYY
    }
}