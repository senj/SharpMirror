var synth = window.speechSynthesis;
var voices = [];

function populateVoiceList() {
    voices = synth.getVoices();
}

populateVoiceList();

if (speechSynthesis.onvoiceschanged !== undefined) {
    speechSynthesis.onvoiceschanged = populateVoiceList;
}

function speak(input) {
    if (synth.speaking) {
        console.error('speechSynthesis.speaking');
        return;
    }

    var utterThis = new SpeechSynthesisUtterance(input);

    var selectedOption = 'Google Deutsch';
    for (i = 0; i < voices.length; i++) {
        if (voices[i].name === selectedOption) {
            utterThis.voice = voices[i];
            break;
        }
    }

    utterThis.lang = 'de-DE';
    utterThis.pitch = 1;
    utterThis.rate = 1;
    utterThis.volume = 0.5;

    utterThis.onerror = function (event) {
        console.log('An error has occurred with the speech synthesis: ' + event.error);
    }

    synth.speak(utterThis);
}