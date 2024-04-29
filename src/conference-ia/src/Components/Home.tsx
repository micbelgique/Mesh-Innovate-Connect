import { Link } from 'react-router-dom';

const Home: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100 space-y-10">
      <h1 className="text-4xl font-bold text-blue-600">
        Page d'accueil
      </h1>
      <div className="flex space-x-4">
        <Link 
          to="/CreateConference" 
          className="px-6 py-3 bg-blue-500 text-white rounded-lg shadow-md hover:bg-blue-600 transition-colors duration-200"
        >
          Création d'une conférence
        </Link>
        <Link 
          to="/Conference" 
          className="px-6 py-3 bg-green-500 text-white rounded-lg shadow-md hover:bg-green-600 transition-colors duration-200"
        >
          Accès à la conférence
        </Link>
      </div>
    </div>
  );
};

export default Home;