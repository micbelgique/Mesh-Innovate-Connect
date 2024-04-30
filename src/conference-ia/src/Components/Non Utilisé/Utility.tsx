// import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";
// import { avatarAppConfig } from "./config";
// const cogSvcRegion = avatarAppConfig.cogSvcRegion
// const cogSvcSubKey = avatarAppConfig.cogSvcSubKey
// const voiceName = avatarAppConfig.voiceName
// const avatarCharacter = avatarAppConfig.avatarCharacter
// const avatarStyle = avatarAppConfig.avatarStyle
// const avatarBackgroundColor = avatarAppConfig.avatarBackgroundColor



// export const createWebRTCConnection = (iceServerUrl: string, iceServerUsername: string, iceServerCredential: string):any => {
//     var peerConnection = new RTCPeerConnection({
//         iceServers: [{
//             urls: [ iceServerUrl ],
//             username: iceServerUsername,
//             credential: iceServerCredential
//         }]
//     })
//     return peerConnection;
// }

// export const createAvatarSynthesizer = () => {
//     const speechSynthesisConfig = SpeechSDK.SpeechConfig.fromSubscription(cogSvcSubKey, cogSvcRegion)
//     speechSynthesisConfig.speechSynthesisVoiceName = voiceName;
//     const videoFormat = new SpeechSDK.AvatarVideoFormat()
//     let videoCropTopLeftX =  700
//     let videoCropBottomRightX = 1220
//     videoFormat.setCropRange(new SpeechSDK.Coordinate(videoCropTopLeftX, 50), new SpeechSDK.Coordinate(videoCropBottomRightX, 1080));
//     const talkingAvatarCharacter = avatarCharacter
//     const talkingAvatarStyle = avatarStyle
//     const avatarConfig = new SpeechSDK.AvatarConfig(talkingAvatarCharacter, talkingAvatarStyle, videoFormat)
//     avatarConfig.backgroundColor = avatarBackgroundColor
//     let avatarSynthesizer = new SpeechSDK.AvatarSynthesizer(speechSynthesisConfig, avatarConfig)
//     avatarSynthesizer.avatarEventReceived = function (s, e) {
//         var offsetMessage = ", offset from session start: " + e.offset / 10000 + "ms."
//         if (e.offset === 0) {
//             offsetMessage = ""
//         }
//         console.log("[" + (new Date()).toISOString() + "] Event received: " + e.description + offsetMessage)
//         if(e.description === "TurnEnd") {
//             console.log("TurnEnd event received. Stopping the avatar.")
//         }
//     }

//     console.log("return value of createAvatarSynthesizer: ", avatarSynthesizer)
//     return avatarSynthesizer;

// }