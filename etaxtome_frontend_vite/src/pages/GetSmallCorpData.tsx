import React, { useEffect } from 'react';
import { useApiKey } from '../context/ApiKeyContext';

// Labels for the corporate data fields
const CorpDataSmallLabels: { [key: string]: string } = {
  corpId: "เลขประจำตัวผู้เสียภาษี",          // Tax ID      
};

const GetSmallCorpData: React.FC = () => {
  const { publicApiKey, corpData, loading, error } = useApiKey(); // Destructure corpData, loading, and error from context

  if (error || !publicApiKey || !corpData) {
    return <p className="p-4 py-10 text-3xl text-center text-red-500">ไม่สามารถค้นหาข้อมูลสถานประกอบการได้ กรุณาลองใหม่อีกครั้ง...</p>;
  }
  
  if (loading) {
    return <p className="p-4 py-10 text-3xl text-center">กำลังโหลดข้อมูลสถานประกอบการ...</p>;
  }

  return (
    <div className="border p-4 rounded-lg">
      <h1 className="text-3xl text-center font-bold mb-5">บริษัท {corpData.corpName} {corpData.corpType}</h1>
      {loading && <p className="text-blue-500">Loading...</p>}
      {error && <p className="text-red-500">{error}</p>}
      {corpData && (
        <form className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-1 gap-4 mx-6">
            {Object.keys(CorpDataSmallLabels).map((key) => (
              <div key={key} className="flex flex-col">
                <label className="text-sm font-semibold mb-1">{CorpDataSmallLabels[key]}</label>
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

export default GetSmallCorpData;
