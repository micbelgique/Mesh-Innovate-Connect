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
        <div>
            <div className="images" >
                {imagesUrls.map((imageUrl, index) => (
                    <img
                        key={index}
                        src={imageUrl}
                        alt={`Image ${index}`}
                        style={{ display: index === currentImageIndex ? 'block' : 'none'}}
                    />
                ))}
            </div>
        </div>
    );
}

export default Carousel;
