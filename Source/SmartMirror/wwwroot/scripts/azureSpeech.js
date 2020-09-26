//const speechConfig = SpeechSDK.SpeechConfig.fromSubscription("d30127fd27014d248a0e58a1435f73af", "westus");
//speechConfig.speechRecognitionLanguage = "de-DE";
//const recognizer = new SpeechSDK.SpeechRecognizer(speechConfig);

//recognizer.recognizing = (s, e) => {
//    console.log(`RECOGNIZING: Text=${e.result.text}`);
//};

//recognizer.recognized = (s, e) => {
//    DotNet.invokeMethodAsync('SmartMirror', 'RecognizerRecognized', e.result.text);
//};

//recognizer.speechStartDetected = function (s, e) {
//    DotNet.invokeMethodAsync('SmartMirror', 'RecognizerSpeechStartDetected');
//};

//// Signals that the speech service has detected that speech has stopped.
//recognizer.speechEndDetected = function (s, e) {
//    DotNet.invokeMethodAsync('SmartMirror', 'RecognizerSpeechEndDetected');
//};

//function startRecognizer() {
//    recognizer.startContinuousRecognitionAsync();
//}