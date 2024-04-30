import React, { useState } from 'react';
import { Link } from 'react-router-dom';

const CreateConference: React.FC = () => {
  const [title, setTitle] = useState('');
  const [isLoading] = useState(false);
  const [message] = useState('');

  // eslint-disable-next-line @typescript-eslint/no-unused-vars

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100 space-y-10">
      <h1 className="text-4xl font-bold text-blue-600">
        Création d'une conférence
      </h1>
      <div className="flex flex-col items-center space-y-4">
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          disabled={isLoading}
          required
          className="px-4 py-2 border border-gray-300 rounded-md"
        />
        <button 
          type="submit" 
          disabled={isLoading}
          className="px-6 py-3 bg-blue-500 text-white rounded-lg shadow-md hover:bg-blue-600 transition-colors duration-200"
        >
          Envoyer
        </button>
      </div>
      <p>{message}</p>
      {!isLoading && (
        <Link 
          to="/" 
          className="px-6 py-3 bg-green-500 text-white rounded-lg shadow-md hover:bg-green-600 transition-colors duration-200"
        >
          Retourner à l'accueil
        </Link>
      )}
    </div>
  );
};

export default CreateConference;