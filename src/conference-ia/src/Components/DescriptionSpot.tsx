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
        let newText = (i >= 10 ? words.slice(i - 9, i + 1) : words.slice(0, i + 1)).join(' ');
        setDisplayedText(newText);
        i++;
      } else {
        clearInterval(intervalId);
      }
    }, 1000);

    return () => clearInterval(intervalId); // Clean up on unmount
  }, [description]);

  return (
    <div className='w-full absolute bottom-0 bg-black bg-opacity-60 text-white font-bold animate-pulse p-20'>
      <p className='text-4xl'>{displayedText}</p>
    </div>
  );
};

export default DescriptionSpot;