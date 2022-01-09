/**
var grammar = '#JSGF V1.0; grammar colors; public <color> = aqua | azure | beige | bisque | black | blue | brown | chocolate | coral | crimson | cyan | fuchsia | ghostwhite | gold | goldenrod | gray | green | indigo | ivory | khaki | lavender | lime | linen | magenta | maroon | moccasin | navy | olive | orange | orchid | peru | pink | plum | purple | red | salmon | sienna | silver | snow | tan | teal | thistle | tomato | turquoise | violet | white | yellow ;'
var recognition = new SpeechRecognition();
var speechRecognitionList = new SpeechGrammarList();
speechRecognitionList.addFromString(grammar, 1);
recognition.grammars = speechRecognitionList;
 */

var tData = JSON.parse(config);

window.SpeechRecognition = window.webkitSpeechRecognition || window.SpeechRecognition;
let finalTranscript = '';
let recognition = new window.SpeechRecognition();

// init state for text recognition
let display = false;

recognition.interimResults = true;
recognition.maxAlternatives = 10;
recognition.continuous = true;
recognition.lang = 'de-DE';

recognition.onresult = (event) => {
    let interimTranscript = '';
    let lastDebounceTranscript = '';
    for (let i = event.resultIndex, len = event.results.length; i < len; i++) {
        let transcript = event.results[i][0].transcript;
        if (transcript === lastDebounceTranscript) {
            return;
        }

        lastDebounceTranscript = transcript;

        if (event.results[i].isFinal && (event.results[i][0].confidence > 0)) {
            if (transcript !== finalTranscript) {
                finalTranscript += transcript;
            }

            validateSpeechIntent(finalTranscript);
            interimTranscript = '';
            setTimeout(stopRecognition, 750)
        } else {
            interimTranscript += transcript;
        }
    }

    updateScroll();
    document.getElementById('speechTextOutput').innerHTML = finalTranscript + '<i style="color:#ddd;">' + interimTranscript + '</>';
}

function stopRecognition() {
    recognition.abort();
    //console.log('recognition aborted');
}

function stopRecognitionByKeyword() {
    finalTranscript = '';
    stopRecognition();
    setTimeout(() => {
        display = true;
    }, 500);
}

recognition.onend = (event) => {
    console.log('recognition ended');

    if (document.getElementById('speechStatusImageWeb') == null) {
        return;
    }

    document.getElementById('speechStatusImageWeb').src = '/icons/speech/mic_off.png';
    document.getElementById('speechStatusImageMobile').src = '/icons/speech/mic_off.png';

    finalTranscript = '';
    document.getElementById('speechTextOutput').innerHTML = ''

    recognition.start();
}

recognition.onspeechstart = (event) => {
    //console.log('speech started');
}

recognition.onspeechend = (event) => {
    //console.log('speech ended');

    if (document.getElementById('speechStatusImageWeb') == null) {
        return;
    }

    document.getElementById('speechStatusImageWeb').src = '/icons/speech/mic_off.png';
    document.getElementById('speechStatusImageMobile').src = '/icons/speech/mic_off.png';

    if (finalTranscript !== '') {
        document.getElementById('speechTextOutput').style.display = "none";
        //document.getElementById('speechStatusImageWeb').style.display = "none";
        display = false;
    }

    finalTranscript = '';
    document.getElementById('speechTextOutput').innerHTML = ''
}

recognition.onstart = (event) => {
    //console.log('recognition started');

    if (document.getElementById('speechStatusImageWeb') == null) {
        return;
    }

    if (display) {
        document.getElementById('speechStatusImageWeb').src = '/icons/speech/mic_on.png';
        document.getElementById('speechStatusImageMobile').src = '/icons/speech/mic_on.png';
        document.getElementById('speechTextOutput').style.display = "block";
        //document.getElementById('speechStatusImageWeb').style.display = "block";

        //speak("Ja?");
    }
}

recognition.start();

function updateScroll() {
    var element = document.getElementById("speechContainer");
    element.scrollTop = element.scrollHeight;
}

async function validateSpeechIntent(text) {
    var input = text.toLowerCase();

    if (input.includes('spiegel') || input.includes('spiegel')) {
        stopRecognitionByKeyword();
    }
    else if (display === true) {
        console.log("validate " + input);
        await DotNet.invokeMethodAsync('SmartMirror', 'SetSpeechInputCaller', input)
    }
}