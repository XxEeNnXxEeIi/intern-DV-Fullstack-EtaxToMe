// src/context/ApiKeyContext.tsx
import React, { createContext, useContext, useState } from 'react';
import axios from 'axios';

// Define the context type
interface ApiKeyContextType {
    publicApiKey: string | null;
    setPublicApiKey: (key: string) => void;
    corpData: any; // Replace with a more specific type if possible
    loading: boolean;
    error: string | null;
    setCorpData: (data: any) => void; // Function to set corpData
    setLoading: (loading: boolean) => void; // Function to set loading
    setError: (error: string | null) => void; // Function to set error
}

// Create the context
const ApiKeyContext = createContext<ApiKeyContextType | undefined>(undefined);

// Provider component for the API key context
export const ApiKeyProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [publicApiKey, setPublicApiKey] = useState<string | null>(null);
    const [corpData, setCorpData] = useState<any>(null); // Replace with a more specific type if possible
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    return (
        <ApiKeyContext.Provider value={{ publicApiKey, setPublicApiKey, corpData, loading, error, setCorpData, setLoading, setError }}>
            {children}
        </ApiKeyContext.Provider>
    );
};

// Custom hook to use the API key context
export const useApiKey = () => {
    const context = useContext(ApiKeyContext);
    if (context === undefined) {
        throw new Error('useApiKey must be used within an ApiKeyProvider');
    }
    return context;
};
