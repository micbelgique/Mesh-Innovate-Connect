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
  
    if (/^\d+$/.test(title)) {
      setMessage('Le titre ne doit pas contenir que des chiffres.');
      return;
    }

    setIsLoading(true);
    setMessage('Création en cours...');

    try {
      const response = await fetch('https://api-generateconference.azurewebsites.net//Conference/CreateConference', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Prompt : title })
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
    <div>
      <h1>Création d'une conférence</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          disabled={isLoading}
          required
        />
        <button type="submit" disabled={isLoading}>
          Créer la conférence
        </button>
      </form>
      <p>{message}</p>
      <Link to="/">Retourner au menu principal</Link>
    </div>
  );
};

export default CreateConference;