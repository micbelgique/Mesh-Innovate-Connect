import React, { useState, useEffect } from 'react';

function Carousel({ imagesUrls }: { imagesUrls: string[] }) {
  const [currentImageIndex, setCurrentImageIndex] = useState<number>(0);

  useEffect(() => {
    const intervalId = setInterval(() => {
      setCurrentImageIndex(prevIndex =>
        prevIndex === imagesUrls.length - 1 ? 0 : prevIndex + 1
      );
    }, 30000);

    return () => clearInterval(intervalId);
  }, [imagesUrls.length]);

  return (
    <div className="fixed inset-0 flex items-center justify-center overflow-hidden">
      <div className="absolute inset-0 z-10">
        {imagesUrls.map((imageUrl, index) => (
          <img
            key={index}
            src={imageUrl}
            alt={`Image ${index}`}
            className={`absolute inset-0 w-full h-full object-cover ${index === currentImageIndex ? 'block' : 'hidden'}`}
          />
        ))}
      </div>
    </div>
  );
}

export default Carousel;