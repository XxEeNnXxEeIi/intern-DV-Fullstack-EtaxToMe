import React, { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useApiKey } from '../context/ApiKeyContext'; // Adjust the import path as necessary

// Component to fetch corporate data based on the API key from the URL
const FetchAndStoreCorpData: React.FC = () => {
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const publicApiKey = queryParams.get('publicApiKey');
  const { setPublicApiKey, setCorpData, setLoading, setError } = useApiKey(); // Use the API Key context

  const fetchCorpData = async (apiKey: string) => {
    setLoading(true); // Start loading
    try {
      const response = await axios.get("http://localhost:5003/api/corp/getcorpdata", {
        headers: {
          'x-api-key': apiKey, // Use the provided API key
        },
      });
      setCorpData(response.data); // Store the corporate data in context
    } catch (error: any) {
      // Handle error and set error message in context
      const errorMessage = error.response?.data?.message || "Error fetching corporate data";
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (publicApiKey) {
      setPublicApiKey(publicApiKey); // Set the API key in context
      fetchCorpData(publicApiKey); // Fetch corporate data
      //navigate(`/publicApiKey/${publicApiKey}`); // Navigate to the desired path
    }
  }, [publicApiKey, setPublicApiKey, setCorpData, setLoading, setError]);

  return null; // This component does not render anything
};

export default FetchAndStoreCorpData;
