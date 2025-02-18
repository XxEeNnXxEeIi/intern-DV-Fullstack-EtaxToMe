using System.ComponentModel;

namespace MyFirestoreApi.Enums
{
    public enum PaymentMethod
    {
        [Description("เงินสด")]
        CASH,

        [Description("เครดิตการ์ด")]
        CREDIT_CARD,

        [Description("โอน")]
        TRANSFER
    }
}
