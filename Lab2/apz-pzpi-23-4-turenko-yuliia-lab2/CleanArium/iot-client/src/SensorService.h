#pragma once
#include <Arduino.h>
#include "config.h"

struct SensorReading {
  float value;
  const char* unit;
};

SensorReading readTemperature();
float getAverageTemperature();
float calculateDangerLevel(float avgTemp);
SensorReading readPh();
SensorReading readWaterLevel();