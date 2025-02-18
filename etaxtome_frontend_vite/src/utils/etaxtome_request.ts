export type TanachodSendRequestType = {
    url: string; // The URL for the fetch request
    method?: 'GET' | 'POST' | 'PUT' | 'DELETE'; // The HTTP method
    headers?: Record<string, string> | any; // Custom headers
    body?: any; // Request body
    queryParams?: any; // Optional query parameters
    expected?: any;
}

export const getCorpId: TanachodSendRequestType = {
    url: "http://localhost:5003/api/corp/getcorpid",
    method: 'GET',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },
    body: null, // No body needed for GET requests
};

export const getCorpdata: TanachodSendRequestType = {
    url: "http://localhost:5003/api/corp/getcorpdata",
    method: 'GET',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },
    body: null, // No body needed for GET requests
};

export const addDocumentRequest: TanachodSendRequestType = {
    url: "http://localhost:5003/api/document/request/AddDocumentRequest",
    method: 'POST',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },
    body: {
        // ในนี้เอา comment ออกตอน test ได้นะถ้าไม่อยากพิมพ์แต่ละช่องด้วยตนเอง เพราะค่าพวกนี้จะ apply ให้ถ้าไม่ได้พิมพ์ใส่
        /*
        documentType: "document_invoice_e_document",
        paymentMethod: "CASH",
        buyerTaxType: "TXID",
        buyerTaxId: "1234567891011", 
        buyerBranchCode: "00001",
        buyerName: "ทดสอบจากVite",
        buyerEmail: "buyerEmail_Vite@hotmail.com",
        buyerAddress: "1/1 Vite",
        buyerAddress2: "2/2 Vite",
        buyerBranchName: "สำนักงานใหญ่ Vite",
        buyerPostCode: "10400",
        buyerPhone: "029936113",
        requestBy: "requestBy_Vite@hotmail.com",
        createdBy: "createdBy_Vite@hotmail.com",
        billNo: "987654321",
        newRequestToken: "Vite",
        files: [
            "Vite"
        ],
        items: [
            {
                productAmount: 1,
                productId: "productId_Vite",
                productName: "productName_Vite",
                productPrice: 1234
            }
        ],
        */
        billDate: new Date().toISOString(),
        createDate: new Date().toISOString(),
        updateDate: new Date().toISOString(),
    },
};


{
    items: { productAmoun: {1} }
}

export const updateDocumentRequest: TanachodSendRequestType = {
    url: "http://localhost:5003/api/document/request/UpdateDocumentRequest",
    method: 'PUT',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },
};

// Define the TanachodFetchData object
export const getDocumentRequestDataFromCorpIdByDateRange: TanachodSendRequestType = {
    url: 'http://localhost:5003/api/document/request/GetDocumentRequestDataFromCorpIdByDateRange',
    method: 'GET',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },

    queryParams: {   // Change this to be an object instead of a serialized string
        'startDate': new Date('2024-09-01').toISOString().split('T')[0], // Format date as YYYY-MM-DD
        'endDate': new Date('2024-09-30').toISOString().split('T')[0],   // Format date as YYYY-MM-DD
        'limitList': '20',  // Optional, set as needed (as string)
        'offSet': '0'       // Optional, set as needed (as string)
    },
};

/*
// คาดหวัง field ที่รับมาจะต้องมี และ ตัว Value จะใช้กำหนดชื่อ table ตามที่เราต้องการจะตั้งนะ
    expected: {
        documentType: "ประเภทเอกสารคำขอ",  // DocumentRequestBill.documentType
        paymentMethod: "ช่องทางชำระ",  // DocumentRequestBill.paymentMethod
        buyerTaxType: "ประเภทผู้ออกใบกำกับภาษี",  // DocumentRequestBuyer.buyerTaxType
        buyerTaxId: "รหัสประจำตัวผู้เสียภาษี",
        buyerBranchCode: "รหัสสาขา",  // DocumentRequestBuyer.buyerBranchCode
        buyerName: "ชื่อลูกค้า",  // DocumentRequestBuyer.buyerName
        buyerEmail: "อีเมลผู้รับ",  // DocumentRequestBuyer.buyerEmail
        buyerAddress: "ที่อยู่",  // DocumentRequestBuyer.buyerAddress
        buyerAddress2: "ที่อยู่ 2",  // DocumentRequestBuyer.buyerAddress2
        buyerBranchName: "ชื่อสาขา",  // DocumentRequestBuyer.buyerBranchName
        buyerPostCode: "รหัสไปรษณีย์",  // DocumentRequestBuyer.buyerPostCode
        buyerPhone: "เบอร์โทรศัพท์",  // DocumentRequestBuyer.buyerPhone
        requestBy: "ผู้จัดทำ",  // DocumentRequestBuyer.requestBy
        createdBy: "อีเมลผู้รับ",
        billNo: "เลขที่ใบเสร็จ",  // DocumentRequestBill.billNo
        totalAmount: "จำนวนเงินทั้งหมด",
        files: "ไฟล์",
        items: "ข้อมูลสินค้า", 
        billDate: "วันที่ใบเสร็จ",  // DocumentRequestBill.billDate
        createDate: "'วันที่สร้าง'",
        updateDate: "วันที่แก้ไข"
    },  
*/

