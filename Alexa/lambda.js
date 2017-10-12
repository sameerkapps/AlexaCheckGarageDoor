////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyrights @2017 Sameer Khandekar
/// License: MIT
////////////////////////////////////////////////////////////////////////////////////////////////////////////
exports.handler = (event, context, callback) => {
    try {
        console.log("event.session.application.applicationId=" + event.session.application.applicationId);

        // if it is a new session, do initialization
        if (event.session.new) {
            onSessionStarted({requestId: event.request.requestId}, event.session);
        }

        // detemine the type of request and call apporpriate function
        if (event.request.type === "LaunchRequest") {
            onLaunch(event.request,
                event.session,
                function callback(sessionAttributes, speechletResponse) {
                    context.succeed(buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === "IntentRequest") {
            onIntent(event.request,
                event.session,
                function callback(sessionAttributes, speechletResponse) {
                    context.succeed(buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === "SessionEndedRequest") {
            onSessionEnded(event.request, event.session);
            context.succeed();
        }
    } catch (e) {
        context.fail("Exception: " + e);
    }
};


// Do initialization here. This just does logging as no initialization is required
function onSessionStarted(sessionStartedRequest, session) {
    console.log("onSessionStarted requestId=" + sessionStartedRequest.requestId +
        ", sessionId=" + session.sessionId);
}

// Called when the user ends the session.
// Not when the skill returns shouldEndSession=true.
function onSessionEnded(sessionEndedRequest, session) {
    console.log("onSessionEnded requestId=" + sessionEndedRequest.requestId +
        ", sessionId=" + session.sessionId);
    // Clean up would go here. None required for this lambda.
}

// This function gets called when user invokes the skill with no additional intention
function onLaunch(launchRequest, session, callback) {
    console.log("onLaunch requestId=" + launchRequest.requestId +
        ", sessionId=" + session.sessionId);

    // create a welcome message and send that to Alexa
    getWelcomeResponse(callback);
}

// This is called when user utters intent for GMT/Zulu time
function onIntent(intentRequest, session, callback) {
    console.log("onIntent requestId=" + intentRequest.requestId +
        ", sessionId=" + session.sessionId);

    // log the intent name for debugging
    console.log("onIntent name=" + intentRequest.intent.name);        

    var intent = intentRequest.intent,
        intentName = intentRequest.intent.name;

    // Check which intent has been called and call appropriate function.
    if ("DoorCheck" === intentName) {
        checkDoorInSession(intent, session, callback);
    } 
    else if ("AMAZON.HelpIntent" === intentName) {
        getHelpResponse(intent, session, callback);
    }
    else {
        throw "Invalid intent";
    }
}

// This provides welcome message to the user.
function getWelcomeResponse(callback) {
    // declare session attributes
    var sessionAttributes = {};
    // define title for the card
    var cardTitle = "Welcome";

    // here is what user will hear
    var speechOutput = "Welcome to the Door Check. " +
        "You can ask something like, is garage open or check door.";

    // if user does not respond, this will be played after a few seconds
    var repromptText = "Are you there? , " +
        "check your garage door";

    // make sure that the session is not ended
    var shouldEndSession = false;

    // build the speech that user will hear
    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}

// Help response is provided when user asks for help during the session
// make sure that this is more elaborate than the standard welcome message.
function getHelpResponse(intent, session, callback) {
    // declare session attributes
    var sessionAttributes = {};
    // define title for the card.
    var cardTitle = "Door Check Help";
    // here is the help provided to the user
    var speechOutput = "If you would like to know if the door is open or closed, just say, " + " Ask Door Check to Check Door.";

    // if user does not respond, this will be played after a few seconds
    var repromptText = "Are you there? , " +
        "ask to check door";

    // make sure that the session is not ended
    var shouldEndSession = false;

    // build the speech that user will hear
    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}

// This function gets called when user asks for ZuluTime/GMT
function checkDoorInSession(intent, session, callback) {
    // declare title of the card
    var cardTitle = intent.name;
    // session should end after this
    var shouldEndSession = true;
    // declare variables
    var speechOutput = "Door Check in development";
    var sessionAttributes = {};
    // reprompt text
    var repromptText = "are you there?";

    // the real logic to get value
    var http = require('https');
    
    var options = {
      host: 'YOUR SERVICE.azurewebsites.net',
      port: 443,
      path: '/api/GarageDoor?doorName="GarageDoor1"',
      headers: {
          'Content-Type': 'application/json;charset=UTF-8',
          'GarageDoorSecurityKey': 'YOUR KEY'
      }
    };

    http.get(options, function(res) {
      console.log("Got response: " + res.statusCode);
    
      res.on("data", function(chunk) {
        console.log("BODY: " + chunk);
        speechOutput = "Door Status is " + chunk;
    // call helper that will compose speech and should session end attribute
    callback(sessionAttributes,
         buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));        
      });
    }).on('error', function(e) {
        console.log("Got error: " + e.message);
        speechOutput = "Error occured checking the door";
        callback(sessionAttributes,
            buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));   
    });
    console.log("post-Get");
}


// This takes title, output speech, reprompt text and flag indicating, if session should end
// and composes that in JSON
function buildSpeechletResponse(title, output, repromptText, shouldEndSession) {
    return {
        outputSpeech: {
            type: "PlainText",
            text: output
        },
        card: {
            type: "Simple",
            title: title,
            content: output
        },
        reprompt: {
            outputSpeech: {
                type: "PlainText",
                text: repromptText
            }
        },
        shouldEndSession: shouldEndSession
    };
}

// This takes speech JSON, session attributes and builds the final response
// that will be returned
function buildResponse(sessionAttributes, speechletResponse) {
    return {
        version: "1.0",
        sessionAttributes: sessionAttributes,
        response: speechletResponse
    };
}
