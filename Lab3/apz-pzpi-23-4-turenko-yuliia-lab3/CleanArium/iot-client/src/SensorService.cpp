#include "SensorService.h"

static float tempBuffer[TEMP_AVG_WINDOW];
static int tempIndex = 0;
static bool bufferFilled = false;

SensorReading readTemperature() {
  const float MIN_TEMP = 20.0f;
  const float MAX_TEMP = 28.0f;
  float temp = random(MIN_TEMP * 10, MAX_TEMP * 10) / 10.0f;

  tempBuffer[tempIndex++] = temp;
  if (tempIndex >= TEMP_AVG_WINDOW) {
    tempIndex = 0;
    bufferFilled = true;
  }

  return { temp, "C" };
}

float getAverageTemperature() {
  int count = bufferFilled ? TEMP_AVG_WINDOW : tempIndex;
  if (count == 0) return 0;

  float sum = 0;
  for (int i = 0; i < count; i++) {
    sum += tempBuffer[i];
  }
  return sum / count;
}

float calculateDangerLevel(float avgTemp) {
  float diff = abs(avgTemp - OPTIMAL_TEMP);
  float level = diff / MAX_TEMP_DEVIATION;
  return constrain(level, 0.0f, 1.0f);
}

SensorReading readPh() {
  return { random(65, 75) / 10.0f, "pH" };
}

SensorReading readWaterLevel() {
  return { random(0, 100), "%" };
}