/*
"data": [
    {
      "documentType": "document_invoice_e_document",
      "runningDocId": "YqB63LuTr38xPqcFQ9k6",
      "paymentMethod": "CASH",
      "buyerTaxId": "1234567891011",
      "buyerTaxType": "TXID",
      "buyerBranchCode": "00000",
      "buyerBranchName": "สำนักงานใหญ่ทดสอบ",
      "buyerName": "ชื่อนามสกุลทดสอบ",
      "buyerPhone": "029936113",
      "buyerEmail": "buyerEmail_อีเมลผู้รับ@hotmail.com",
      "buyerAddress": "447/55 ถนนราชปรารภทดสอบ",
      "buyerAddress2": "574/27 หมู่บ้านแอนเนกซ์ทดสอบ",
      "buyerPostCode": "10400",
      "requestBy": "requestBy@hotmail.com",
      "billNo": "1110987654321",
      "files": [
        "string"
      ],
      "items": [
        {
          "productAmount": 2,
          "productId": "0001",
          "productName": "ทดสอบ",
          "productPrice": 1234.5,
          "productTotal": 2469
        }
      ],
      "status": "New",
      "billDate": "2024-09-10T09:23:15.648686Z",
      "createDate": "2024-09-10T09:23:15.649Z",
      "updateDate": "2024-09-10T09:23:15.648687Z"
    },
],
  "meta": {
    "nextOffset": -1,
    "total": 11
  },
  "success": true,
  "message": "Documents retrieved successfully."
*/

/*
// Define and serialize the query parameters in one step
const queryParams = (() => {
    const startDate = new Date('2024-09-01'); // Set your desired start date
    const endDate = new Date('2024-09-30');   // Set your desired end date

    // Create an object and return the serialized string
    return new URLSearchParams({
        startDate: startDate.toISOString().split('T')[0], // Format date as YYYY-MM-DD
        endDate: endDate.toISOString().split('T')[0],     // Format date as YYYY-MM-DD
        limitList: '20',                                   // Optional, set as needed (as string)
        offSet: '0'                                        // Optional, set as needed (as string)
    }).toString(); // Convert to query string directly here
})();

// Serialize the query parameters to a query string
const serializedQueryParams = new URLSearchParams(queryParams);
*/


 /*
    body: {
        documentType: "document_invoice_e_document",
        paymentMethod: "CASH",
        buyerTaxType: "TXID",
        buyerTaxId: "1234567891011",
        buyerBranchCode: "00000",
        buyerName: "ชื่อนามสกุลทดสอบ",
        buyerEmail: "buyerEmail_อีเมลผู้รับ@hotmail.com",
        buyerAddress: "447/55 ถนนราชปรารภทดสอบ",
        buyerAddress2: "574/27 หมู่บ้านแอนเนกซ์ทดสอบ",
        buyerBranchName: "สำนักงานใหญ่ทดสอบ",
        buyerPostCode: "10400",
        buyerPhone: "029936113",
        requestBy: "requestBy@hotmail.com",
        createdBy: "",
        billNo: "1110987654321",
        newRequestToken: "newRequestToken",
        totalAmount: 2469,
        files: [
            "string"
        ],
        items: [
            {
                productAmount: 2,
                productId: "0001",
                productName: "ทดสอบ",
                productPrice: 1234.5
            }
        ],
        billDate: "2024-10-02T06:35:04Z",
        created_at: "2024-10-02T06:35:04Z",
        createDate: "2024-10-02T06:35:04Z",
        updated_at: "2024-10-02T06:35:04Z",
        updateDate: "2024-10-02T06:35:04Z"
    },
    */

    /*
This is data that what we got from above axios
"data": [
    {
      "documentType": "document_invoice_e_document",
      "runningDocId": "YqB63LuTr38xPqcFQ9k6",
      "paymentMethod": "CASH",
      "buyerTaxId": "1234567891011",
      "buyerTaxType": "TXID",
      "buyerBranchCode": "00000",
      "buyerBranchName": "สำนักงานใหญ่ทดสอบ",
      "buyerName": "ชื่อนามสกุลทดสอบ",
      "buyerPhone": "029936113",
      "buyerEmail": "buyerEmail_อีเมลผู้รับ@hotmail.com",
      "buyerAddress": "447/55 ถนนราชปรารภทดสอบ",
      "buyerAddress2": "574/27 หมู่บ้านแอนเนกซ์ทดสอบ",
      "buyerPostCode": "10400",
      "requestBy": "requestBy@hotmail.com",
      "billNo": "1110987654321",
      "files": [
        "string"
      ],
      "items": [
        {
          "productAmount": 2,
          "productId": "0001",
          "productName": "ทดสอบ",
          "productPrice": 1234.5,
          "productTotal": 2469
        }
      ],
      "status": "New",
      "billDate": "2024-09-10T09:23:15.648686Z",
      "createDate": "2024-09-10T09:23:15.649Z",
      "updateDate": "2024-09-10T09:23:15.648687Z"
    },
],
  "meta": {
    "nextOffset": -1,
    "total": 100
  },
  "success": true,
  "message": "Documents retrieved successfully."
*/

/*
    // Define the TanachodFetchData object
export const getDocumentRequestDataFromCorpIdByDateRange: TanachodSendRequestType = {
    url: 'http://localhost:5003/api/document/request/GetDocumentRequestDataFromCorpIdByDateRange',
    method: 'GET',
    headers: {
        'x-api-key': "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"
    },

    queryParams: {   // Change this to be an object instead of a serialized string
        'startDate': new Date('2024-09-01').toISOString().split('T')[0], // Format date as YYYY-MM-DD
        'endDate': new Date('2024-09-15').toISOString().split('T')[0],   // Format date as YYYY-MM-DD
        'limitList': '20',  // Optional, set as needed (as string)
        'offSet': '0'       // Optional, set as needed (as string)
    },

      
};
*/
