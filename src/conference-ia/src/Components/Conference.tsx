import React, { useEffect, useState } from 'react';
import Avatar from './Avatar';
import "../css/Conference.css"




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
        <div>
            <Avatar conferenceText={conferenceText} images={imagesUrls} />
        </div>
    );
};

export default Conference;
