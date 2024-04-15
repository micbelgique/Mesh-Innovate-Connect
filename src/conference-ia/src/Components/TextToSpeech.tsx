import React, { useEffect } from 'react';
import * as sdk from "microsoft-cognitiveservices-speech-sdk";




interface ConferenceContext {
  content: string;
}

const TextToSpeech: React.FC<ConferenceContext> = ({ content }) => {
    try {
      
      const key : string = import.meta.env.VITE_COG_SVC_SUB_KEY;
      console.log(key);
      let speechConfig = sdk.SpeechConfig.fromSubscription(key, "westeurope");
      speechConfig.speechSynthesisVoiceName = "fr-FR-CoralieNeural";
      let synthesizer = new sdk.SpeechSynthesizer(speechConfig);

      const startAudio = async () => {

        synthesizer.speakTextAsync(
          content,
          function (result: any) {
            if (result.reason === sdk.ResultReason.SynthesizingAudioCompleted) {
              console.log("synthesis finished.");
            } else {
              console.error(`Speech synthesis canceled: ${result.errorDetails} \nDid you set the speech resource key and region values?`);
            }
            synthesizer.close();
          },
          function (err: any) {
            console.trace(`Error: ${err}`);
            synthesizer.close();
          }
        );
      };
  
      useEffect(() => {
        startAudio();
      }, []);

      
    } catch (error) {
      console.trace(`Error: ${error}`);
    }
   


    return (
      <div></div>
    );
  };

export default TextToSpeech;