import React, { useState, useRef, useEffect } from "react";
import "../css/Avatar.css";
import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";
import { createAvatarSynthesizer, createWebRTCConnection } from "./Utility";
import { avatarAppConfig } from "./config";
import Carousel from "./Carousel";
import DescriptionSpot from "./DescriptionSpot";

interface AvatarProps {
  conferenceText: string;
  images : string[];
}

const Avatar = ({ conferenceText, images }: AvatarProps) => {
  const [avatarSynthesizer, setAvatarSynthesizer] = useState<any>(null);
  const myAvatarVideoRef = useRef<HTMLDivElement>(null);
  const myAvatarVideoEleRef = useRef<HTMLVideoElement>(null);
  const myAvatarAudioEleRef = useRef<HTMLAudioElement>(null);
  const [description, setDescription] = useState<string>("");
  const iceUrl = avatarAppConfig.iceUrl;
  const iceUsername = avatarAppConfig.iceUsername;
  const iceCredential = avatarAppConfig.iceCredential;

  const handleOnTrack = (event: any) => {
    console.log("#### Printing handle onTrack ", event);
    console.log("Printing event.track.kind ", event.track.kind);
    if (event.track.kind === "video") {
      const mediaPlayer = myAvatarVideoEleRef.current;
      mediaPlayer!.id = event.track.kind;
      mediaPlayer!.srcObject = event.streams[0];
      mediaPlayer!.autoplay = true;
      mediaPlayer!.playsInline = true;
      mediaPlayer!.addEventListener("play", () => {
        window.requestAnimationFrame(() => {});
      });
    } else {
      const audioPlayer = myAvatarAudioEleRef.current;
      audioPlayer!.srcObject = event.streams[0];
      audioPlayer!.autoplay = true;
      audioPlayer!.muted = true;
    }
  };

  const stopSession = () => {
    try {
      avatarSynthesizer.stopSpeakingAsync().then(() => {
        console.log("[" + new Date().toISOString() + "] Stop speaking request sent.");
        avatarSynthesizer.close();
      }).catch((error: any) => {
        console.error(error);
      });
    } catch (e) {
      console.error(e);
    }
  };

  const speakSelectedText = (text : string) => {
    if (!avatarSynthesizer) {
        console.error("Avatar synthesizer is not initialized.");
        return;
    }

    const audioPlayer = myAvatarAudioEleRef.current;
    audioPlayer!.muted = false;

    avatarSynthesizer.speakTextAsync(text).then((result: any) => {
        if (result.reason === SpeechSDK.ResultReason.SynthesizingAudioCompleted) {
            console.log("Speech and avatar synthesized to video stream.");
        } else {
            console.log("Unable to speak. Result ID: " + result.resultId);
            if (result.reason === SpeechSDK.ResultReason.Canceled) {
                let cancellationDetails = SpeechSDK.CancellationDetails.fromResult(result);
                console.log(cancellationDetails.reason);
                if (cancellationDetails.reason === SpeechSDK.CancellationReason.Error) {
                    console.log(cancellationDetails.errorDetails);
                }
            }
        }
    }).catch((error: any) => {
        console.error(error);
        if (avatarSynthesizer) {
            avatarSynthesizer.close();
        }
    });
};


const startSession = async () => {
  let peerConnection = createWebRTCConnection(
    iceUrl,
    iceUsername,
    iceCredential
  );
  peerConnection.ontrack = handleOnTrack;
  peerConnection.addTransceiver('video', { direction: 'sendrecv' });
  peerConnection.addTransceiver('audio', { direction: 'sendrecv' });

  let avatarSynthesizer = createAvatarSynthesizer();
  setAvatarSynthesizer(avatarSynthesizer);
  avatarSynthesizer
    .startAvatarAsync(peerConnection)
    .then((r) => {
      if (r.reason === SpeechSDK.ResultReason.SynthesizingAudioCompleted) {
        console.log("Speech and avatar synthesized to video stream.");
      }
      console.log('[' + new Date().toISOString() + '] Avatar started.');
    })
    .catch((error) => {
      console.log(
        '[' +
          new Date().toISOString() +
          '] Avatar failed to start. Error: ' +
          error
      );
    });
  };



  useEffect(() => {
    if(conferenceText){
      startSession();
      speakSelectedText(conferenceText);
      setTimeout(() => {
        if(avatarSynthesizer){
          setDescription(conferenceText); 
        }else{
          setDescription("L'avatar n'a pas pu démarrer la session");
        }
      }, 7000);
    }else{
      setDescription("Nous n'avons pas pu récupérer les données de la conférence");
    }
  }, []);

  return (
    <div className="container myAvatarContainer flex-row">
      <div className="container myAvatarVideoRootDiv d-flex justify-content-between">
  
        <Carousel imagesUrls={images} />
  
        <div className="myAvatarVideo">
          <div id="myAvatarVideo" className="myVideoDiv" ref={myAvatarVideoRef}>
            <video className="myAvatarVideoElement" ref={myAvatarVideoEleRef}></video>
            <audio ref={myAvatarAudioEleRef}></audio>
          </div>
        </div>

        <DescriptionSpot description={description} />
        
  
      </div>
    </div>
  );

};

export default Avatar;
