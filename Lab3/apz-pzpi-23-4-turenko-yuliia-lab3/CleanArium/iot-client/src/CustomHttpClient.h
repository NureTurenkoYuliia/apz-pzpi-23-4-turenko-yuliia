#pragma once
#include "SensorService.h"

void sendSensorData(const SensorReading& reading);
void sendExecutedCommand(int type, int status);
String fetchCommands();