#include "CommandProcessor.h"
#include "CustomHttpClient.h"
#include <ArduinoJson.h>

bool lampState = false;

bool processCommands(const String& json) {
  if (json.length() == 0) return false;

  StaticJsonDocument<512> doc;
  deserializeJson(doc, json);

  bool executed = false;

  for (JsonObject cmd : doc.as<JsonArray>()) {
    int commandType = cmd["commandType"];

    switch (commandType) {
      case 1:
        lampState = true;
        Serial.println("Turn ON");
        executed = true;
        break;
      case 2:
        lampState = false;
        Serial.println("Turn OFF");
        executed = true;
        break;
      case 3:
        Serial.println("SetValue executed");
        executed = true;
        break;
      case 4:
        Serial.println("Calibration done");
        executed = true;
        break;
    }

    sendExecutedCommand(commandType, 3);
  }

  return executed;
}
