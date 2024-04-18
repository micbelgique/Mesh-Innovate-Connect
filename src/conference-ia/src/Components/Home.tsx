import { Link } from 'react-router-dom';

const Home: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-3xl font-bold underline text-titre mb-10">
        Page d'accueil
      </h1>
      <nav>
        <ul className="space-y-10 ">
          <li>
            <Link 
              to="/CreateConference" 
              className="px-6 py-3 bg-blue-500 text-white rounded-lg shadow-md hover:bg-blue-600"
            >
              Création d'une conférence
            </Link>
          </li>
          <li>
            <Link 
              to="/Conference" 
              className="px-6 py-3 bg-green-500 text-white rounded-lg shadow-md hover:bg-green-600"
            >
              Accès à la conférence
            </Link>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Home;