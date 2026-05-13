#include "CustomHttpClient.h"
#include "config.h"
#include <HTTPClient.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>

void sendSensorData(const SensorReading& reading) {
  HTTPClient http;
  WiFiClientSecure client;
  client.setInsecure();

  String url = String(BASE_URL) + "/api/Device/" + DEVICE_ID + "/sensor-data";
  http.begin(client, url);
  http.addHeader("Content-Type", "application/json");

  StaticJsonDocument<128> doc;
  doc["Value"] = reading.value;
  doc["Unit"] = reading.unit;

  String body;
  serializeJson(doc, body);
  int code = http.POST(body);

  Serial.println("POST sensor data: " + String(code));

  http.end();
}

void sendExecutedCommand(int type, int status) {
  HTTPClient http;
  WiFiClientSecure client;
  client.setInsecure();

  String url = String(BASE_URL) + "/api/Device/" + DEVICE_ID + "/executed-commands";
  http.begin(client, url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Accept", "application/json");

  StaticJsonDocument<128> doc;
  doc["CommandType"] = (type == 0) ? 3 : type;
  doc["CommandStatus"] = status;

  String body;
  serializeJson(doc, body);
  int code = http.POST(body);
  
  Serial.println("POST executed command: " + String(code));

  http.end();
}

String fetchCommands() {
  HTTPClient http;
  WiFiClientSecure client;
  client.setInsecure();

  String url = String(BASE_URL) + "/api/Device/executed-commands-by-device/" + DEVICE_ID;
  http.begin(client, url);
  int code = http.GET();
  Serial.println("GET commands code: " + String(code));

  String payload = "";
  if (code == 200) {
    payload = http.getString();
  }

  http.end();
  return payload;
}
