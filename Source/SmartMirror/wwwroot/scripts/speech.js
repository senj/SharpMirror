﻿/**
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
        } else {
            interimTranscript += transcript;
        }
    }

    updateScroll();
    document.getElementById('speechTextOutput').innerHTML = finalTranscript + '<i style="color:#ddd;">' + interimTranscript + '</>';
}

recognition.onend = (event) => {
    //document.getElementById('speechStatusImageWeb').src = '/images/voice/muted.png';
    //document.getElementById('speechStatusImageMobile').src = '/images/voice/muted.png';
    console.log('recognition ended');

    document.getElementById('speechStatusImageWeb').innerHTML = '&#128360;';
    document.getElementById('speechStatusImageMobile').innerHTML = '&#128360;';

    finalTranscript = '';
    document.getElementById('speechTextOutput').innerHTML = ''

    recognition.start();
}

recognition.onspeechstart = (event) => {
    document.getElementById('speechStatusImageWeb').innerHTML = '&#128362;';
    document.getElementById('speechStatusImageMobile').innerHTML = '&#128362;';
}

recognition.onspeechend = (event) => {
    document.getElementById('speechStatusImageWeb').innerHTML = '&#128360;';
    document.getElementById('speechStatusImageMobile').innerHTML = '&#128360;';

    finalTranscript = '';
    document.getElementById('speechTextOutput').innerHTML = ''
}

recognition.onstart = (event) => {
    //document.getElementById('speechStatusImageWeb').src = '/images/voice/active.png';
    //document.getElementById('speechStatusImageMobile').src = '/images/voice/active.png';
}

recognition.start();

function updateScroll() {
    var element = document.getElementById("speechContainer");
    element.scrollTop = element.scrollHeight;
}

function validateSpeechIntent(text) {
    var input = text.toLowerCase();

    if (input.includes('ende') || input.includes('stop') || input.includes('stopp')) {
        finalTranscript = '';
    }
    else {
        DotNet.invokeMethodAsync('SmartMirror', 'SetSpeechInput', input);
    }
}

//recognition.onaudiostart = (event) => {
//    document.getElementById('speechStatusImageWeb').src = '/images/voice/speaking.png';
//    document.getElementById('speechStatusImageMobile').src = '/images/voice/speaking.png';
//}

//recognition.onaudioend = (event) => {
//    document.getElementById('speechStatusImageWeb').src = '/images/voice/active.png';
//document.getElementById('speechStatusImageMobile').src = '/images/voice/active.png';
//}

//recognition.onstart = (event) => {
//    document.getElementById('speechStatusImageWeb').src = '/images/voice/active.png';
//    document.getElementById('speechStatusImageMobile').src = '/images/voice/active.png';
//}

//recognition.onnomatch = (event) => {
//    console.log('Speech not recognised');
//    recognition.start();
//}


//function startRecognition() {
//    finalTranscript = '';

//    var domElement = document.getElementById('speechTextOutput');
//    if (domElement !== null) {
//        domElement.innerHTML = '';
//    }

//    recognition.start();
//}

//function stopRecognition() {
//    recognition.stop();
//}

//function abortRecognition() {
//    recognition.abort();
//}