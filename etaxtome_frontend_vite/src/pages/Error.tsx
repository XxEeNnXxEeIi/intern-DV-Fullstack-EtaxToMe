import React, { useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Error = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const isError = localStorage.getItem('formSubmitted');
  }, [navigate]);

  return (
    <main className="flex flex-col justify-center items-center min-h-screen p-4">
      <div className="border-2 border-red-500 rounded-lg p-6 bg-white shadow-md max-w-md text-center">
        <h4 className="text-xl font-semibold text-red-500">
          Error: {localStorage.getItem('statusCode') || "Unknown Error"}
        </h4>
        <p className="mt-2 text-gray-700">
          {localStorage.getItem('errorMessage') || "An unexpected error occurred."}
        </p>
      </div>
      <Link to="/" className="mt-4 text-blue-500 underline hover:text-blue-700">
        Go back to Home
      </Link>
    </main>
  );
};

export default Error;
