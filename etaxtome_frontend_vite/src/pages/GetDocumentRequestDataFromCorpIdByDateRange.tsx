'use client';

import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { getDocumentRequestDataFromCorpIdByDateRange } from '../utils/etaxtome_request';
import GetSmallCorpData from './GetSmallCorpData';
import { useApiKey } from '../context/ApiKeyContext';

enum TableField {
    documentType = "ประเภท",
    billNo = "เลขที่ใบเสร็จ",
    totalAmount = "จำนวน",
    buyerTaxId = "รหัสผู้เสียภาษี",
    buyerName = "ชื่อผู้เสียภาษี",
    buyerEmail = "ผู้รับ",
    buyerPhone = "เบอร์โทรศัพท์",
    requestBy = "ผู้จัดทำ",
    billDate = "วันที่เอกสาร",
    createDate = "วันที่สร้าง",
    updateDate = "วันที่แก้ไข",
    status = "สถานะ",
}

// Document types for display
enum DocumentType {
    document_invoice_e_document = "ใบแจ้งหนี้",
    document_invoice_e_tax = "ใบแจ้งหนี้/ใบกำกับภาษี e-tax",
    document_receipt_e_document = "ใบเสร็จรับเงิน/ใบกำกับภาษี",
    document_receipt_e_document_tax = "ใบเสร็จรับเงิน/ใบกำกับภาษี",
    document_receipt_e_tax = "ใบเสร็จรับเงิน/ใบกำกับภาษี e-tax",
    document_credit_note = "ใบลดหนี้"
}

// Utility function to format date to dd/mm/yyyy
const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
};

