import React, { useEffect, useState } from 'react';
import Carousel from './Carousel';
import TextToSpeech from './TextToSpeech';
import "../css/Conference.css"




const Conference: React.FC = () => {

    const [conferenceText, setConferenceText] = useState<string>('');
    const [imagesUrls, setImagesUrls] = useState<string[]>([]);
    useEffect(() => {
      const fetchConference = async () => {
          try {
            const reponse = fetch('https://localhost:7177/Conference/GetConference');
;               const data = await (await reponse).json();
                
                if (data.hasOwnProperty('ConferenceTalk')) {
                    setConferenceText(data.ConferenceTalk);
                }
                if (data.hasOwnProperty('ImagesUrl')) {
                    setImagesUrls(data.ImagesUrl);
                }
          } catch (error) {
              console.error('Error fetching conference data:', error);
          }
      };

      fetchConference();
  }, []);

    return (
        <div className="div-parent">
            <Carousel imagesUrls={imagesUrls} />
            <TextToSpeech content={conferenceText} />   
        </div>
    );
};

export default Conference;
