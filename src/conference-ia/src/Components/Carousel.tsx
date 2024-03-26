import React, { useState, useEffect } from 'react';
import "../css/Carousel.css"

function Carousel({ imagesUrls }: { imagesUrls: string[] }) {
    const [currentImageIndex, setCurrentImageIndex] = useState<number>(0);

    useEffect(() => {
        const intervalId = setInterval(() => {
            setCurrentImageIndex(prevIndex =>
                prevIndex === imagesUrls.length - 1 ? 0 : prevIndex + 1
            );
        }, 10000);

        return () => clearInterval(intervalId);
    }, [imagesUrls.length]);

    return (
        <div>
            <div id="carousel" style={{ display: 'flex', justifyContent: 'center' }}>
                {imagesUrls.map((imageUrl, index) => (
                    <img
                        key={index}
                        src={imageUrl}
                        alt={`Image ${index}`}
                        style={{ display: index === currentImageIndex ? 'block' : 'none', maxWidth: '70%'}}
                    />
                ))}
            </div>
        </div>
    );
}

export default Carousel;
