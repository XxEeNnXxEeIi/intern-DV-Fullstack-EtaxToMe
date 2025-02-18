import React from 'react';
import { createBrowserRouter, RouterProvider, useParams } from 'react-router-dom';
import { Error, Success, HomeLayout, GetDocumentRequestDataFromCorpIdByDateRange, AddDocumentRequest } from './pages';
import FetchAndStoreCorpData from './utils/FetchAndStoreCorpData'; // Update the path as necessary
import GetCorpData from './pages/GetCorpData';
import { ApiKeyProvider } from './context/ApiKeyContext'; // Adjust the import path as necessary

const router = createBrowserRouter([
  {
    path: '/',
    element: (
      <ApiKeyProvider>
        <FetchAndStoreCorpData />
        <HomeLayout /> {/* This will be the layout that contains the Navbar */}
      </ApiKeyProvider>
    ),
    errorElement: <Error />,

    children: [
      {
        path: '/',
        element: <GetCorpData />, // Use the wrapper component here
        errorElement: <Error />,
      },
      {
        path: 'document/get/',
        element: <GetDocumentRequestDataFromCorpIdByDateRange />,
        errorElement: <Error />,
      },
      {
        path: 'document/get/:publicApiKey', // Updated path
        element: <GetDocumentRequestDataFromCorpIdByDateRange />, // Use the AddDocumentRequest component
        errorElement: <Error />,
      },
      {
        path: 'document/add/',
        element: <AddDocumentRequest />,
        errorElement: <Error />,
      },
      {
        path: 'document/add/:publicApiKey', // Updated path
        element: <AddDocumentRequest />, // Use the AddDocumentRequest component
        errorElement: <Error />,
      },
      {
        path: '/error',
        element: <Error />,
      },
      {
        path: '/success',
        element: <Success />,
      },
    ],
  },
]);

function App() {
  return (
    <>
      <RouterProvider router={router} />
    </>
  );
}

export default App;
