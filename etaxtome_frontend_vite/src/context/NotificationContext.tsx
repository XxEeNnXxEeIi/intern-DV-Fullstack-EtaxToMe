import React, { createContext, useContext, useState, ReactNode } from 'react';

// Define types for the notification context
interface Notification {
  error: {
    statusCode: number | null;
    message: string | null;
  };
  success: {
    message: string | null;
  };
}

interface NotificationContextType {
  notification: Notification;
  setNotification: React.Dispatch<React.SetStateAction<Notification>>;
  clearNotifications: () => void;
}

// Create the NotificationContext
const NotificationContext = createContext<NotificationContextType | undefined>(undefined);

// Create a provider component
export const NotificationProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [notification, setNotification] = useState<Notification>({
    error: {
      statusCode: null,
      message: null,
    },
    success: {
      message: null,
    },
  });

  const clearNotifications = () => {
    setNotification({
      error: {
        statusCode: null,
        message: null,
      },
      success: {
        message: null,
      },
    });
  };

  return (
    <NotificationContext.Provider value={{ notification, setNotification, clearNotifications }}>
      {children}
    </NotificationContext.Provider>
  );
};

// Custom hook to use the NotificationContext
export const useNotification = (): NotificationContextType => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error('useNotification must be used within a NotificationProvider');
  }
  return context;
};
