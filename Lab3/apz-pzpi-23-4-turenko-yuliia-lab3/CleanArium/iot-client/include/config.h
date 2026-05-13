#pragma once

#define WIFI_SSID "Wokwi-GUEST"
#define WIFI_PASSWORD ""

#define BASE_URL "https://cleanarium-g8d5eudwdna4gga6.westeurope-01.azurewebsites.net" 
#define DEVICE_ID 1

enum DeviceType {
  DEVICE_HEATER = 1,
  DEVICE_LAMP = 2,
  DEVICE_SENSOR = 3
};

#define DEVICE_TYPE 1

#define SENSOR_INTERVAL_MS   300
#define COMMAND_INTERVAL_MS  15000

#define OPTIMAL_TEMP 25.0f
#define MAX_TEMP_DEVIATION 5.0f
#define TEMP_AVG_WINDOW 5