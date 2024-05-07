import React, { useState, useEffect } from 'react';
import 'tailwindcss/tailwind.css';

interface DescriptionSpotProps {
  description: string;
}

const DescriptionSpot: React.FC<DescriptionSpotProps> = ({ description }) => {
  const [displayedText, setDisplayedText] = useState('');

  useEffect(() => {
    let words = description.split(' ');
    let i = 0;

    let intervalId = setInterval(() => {
      if (i < words.length) {
        let newText = words.slice(i, i + 10).join(' ');
        setDisplayedText(newText);
        i += 10;
      } else {
        clearInterval(intervalId);
      }
    }, 2000);
  
    return () => clearInterval(intervalId); // Clean up on unmount
  }, [description]);

  return (
    <div className='w-full absolute bottom-0 bg-black bg-opacity-60 text-white font-bold p-20'>
      <p className='text-4xl text-center'>{displayedText}</p>
    </div>
  );
};

export default DescriptionSpot;