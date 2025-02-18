'use client'; // Mark as a client component

import React, { useEffect, useState } from 'react';
import { addDocumentRequest } from '../utils/etaxtome_request';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import GetSmallCorpData from './GetSmallCorpData';
import { useApiKey } from '../context/ApiKeyContext';

// enum นี้อ่านจากบนลง ถ้าอยากแก้ให้เมนูไหน อยู่ล่างอยู่บน สลับบรรทัดได้เลยนะ

enum DocumentType {
    document_invoice_e_document = "ใบแจ้งหนี้",
    document_invoice_e_tax = "ใบแจ้งหนี้/ใบกำกับภาษี e-tax",
    document_receipt_e_document = "ใบเสร็จรับเงิน/ใบกำกับภาษี",
    document_receipt_e_document_tax = "ใบเสร็จรับเงิน/ใบกำกับภาษี",
    document_receipt_e_tax = "ใบเสร็จรับเงิน/ใบกำกับภาษี e-tax",
    document_credit_note = "ใบลดหนี้"
}

enum PaymentMethod {
    cash = "เงินสด",
    creditCard = "เครดิตการ์ด",
    transfer = "โอน",
}

enum BuyerTaxType {
    nidn = "บุคคลธรรมดา",
    txid = "นิติบุคคล/บุคคลธรรมดาที่จด VAT",
}

enum DocumentRequestBuyer {
    buyerTaxType = "ประเภทผู้ออกใบกำกับภาษี",
    buyerTaxId = "เลขประจำตัวผู้เสียภาษี",
    buyerBranchCode = "รหัสสาขา",
    buyerName = "ชื่อลูกค้า",
    buyerEmail = "อีเมลผู้รับ",
    requestBy = "ผู้จัดทำ",
    buyerAddress = "ที่อยู่",
    buyerAddress2 = "ที่อยู่ 2",
    buyerBranchName = "ชื่อสาขา",
    buyerPostCode = "รหัสไปรษณีย์",
    buyerPhone = "เบอร์โทรศัพท์",
}

enum DocumentRequestBill {
    billNo = "เลขที่ใบเสร็จ",
    billDate = "วันที่ใบเสร็จ",
    documentType = "ประเภทเอกสารคำขอ",
    paymentMethod = "ช่องทางชำระ",
}

enum DocumentRequestProduct {
    productAmount = "จำนวนสินค้า",
    totalAmount = "จำนวนเงินทั้งหมด (มูลค่าสุทธิรวมภาษี 7%)",
}