export default function GetDocumentRequestDataFromCorpIdByDateRange() {
    const [formData, setFormData] = useState<any>({});
    const [result, setResult] = useState<any[]>([]);
    const [totalRecords, setTotalRecords] = useState<number>(0);
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [sortConfig, setSortConfig] = useState<{ key: string, direction: string } | null>(null);
    const [error, setError] = useState<string | null>(null); // Error state to store error message
    const { publicApiKey, corpData } = useApiKey(); // Destructure corpData, loading, and error from context
    const [startDate, setStartDate] = useState(() => {
        const date = new Date();
        date.setDate(date.getDate() - 30); // 30 days ago
        return date.toISOString().split('T')[0]; // Format to YYYY-MM-DD
    });
    
    const [endDate, setEndDate] = useState(() => {
        const date = new Date();
        return date.toISOString().split('T')[0]; // Format to YYYY-MM-DD
    });

    const rowsPerPage = 10;

    const fetchDataWithOffset = async (offset: number) => {
        try {
            const updatedFormData = { ...formData, startDate, endDate };
            
            const response = await axios.get(getDocumentRequestDataFromCorpIdByDateRange.url, {
                headers: {
                    'x-api-key': publicApiKey,
                },
                params: { ...updatedFormData, limitList: rowsPerPage.toString(), offSet: offset.toString() },
            });
    
            // Check if data is empty
            if (response.data.data.length === 0) {
                throw new Error("No data available");
            }
    
            // Set the result and total records
            setResult(response.data.data); 
            setTotalRecords(response.data.meta.total);
            setError(null); // Clear any previous errors if successful
        } catch (error: any) {
            let errorMessage: string;
            let statusCode: string;
    
            // Check if the error has a response (this occurs when the server responds with an error status)
            if (error.response) {
                statusCode = error.response.status.toString(); // HTTP status code (e.g., 400, 500)
                errorMessage = error.response.data.message || error.message; // Backend error message or fallback to Axios message
            } else {
                // Handle network errors or other issues without a response
                statusCode = 'Network Error';
                errorMessage = error.message;
            }
    
            // Set the error message in state and store it in localStorage for later use
            setError(errorMessage);
            localStorage.setItem('statusCode', statusCode);
            localStorage.setItem('errorMessage', errorMessage);
        }
    };
    
    const requestSort = (key: string) => {
        let direction = 'asc';
        if (sortConfig && sortConfig.key === key && sortConfig.direction === 'asc') {
            direction = 'desc';
        }
        setSortConfig({ key, direction });
    };

    const sortedData = () => {
        if (!sortConfig) return result;
        return [...result].sort((a, b) => {
            if (a[sortConfig.key] < b[sortConfig.key]) {
                return sortConfig.direction === 'asc' ? -1 : 1;
            }
            if (a[sortConfig.key] > b[sortConfig.key]) {
                return sortConfig.direction === 'asc' ? 1 : -1;
            }
            return 0;
        });
    };

    const handlePageChange = (newPage: number) => {
        setCurrentPage(newPage);
        const offset = (newPage - 1) * rowsPerPage;
        fetchDataWithOffset(offset);
    };

    // Calculate start and end for pagination display
    const startRecord = (currentPage - 1) * rowsPerPage + 1;
    const endRecord = Math.min(currentPage * rowsPerPage, totalRecords);
    const emptyRowsCount = rowsPerPage - result.length;

    return (
        <div className="flex flex-col items-center justify-center min-h-screen p-4">
            {publicApiKey && corpData ? (
            <>
            <GetSmallCorpData />
            <h2 className="text-2xl text-center font-bold mt-4 mb-4">รายการคำขอ</h2>
            {/* Display Error Message if Error Occurs */}
            {error ? (
                <div className="border-2 border-red-500 rounded-lg p-4 bg-white shadow-md text-center mb-4">
                    <h4 className="text-xl font-semibold text-red-500">
                        Error: {localStorage.getItem('statusCode') || "Unknown Error"}
                    </h4>
                    <p className="mt-2 text-gray-700">
                        {localStorage.getItem('errorMessage') || "An unexpected error occurred."}
                    </p>
                </div>
            ) : (
                <>
                    <div className="flex items-center mr-4">
                        <p className="text-1xl text-red-600 pb-4">หมายเหตุ: ดูย้อนหลังได้ไม่เกิน 30 วัน</p> {/* Note on the same line */}
                    </div>
                    <div className="flex mb-4 items-center"> {/* Added items-center to vertically align inputs */}
                    <label className="mr-2 text-lg font-semibold">ตั้งแต่วันที่:</label> {/* Added styling to the label */}
                    <input 
                        type="date" 
                        defaultValue={startDate} 
                        onChange={(e) => setStartDate(e.target.value)} 
                        className="border rounded px-2 py-1 mr-4"
                    />
                    <label className="mr-2 text-lg font-semibold">จนถึงวันที่:</label> {/* Added styling to the label */}
                    <input 
                        type="date" 
                        defaultValue={endDate} 
                        onChange={(e) => setEndDate(e.target.value)} 
                        className="border rounded px-2 py-1"
                    />
                    <button 
                        onClick={() => fetchDataWithOffset(0)} // Call your fetch function to refresh the data
                        className="ml-2 bg-blue-600 text-white rounded px-4 py-1 hover:bg-blue-700 transition duration-300" // Added hover effects
                    >
                        เริ่มค้นหา
                    </button>
                    </div>

                    <div className="max-w-full mx-auto border p-2 rounded-lg">
                        {/* Scrollable wrapper */}
                        <div className="overflow-x-auto">
                            <table className="min-w-full border border-gray-300">
                                <thead className="bg-[#0C3764] text-white">
                                    <tr>
                                        {Object.entries(TableField).map(([key, value]) => (
                                            <th
                                                key={key}
                                                onClick={() => requestSort(key)}
                                                className={`cursor-pointer hover:bg-blue-600 py-2 px-4 border-b ${sortConfig?.key === key ? 'text-red-600' : ''}`} // Add red color for the sorted column
                                            >
                                                {value}
                                                {sortConfig?.key === key && (sortConfig.direction === 'asc' ? <span> ▲</span> : <span> ▼</span>)}

                                            </th>
                                        ))}
                                    </tr>
                                </thead>
                                <tbody>
                                {/* Render data rows */}
                                {sortedData().map((row: any, index: number) => (
                                    <tr key={index} className="hover:bg-gray-100  h-12"> {/*h-12 ตั้งความสูง table ไว้เลยแม้ไม่มี data*/}
                                        {Object.keys(TableField).map((fieldKey) => (
                                            <td key={fieldKey} className="py-2 px-4 border-b whitespace-nowrap">
                                                {fieldKey === 'documentType'
                                                    ? DocumentType[row[fieldKey] as keyof typeof DocumentType] || row[fieldKey]
                                                    : fieldKey === 'billDate' || fieldKey === 'createDate' || fieldKey === 'updateDate'
                                                        ? formatDate(row[fieldKey]) // Format date
                                                        : row[fieldKey]}
                                            </td>
                                        ))}
                                    </tr>
                                ))}
                                {/* Render empty rows */}
                                {Array.from({ length: emptyRowsCount }).map((_, index) => (
                                    <tr key={`empty-row-${index}`} className="hover:bg-gray-100  h-12">
                                        {Object.keys(TableField).map((fieldKey) => (
                                            <td key={fieldKey} className="py-2 px-4 border-b whitespace-nowrap">
                                                {/* Empty cell */}
                                            </td>
                                        ))}
                                    </tr>
                                ))}
                            </tbody>
                            </table>
                        </div>
    
                        {/* Pagination controls */}
                        <div className="mt-4">
                            <span>{`Showing ${startRecord}-${endRecord} of ${totalRecords} items`}</span>
                            <div className="flex items-center mt-2">
                                {/* Previous Button */}
                                {currentPage > 1 && (
                                    <button
                                        onClick={() => handlePageChange(currentPage - 1)}
                                        className="px-4 py-2 border rounded mx-1 bg-gray-300 hover:bg-gray-400"
                                    >
                                        &lt; {/* Previous */}
                                    </button>
                                )}
                                
                                {/* Page Number Buttons */}
                                {(() => {
                                    const totalPages = Math.ceil(totalRecords / rowsPerPage);
                                    const maxVisiblePages = 3;

                                    // Calculate start and end page numbers
                                    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
                                    const endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

                                    // Adjust startPage if endPage is at the maximum
                                    if (endPage - startPage < maxVisiblePages - 1) {
                                        startPage = Math.max(1, endPage - maxVisiblePages + 1);
                                    }

                                    return Array.from({ length: endPage - startPage + 1 }, (_, index) => (
                                        <button
                                            key={startPage + index}
                                            onClick={() => handlePageChange(startPage + index)}
                                            className={`px-4 py-2 border rounded mx-1 ${currentPage === startPage + index ? 'bg-blue-600 text-white' : 'bg-gray-300'}`}
                                        >
                                            {startPage + index}
                                        </button>
                                    ));
                                })()}

                                {/* Next Button */}
                                {currentPage < Math.ceil(totalRecords / rowsPerPage) && (
                                    <button
                                        onClick={() => handlePageChange(currentPage + 1)}
                                        className="px-4 py-2 border rounded mx-1 bg-gray-300 hover:bg-gray-400"
                                    >
                                        &gt; {/* Next */}
                                    </button>
                                )}
                            </div>
                        </div>


                    </div>
                </>
            )}
            </>
            ) : (
                <div className="text-red-500 text-3xl">ไม่สามารถค้นหาข้อมูลสถานประกอบการได้ กรุณาลองใหม่อีกครั้ง...</div> // Optional message if API key is null
            )}
        </div>
    );    
}
