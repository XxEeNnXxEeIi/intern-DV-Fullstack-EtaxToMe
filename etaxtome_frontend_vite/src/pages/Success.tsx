import React, { useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Success = () => {
  const navigate = useNavigate();

  useEffect(() => {
    // Check if the form was submitted successfully
    const isSuccess = localStorage.getItem('formSubmitted');

    if (!isSuccess) {
    } else {
      localStorage.removeItem('formSubmitted'); // Clear the flag
      localStorage.removeItem('statusCode'); // Clear the status code
      localStorage.removeItem('successMessage'); // Clear the success message
      localStorage.removeItem('errorMessage'); // Clear the success message
    }
  }, [navigate]);

  return (
    <main className="flex flex-col justify-center items-center min-h-screen p-4">
      <div className="border-2 border-green-500 rounded-lg p-6 bg-white shadow-md max-w-md text-center">
        <h1 className="text-2xl font-semibold text-green-500">Success!</h1>
        <p className="mt-2 text-gray-700">
          {localStorage.getItem('successMessage') || 'Your action was completed successfully.'}
        </p>
      </div>
      <Link to="/" className="mt-4 text-blue-500 underline hover:text-blue-700">Go back to Home</Link>
    </main>
  );
};

export default Success;
