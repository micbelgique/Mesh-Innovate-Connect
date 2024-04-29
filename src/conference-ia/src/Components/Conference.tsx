import React, { useEffect, useState } from 'react';
import TextToSpeech from './TextToSpeech';
import Carousel from './Carousel';
import DescriptionSpot from './DescriptionSpot';





const Conference: React.FC = () => {

    const [conferenceText, setConferenceText] = useState<string>("");
    const [imagesUrls, setImagesUrls] = useState<string[]>([]);
    useEffect(() => {
      const fetchConference = async () => {
          try {
                const reponse = await fetch('https://api-generateconference.azurewebsites.net//Conference/GetConference');
;               const data = await (await reponse).json();
                if (data.hasOwnProperty('ConferenceTalk')) {
                    setConferenceText(data.ConferenceTalk);
                }
                if (data.hasOwnProperty('ImagesUrls')) {
                    setImagesUrls(data.ImagesUrls);
                }
          } catch (error) {
              console.error('Error fetching conference data:', error);
          }
      };

      fetchConference();
  }, []);

  return (
    <div className="container conferenceContainer flex-row">
      <div className="container data d-flex justify-content-between">
  
        <Carousel imagesUrls={imagesUrls} />
  
        <TextToSpeech content={conferenceText} />

        <DescriptionSpot description={conferenceText} />
        
      </div>
    </div>
  );
};

export default Conference;
