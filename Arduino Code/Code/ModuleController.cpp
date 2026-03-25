#include "ModuleController.h"
#include "VibMotorControl.h"
#include "ServoControl.h"
#include "buttons.h"
#include "ledcontrol.h"

int receivedCommands = 0;

void parseCSV(String input, int* output, int maxCount);
void parseCSV(String input, float* output, int maxCount);
void parseCSV(String input, unsigned long* output, int maxCount);
void parseCSV(String input, uint8_t* output, int maxCount);

void processLine(String line) {
  line.trim();
  if (line.length() == 0) return;

  if (line == "BTN") {
    // Reply with button states
    String btnStates = "BTN:";
    for (int i = 0; i < NUM_MODULES; i++) {
      btnStates += buttonStates[i]; // Assuming you store current button states here
      if (i < NUM_MODULES - 1)
        btnStates += ",";
    }
    Serial.println(btnStates);
    return;
  }

  int sepIndex = line.indexOf(':');
  if (sepIndex == -1) return;

  String label = line.substring(0, sepIndex);
  String data = line.substring(sepIndex + 1);

  if (label == "R" || label == "G" || label == "B") {
    int values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < NUM_MODULES; i++) {
      int v = values[i % 7];
      if (label == "R") ledColors[i][0] = v;
      if (label == "G") ledColors[i][1] = v;
      if (label == "B") ledColors[i][2] = v;
    }
  } else if (label == "LINSERVO" || label == "VERTSERVO" || label == "ROTSERVO") {
    float values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < 7; i++) {
      float val = values[i];
      if (label == "LINSERVO") servoTargetAngle[i * 3 + 0] = val;
      else if (label == "VERTSERVO") {
        servoTargetAngle[i * 3 + 1] = val;
        servoTargetAngle[21 + i * 2 + 0] = val;
      } else if (label == "ROTSERVO") {
        servoTargetAngle[i * 3 + 2] = val;
        servoTargetAngle[21 + i * 2 + 1] = val;
      }
    }
  } else if (label == "VIB") {
    int values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < NUM_MODULES; i++) {
      vibMotorActive[i] = values[i % 7] > 0;
    }
  } else if (label == "VIBONTIME") {
    unsigned long values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < NUM_MODULES; i++) {
      vibMotorOnTime[i] = values[i % 7];
    }
  } else if (label == "VIBOFFTIME") {
    unsigned long values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < NUM_MODULES; i++) {
      vibMotorOffTime[i] = values[i % 7];
    }
  } else if (label == "VIBINTENSITY") {
    uint8_t values[7];
    parseCSV(data, values, 7);
    for (int i = 0; i < NUM_MODULES; i++) {
      vibMotorIntensityPercent[i] = values[i % 7];
    }
  } else if (label == "SPD") {
    speedDegPerSec = data.toFloat();
  }
}

void parseCSV(String input, int* output, int maxCount) {
  int count = 0, last = 0;
  input += ",";
  for (size_t  i = 0; i < input.length(); i++) {
    if (input.charAt(i) == ',' && count < maxCount)
      output[count++] = input.substring(last, i).toInt(), last = i + 1;
  }
}

void parseCSV(String input, float* output, int maxCount) {
  int count = 0, last = 0;
  input += ",";
  for (size_t  i = 0; i < input.length(); i++) {
    if (input.charAt(i) == ',' && count < maxCount)
      output[count++] = input.substring(last, i).toFloat(), last = i + 1;
  }
}

void parseCSV(String input, unsigned long* output, int maxCount) {
  int count = 0, last = 0;
  input += ",";
  for (size_t  i = 0; i < input.length(); i++) {
    if (input.charAt(i) == ',' && count < maxCount)
      output[count++] = input.substring(last, i).toInt(), last = i + 1;
  }
}

void parseCSV(String input, uint8_t* output, int maxCount) {
  int count = 0, last = 0;
  input += ",";
  for (size_t  i = 0; i < input.length(); i++) {
    if (input.charAt(i) == ',' && count < maxCount)
      output[count++] = input.substring(last, i).toInt(), last = i + 1;
  }
}
