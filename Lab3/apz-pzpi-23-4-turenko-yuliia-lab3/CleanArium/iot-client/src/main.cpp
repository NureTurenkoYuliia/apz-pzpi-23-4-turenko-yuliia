#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "config.h"
#include "WiFiManager.h"
#include "SensorService.h"
#include "CustomHttpClient.h"
#include "CommandProcessor.h"

#define LAMP_PIN 2

void initLamp() {
  pinMode(LAMP_PIN, OUTPUT);
  digitalWrite(LAMP_PIN, LOW);
}

void lampOn() {
  digitalWrite(LAMP_PIN, HIGH);
}

void lampOff() {
  digitalWrite(LAMP_PIN, LOW);
}

void lampBlink(int durationMs = 2500) {
  lampOn();
  delay(durationMs);
  lampOff();
}

unsigned long lastSensor = 0;
unsigned long lastCommands = 0;

void setup() {
  Serial.begin(115200);
  delay(500);
  Serial.println("ESP32 started");

  initLamp();
  connectWiFi();
}

void loop() {
  Serial.println("Loop running...");
  unsigned long now = millis();

  if (now - lastSensor > SENSOR_INTERVAL_MS) {
    if (DEVICE_TYPE == DEVICE_HEATER) { 
      SensorReading t = readTemperature();
      sendSensorData(t);

      float avg = getAverageTemperature();
      float danger = calculateDangerLevel(avg);

      Serial.println("Avg temp: " + String(avg));
      Serial.println("Danger level: " + String(danger));

      if (danger > 0.7f) {
        sendExecutedCommand(3, 3);
        Serial.println("Auto command triggered");
        lampBlink(500);
      }
    }

    if (DEVICE_TYPE == DEVICE_SENSOR) {
      sendSensorData(readPh());
      sendSensorData(readWaterLevel());
    }

    lastSensor = now;
  }

  if (now - lastCommands > COMMAND_INTERVAL_MS) {
    String cmds = fetchCommands();
    
    if (processCommands(cmds)) {
      lampBlink(300); 
    }

    lastCommands = now;
  }

  delay(5000);
}