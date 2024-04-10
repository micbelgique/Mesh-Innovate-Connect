import React, { useState, useEffect } from 'react';

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
    }, 700);

    return () => clearInterval(intervalId); // Clean up on unmount
  }, [description]);

  return (
    <div className='descriptionArea'>
      <p>{displayedText}</p>
    </div>
  );
};

export default DescriptionSpot;