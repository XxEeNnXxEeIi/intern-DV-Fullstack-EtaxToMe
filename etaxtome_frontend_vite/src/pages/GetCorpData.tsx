import React, { useEffect } from 'react';
import { useApiKey } from '../context/ApiKeyContext';

// Labels for the corporate data fields
const CorpDataLabels: { [key: string]: string } = {
  corpName: "ชื่อบริษัท (ภาษาไทย)",                 // Thai Name
  corpNameEn: "ชื่อบริษัท (ภาษาอังกฤษ)",            // English Name
  corpType: "ประเภทบริษัท",                // Company Type
  taxType: "ประเภทผู้เสียภาษี",                   // Tax Type
  corpId: "เลขประจำตัวผู้เสียภาษี",          // Tax ID
  corpBusinessType: "รหัสประเภทธุรกิจ",   // Business Type Code
  corpAddress: "ที่อยู่",                   // Address
  branchName: "ชื่อสาขา",                      // Branch
  branchCode: "รหัสสาขา",                 // Branch Code
  corpPhone: "โทรศัพท์",                   // Phone
  corpMobilePhone: "โทรศัพท์เคลื่อนที่",   // Mobile Phone
  corpEmail: "อีเมล",                      // Email
};

const GetCorpData: React.FC = () => {
  const { publicApiKey, corpData, loading, error } = useApiKey(); // Destructure corpData, loading, and error from context

  if (error || !publicApiKey) {
    return <p className="p-4 py-10 text-3xl text-center text-red-500">ไม่สามารถค้นหาข้อมูลสถานประกอบการได้ กรุณาลองใหม่อีกครั้ง...</p>;
  }
  
  if (loading) {
    return <p className="p-4 py-10 text-3xl text-center">กำลังโหลดข้อมูลสถานประกอบการ...</p>;
  }

  return (
    <div className="border p-4 rounded-lg">
      <h1 className="text-3xl text-center font-bold mb-5">ข้อมูลบริษัท</h1>
      {loading && <p className="text-blue-500">Loading...</p>}
      {error && <p className="text-red-500">{error}</p>}
      {corpData && (
        <form className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
            {Object.keys(CorpDataLabels).map((key) => (
              <div key={key} className="flex flex-col">
                <label className="text-sm font-semibold mb-1">{CorpDataLabels[key]}</label>
                <input
                  type="text"
                  name={key}
                  value={corpData[key] || ''} // Use corpData with the key to fill the input
                  readOnly
                  className="border rounded-md p-2 bg-gray-100" // Non-editable style
                />
              </div>
            ))}
          </div>
        </form>
      )}
    </div>
  );
};

export default GetCorpData;
