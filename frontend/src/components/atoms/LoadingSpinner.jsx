import React from 'react';

const LoadingSpinner = () => {
  return (
    <div className="flex items-center justify-center h-screen w-full">
      <div className="animate-spin rounded-full h-12 w-12 border-4 border-green-dark border-t-transparent"></div>
    </div>
  );
};

export default LoadingSpinner;
