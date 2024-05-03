import React, { useState } from 'react';
import { Link } from 'react-router-dom';

const CreateConference: React.FC = () => {
  const [title, setTitle] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!title || !title.trim()) {
      setMessage('Vous devez écrire un contexte.');
      return;
    }

    setIsLoading(true);
    setMessage('Création en cours...');

    try {
      const response = await fetch('https://api-generateconference.azurewebsites.net/Conference/CreateConference', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Prompt: title })
      });

      if (response.ok) {
        setMessage('La conférence a été créée.');
      } else {
        setMessage('La création de la conférence a échoué.');
      }
    } catch (error) {
      setMessage('Une erreur est survenue lors de la création de la conférence.');
    } finally {
      setIsLoading(false);
    }
  };

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
          onClick={handleSubmit}
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