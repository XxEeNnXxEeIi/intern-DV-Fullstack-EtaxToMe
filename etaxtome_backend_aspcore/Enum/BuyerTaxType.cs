using System.ComponentModel;

namespace MyFirestoreApi.Enums
{
    public enum TaxIssuerType
    {
        [Description("บุคคลธรรมดา")]
        NIDN, 
        
        [Description("นิติบุคคล/บุคคลธรรมดาที่จด VAT")]
        TXID 
    }
}