export default function AddDocumentRequest() {
    const [submitFormData, setSubmitFormData] = useState<any>({}); // Use default form data from request
    const navigate = useNavigate(); // Initialize the navigate function
    const { publicApiKey, corpData } = useApiKey();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setSubmitFormData((prev: any) => ({ ...prev, [name]: value })); 
        console.log(submitFormData);
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault(); // Prevent the default form submission

        // Safeguard against undefined submitFormData
        if (!submitFormData) {
            console.error('submitFormData is undefined');
            return; // Exit the function if submitFormData is undefined
        }
        
        // Check if the billDate exists and convert it to the desired format
        if (submitFormData.billDate) {
            const localDate = new Date(submitFormData.billDate);
            const utcDateString = localDate.toISOString(); // Converts to UTC format "YYYY-MM-DDTHH:MM:SSZ"
            setSubmitFormData((prev: any) => ({
                ...prev,
                billDate: utcDateString // Update the billDate in the state to be in UTC format
            }));
        }

        // Create a new object for the final submission data
        const finalSubmitData = { ...addDocumentRequest.body, ...submitFormData };

        // Iterate over the keys in addDocumentRequest.body to ensure all required fields are present
        for (const key in addDocumentRequest.body) {
            if (!finalSubmitData[key]) {
                // If the key is missing in submitFormData, fill it with the default value
                finalSubmitData[key] = addDocumentRequest.body[key];
            }
        }

        console.log(finalSubmitData)
    
        try {
            const response = await axios.post(addDocumentRequest.url, finalSubmitData, {
                headers: { 'x-api-key': publicApiKey }, // Corrected syntax for headers
            });
            
            // Set the local storage for successful submission
            localStorage.setItem('formSubmitted', 'true'); 
            localStorage.setItem('statusCode', response.status.toString());
            localStorage.setItem('successMessage', response.status === 200 ? 'สร้างคำขอเอกสารสำเร็จ!' : response.statusText);
            navigate('/success'); // Navigate to the success page
        } catch (error: any) {
            // Set the local storage for failed submission
            localStorage.setItem('formSubmitted', 'false'); 
            localStorage.setItem('statusCode', error.response ? error.response.status.toString() : 'Network Error');  
            localStorage.setItem('errorMessage', error.response ? error.response.message: 'ล้มเหลว, กรุณาลองใหม่อีกครั้ง');   
            navigate('/error'); // Navigate to the error page
        }
    };
    
    const handleClear = () => {
        console.log('clear...')
        setSubmitFormData(''); // Reset form to initial state
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-screen p-4">
            {publicApiKey && corpData ? (
            <>
            <GetSmallCorpData />
            <h1 className="text-2xl font-bold mb-4 mt-4">สร้างคำขอเอกสารใหม่</h1>
            <form onSubmit={handleSubmit} className="w-full max-w-5xl space-y-4">
                
            {/* Document Request Bill Section */}
            <div className="border p-4 rounded-lg">
                <h2 className="text-xl font-semibold mb-2">ข้อมูลใบเสร็จ</h2>
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                {Object.entries(DocumentRequestBill).map(([key, label]) => {
                const isEnumField = key === "documentType" || key === "paymentMethod";
                const isDateField = key === "billDate";

                return (
                    <div key={key} className="flex flex-col">
                        <label className="text-sm font-semibold mb-1">{label}</label>
                        {isEnumField ? (
                            // Check if there are any options available
                            (key === "documentType" && Object.keys(DocumentType).length > 0) ? (
                                <select
                                    name={key}
                                    onChange={handleChange}
                                    className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required
                                >
                                    {Object.entries(DocumentType).map(([enumKey, enumValue]) => (
                                        <option key={enumKey} value={enumKey}>{enumValue}</option>
                                    ))}
                                </select>
                            ) : (key === "paymentMethod" && Object.keys(PaymentMethod).length > 0) ? (
                                <select
                                    name={key}
                                    onChange={handleChange}
                                    className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required
                                >
                                    {Object.entries(PaymentMethod).map(([enumKey, enumValue]) => (
                                        <option key={enumKey} value={enumKey}>{enumValue}</option>
                                    ))}
                                </select>
                            ) : (
                                // Fallback to input box if no options are available
                                <input
                                    type="text"
                                    name={key}
                                    onChange={handleChange}
                                    className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required
                                />
                            )
                        ) : isDateField ? (
                            <input
                                type="date"
                                name={key}
                                onChange={handleChange}
                                className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        ) : (
                            <input
                                type="text"
                                name={key}
                                onChange={handleChange}
                                className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        )}
                    </div>
                );
            })}
                </div>
            </div>

                {/* Document Request Product Section */}
                <div className="border p-4 rounded-lg">
                    <h2 className="text-xl font-semibold mb-2">ข้อมูลสินค้า</h2>
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                        {Object.keys(DocumentRequestProduct).map((key) => (
                            <div key={key} className="flex flex-col">
                                <label className="text-sm font-semibold mb-1">{DocumentRequestProduct[key as keyof typeof DocumentRequestProduct]}</label>
                                <input
                                    type="text"
                                    name={key}
                                    onChange={handleChange}
                                    className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required
                                />
                            </div>
                        ))}
                    </div>
                </div>

                {/* Document Request Buyer Section */}
                <div className="border p-4 rounded-lg">
                    <h2 className="text-xl font-semibold mb-2">ข้อมูลลูกค้า</h2>
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                        {Object.entries(DocumentRequestBuyer).map(([key, label]) => {
                            const isEnumField = key === "buyerTaxType"; // Check if the field is one of the enum types
                            const enumAvailable = isEnumField && typeof BuyerTaxType !== "undefined" && Object.keys(BuyerTaxType).length > 0;

                            return (
                                <div key={key} className="flex flex-col">
                                    <label className="text-sm font-semibold mb-1">{label}</label>
                                    {enumAvailable ? (
                                        <select
                                            name={key}
                                            onChange={handleChange}
                                            className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            {Object.entries(BuyerTaxType).map(([enumKey, enumValue]) => (
                                                <option key={enumKey} value={enumKey}>{enumValue}</option>
                                            ))}
                                        </select>
                                    ) : (
                                        <input
                                            type="text"
                                            name={key}
                                            onChange={handleChange}
                                            className="border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    )}
                                </div>
                            );
                        })}
                    </div>
                </div>

                <div className="flex justify-center mx-4 space-x-4">
                <button
                    type="submit"
                    className="bg-blue-500 text-white font-semibold py-2 px-4 rounded-md hover:bg-blue-600 transition"
                >
                    สร้างเอกสาร
                </button>
                <button
                    type="button" // Change to button type
                    onClick={handleClear} // Handle clear functionality
                    className="bg-[#FEAB71] text-white font-semibold py-2 px-4 rounded-md hover:bg-[#FFA07A] transition" // Custom color
                >
                    ล้างข้อมูลที่กรอก
                </button>
            </div>
            </form>
            </>
            ) : (
                <div className="text-red-500 text-3xl">ไม่สามารถค้นหาข้อมูลสถานประกอบการได้ กรุณาลองใหม่อีกครั้ง...</div> // Optional message if API key is null
            )}
        </div>
    );
}